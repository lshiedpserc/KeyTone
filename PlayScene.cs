using Raylib_cs;
using System;

namespace KeyTone
{
    public static class PlayScene
    {
        public static void Init()
        {
            // VirtualKeyboard and NoteManager already init in Program.cs
        }

        public static void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                AudioTimeManager.Stop();
                Program.CurrentState = GameState.Title;
            }

            double time = AudioTimeManager.GetCurrentTime();

            // Check end condition (if all notes passed, go to results)
            bool allDone = true;
            if (NoteManager.Notes.Count > 0)
            {
                foreach (var note in NoteManager.Notes)
                {
                    if (!note.IsHit && !note.IsMissed)
                    {
                        allDone = false;
                        break;
                    }
                }

                // Allow a brief delay after last note
                if (allDone && NoteManager.Notes[NoteManager.Notes.Count-1].HitTime + 2.0 < time)
                {
                    AudioTimeManager.Stop();
                    Program.CurrentState = GameState.Results;
                }
            }

            float dt = Raylib.GetFrameTime();
            VirtualKeyboard.Update(dt);
            NoteManager.Update(time);
        }

        public static void Draw()
        {
            // Grid Background
            for (int i = 0; i < 1280; i += 40)
            {
                Raylib.DrawLine(i, 0, i, 720, new Color(0, 219, 231, 10));
            }
            for (int i = 0; i < 720; i += 40)
            {
                Raylib.DrawLine(0, i, 1280, i, new Color(0, 219, 231, 10));
            }

            // HUD
            Raylib.DrawText("NEON VELOCITY", 50, 50, 30, new Color(0, 219, 231, 255));
            double time = AudioTimeManager.GetCurrentTime();
            Raylib.DrawText($"Time: {time:F3}s", 50, 90, 20, Color.White);

            VirtualKeyboard.Draw();
            NoteManager.Draw(time, 1280, 720);
        }

        public static void Unload()
        {
        }

        public static void OnEnter()
        {
            NoteManager.Init(); // Reset notes/score
            AudioTimeManager.Play();
        }
    }
}
