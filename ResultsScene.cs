using Raylib_cs;
using System.Numerics;
using System;

namespace KeyTone
{
    public static class ResultsScene
    {
        public static void Init() { }

        public static void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                Program.CurrentState = GameState.Title;
            }
        }

        public static void Draw()
        {
            int screenWidth = 1280;
            int screenHeight = 720;

            Color primary = new Color(0, 219, 231, 255);
            Color secondary = new Color(255, 36, 228, 255);
            Color surfaceContainer = new Color(29, 32, 38, 200);

            // Draw Background Scanlines
            for (int i = 0; i < screenHeight; i += 4)
            {
                Raylib.DrawLine(0, i, screenWidth, i, new Color(0, 219, 231, 5));
            }

            // Calculate Rank
            string rank = "F";
            Color rankColor = Color.Red;
            int totalNotes = NoteManager.Notes.Count;
            if (totalNotes > 0)
            {
                int hitNotes = 0;
                foreach(var n in NoteManager.Notes) if(n.IsHit) hitNotes++;

                float accuracy = (float)hitNotes / totalNotes;
                if (accuracy == 1.0f) { rank = "S"; rankColor = primary; }
                else if (accuracy >= 0.9f) { rank = "A"; rankColor = secondary; }
                else if (accuracy >= 0.7f) { rank = "B"; rankColor = new Color(52, 252, 13, 255); }
                else if (accuracy >= 0.5f) { rank = "C"; rankColor = Color.Yellow; }
            }

            // Rank Glitch text effect
            float t = (float)Raylib.GetTime();
            int glitchOffset = (int)(Math.Sin(t * 20) * 3);

            Raylib.DrawText(rank, 300 + glitchOffset, 200, 200, new Color((byte)secondary.R, (byte)secondary.G, (byte)secondary.B, (byte)150));
            Raylib.DrawText(rank, 300 - glitchOffset, 200, 200, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)150));
            Raylib.DrawText(rank, 300, 200, 200, rankColor);

            Raylib.DrawText("TRACK CLEARED", 220, 420, 40, Color.White);

            // Stats Card
            Rectangle cardRect = new Rectangle(700, 150, 450, 400);
            Raylib.DrawRectangleRounded(cardRect, 0.1f, 10, surfaceContainer);
            Raylib.DrawRectangleRoundedLinesEx(cardRect, 0.1f, 10, 2f, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)100));

            Raylib.DrawText("TOTAL SCORE", 730, 180, 20, Color.Gray);
            Raylib.DrawText($"{NoteManager.Score:D8}", 730, 210, 60, primary);

            Raylib.DrawText("MAX COMBO", 730, 310, 20, Color.Gray);
            Raylib.DrawText($"{NoteManager.MaxCombo}x", 730, 340, 40, secondary);

            // Action Prompt
            float alpha = (float)(Math.Sin(t * 5) * 0.5 + 0.5);
            Raylib.DrawText("Press SPACE/ENTER to return", screenWidth/2 - 200, screenHeight - 100, 30, new Color((byte)255, (byte)255, (byte)255, (byte)(255 * alpha)));
        }

        public static void Unload() { }
    }
}
