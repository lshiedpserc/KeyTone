using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace KeyTone
{
    public class Note
    {
        public KeyboardKey TargetKey { get; set; }
        public double HitTime { get; set; }
        public bool IsHit { get; set; } = false;
        public bool IsMissed { get; set; } = false;

        // Visuals
        public float XOffset { get; set; }
        public float YOffset { get; set; }
    }

    public enum Judgement
    {
        None,
        Perfect,
        Great,
        Miss
    }

    public static class NoteManager
    {
        public static List<Note> Notes = new List<Note>();
        public static float ApproachTime = 2.0f; // Seconds before hit time that note appears
        public static float PerfectWindow = 0.05f; // +/- 50ms
        public static float GreatWindow = 0.15f; // +/- 150ms

        public static int Score = 0;
        public static int Combo = 0;
        public static int MaxCombo = 0;

        // Judgement Display
        public static Judgement LastJudgement = Judgement.None;
        public static float JudgementTimer = 0f;

        public static void Init()
        {
            Notes.Clear();
            Score = 0;
            Combo = 0;
            MaxCombo = 0;
            LastJudgement = Judgement.None;

            // Generate some dummy notes for testing
            LoadDummyNotes();
        }

        private static void LoadDummyNotes()
        {
            double time = 2.0; // Start 2 seconds in
            KeyboardKey[] testKeys = { KeyboardKey.D, KeyboardKey.F, KeyboardKey.J, KeyboardKey.K, KeyboardKey.A, KeyboardKey.Semicolon };

            for (int i = 0; i < 50; i++)
            {
                Notes.Add(new Note
                {
                    TargetKey = testKeys[i % testKeys.Length],
                    HitTime = time
                });
                time += 0.5; // Every half second
            }
        }

        public static void Update(double currentTime)
        {
            // Process Inputs
            foreach (var kvp in VirtualKeyboard.Keys)
            {
                if (Raylib.IsKeyPressed(kvp.Key))
                {
                    HandleInput(kvp.Key, currentTime);
                }
            }

            // Process Misses
            foreach (var note in Notes)
            {
                if (!note.IsHit && !note.IsMissed)
                {
                    if (currentTime > note.HitTime + GreatWindow)
                    {
                        note.IsMissed = true;
                        RegisterJudgement(Judgement.Miss);
                    }
                }
            }

            // Update timers
            if (JudgementTimer > 0)
            {
                JudgementTimer -= Raylib.GetFrameTime();
            }
        }

        private static void HandleInput(KeyboardKey key, double currentTime)
        {
            // Find the closest active note for this key
            Note? closestNote = null;
            double minDiff = double.MaxValue;

            foreach (var note in Notes)
            {
                if (!note.IsHit && !note.IsMissed && note.TargetKey == key)
                {
                    double diff = System.Math.Abs(note.HitTime - currentTime);
                    if (diff <= GreatWindow && diff < minDiff)
                    {
                        closestNote = note;
                        minDiff = diff;
                    }
                }
            }

            if (closestNote != null)
            {
                closestNote.IsHit = true;
                if (minDiff <= PerfectWindow)
                {
                    RegisterJudgement(Judgement.Perfect);
                }
                else
                {
                    RegisterJudgement(Judgement.Great);
                }
            }
        }

        private static void RegisterJudgement(Judgement j)
        {
            LastJudgement = j;
            JudgementTimer = 1.0f; // Show for 1 second

            switch (j)
            {
                case Judgement.Perfect:
                    Score += 1000;
                    Combo++;
                    break;
                case Judgement.Great:
                    Score += 500;
                    Combo++;
                    break;
                case Judgement.Miss:
                    Combo = 0;
                    break;
            }

            if (Combo > MaxCombo) MaxCombo = Combo;
        }

        public static void Draw(double currentTime, int screenWidth, int screenHeight)
        {
            Color primary = new Color(0, 219, 231, 255);
            Color secondary = new Color(255, 36, 228, 255);

            // Draw Notes
            foreach (var note in Notes)
            {
                if (note.IsHit || note.IsMissed) continue;

                double timeToHit = note.HitTime - currentTime;

                if (timeToHit <= ApproachTime && timeToHit >= -GreatWindow)
                {
                    if (VirtualKeyboard.Keys.TryGetValue(note.TargetKey, out VirtualKey? vKey) && vKey != null)
                    {
                        float progress = 1.0f - (float)(timeToHit / ApproachTime);

                        // Target coordinates
                        float targetX = vKey.Position.X + vKey.Width / 2;
                        float targetY = vKey.Position.Y + vKey.Height / 2;

                        // Start coordinates (Top and Left/Right depending on keyboard side)
                        // Coordinate Crossing Logic:
                        float currentYFromTop = Raylib_cs.Raymath.Lerp(0, targetY, progress);
                        float startX = targetX > screenWidth / 2 ? screenWidth : 0;
                        float currentXFromSide = Raylib_cs.Raymath.Lerp(startX, targetX, progress);

                        // Draw Y-Note (Vertical Line / Box)
                        Rectangle yNoteRect = new Rectangle(targetX - 10, currentYFromTop - 10, 20, 20);
                        Raylib.DrawRectangleRounded(yNoteRect, 0.5f, 10, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)150));
                        Raylib.DrawRectangleRoundedLinesEx(yNoteRect, 0.5f, 10, 2f, primary);
                        // Vertical trail line
                        Raylib.DrawLineEx(new Vector2(targetX, 0), new Vector2(targetX, currentYFromTop), 2f, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)50));

                        // Draw X-Note (Horizontal Line / Box)
                        Rectangle xNoteRect = new Rectangle(currentXFromSide - 10, targetY - 10, 20, 20);
                        Raylib.DrawRectangleRounded(xNoteRect, 0.5f, 10, new Color((byte)secondary.R, (byte)secondary.G, (byte)secondary.B, (byte)150));
                        Raylib.DrawRectangleRoundedLinesEx(xNoteRect, 0.5f, 10, 2f, secondary);
                        // Horizontal trail line
                        Raylib.DrawLineEx(new Vector2(startX, targetY), new Vector2(currentXFromSide, targetY), 2f, new Color((byte)secondary.R, (byte)secondary.G, (byte)secondary.B, (byte)50));
                    }
                }
            }

            // Draw HUD (Combo & Judgement)
            Raylib.DrawText($"SCORE: {Score:D8}", screenWidth - 300, 50, 30, primary);
            Raylib.DrawText($"COMBO: {Combo}x", screenWidth - 300, 90, 20, secondary);

            if (JudgementTimer > 0)
            {
                Color jColor = Color.White;
                string jText = "";

                switch (LastJudgement)
                {
                    case Judgement.Perfect:
                        jColor = primary;
                        jText = "PERFECT";
                        break;
                    case Judgement.Great:
                        jColor = secondary;
                        jText = "GREAT";
                        break;
                    case Judgement.Miss:
                        jColor = Color.Red;
                        jText = "MISS";
                        break;
                }

                // Flash and fade out
                byte alpha = (byte)(255 * (JudgementTimer / 1.0f));
                jColor = new Color((byte)jColor.R, (byte)jColor.G, (byte)jColor.B, alpha);

                int fontSize = 50 + (int)((1.0f - JudgementTimer) * 20); // slightly expand
                int textWidth = Raylib.MeasureText(jText, fontSize);
                Raylib.DrawText(jText, screenWidth / 2 - textWidth / 2, screenHeight - 150, fontSize, jColor);
            }
        }
    }
}
