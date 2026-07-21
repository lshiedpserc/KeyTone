using Raylib_cs;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System;

namespace KeyTone
{
    public static class EditScene
    {
        private static bool isRecording = false;
        private static double currentOffset = 0; // For seeking/pausing

        // Settings for Editor
        private static float timelineZoom = 100f; // pixels per second
        private static Note? selectedNote = null;

        public static void Init()
        {
            VirtualKeyboard.Init(1280, 720);
        }

        public static void Update()
        {
            float dt = Raylib.GetFrameTime();
            VirtualKeyboard.Update(dt);

            // Check modifier keys for editor commands
            bool ctrlPressed = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);

            if (ctrlPressed)
            {
                // Play / Pause toggle
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    if (isRecording)
                    {
                        AudioTimeManager.Pause();
                        isRecording = false;
                    }
                    else
                    {
                        AudioTimeManager.Resume();
                        isRecording = true;
                    }
                }

                // Seek
                if (!isRecording)
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.Right))
                    {
                        currentOffset = AudioTimeManager.GetCurrentTime() + 1.0; // Forward 1s
                        AudioTimeManager.Seek(currentOffset);
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.Left))
                    {
                        currentOffset = Math.Max(0, AudioTimeManager.GetCurrentTime() - 1.0); // Back 1s
                        AudioTimeManager.Seek(currentOffset);
                    }
                }

                // Save / Load JSON
                if (Raylib.IsKeyPressed(KeyboardKey.S))
                {
                    SaveNotes("level.json");
                }
                if (Raylib.IsKeyPressed(KeyboardKey.L))
                {
                    LoadNotes("level.json");
                }

                // Exit
                if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    AudioTimeManager.Stop();
                    Program.CurrentState = GameState.Title;
                }
            }
            else if (isRecording)
            {
                // Real-time Recording Mode: record keys pressed
                double currentTime = AudioTimeManager.GetCurrentTime();
                foreach (var kvp in VirtualKeyboard.Keys)
                {
                    if (Raylib.IsKeyPressed(kvp.Key))
                    {
                        NoteManager.Notes.Add(new Note
                        {
                            TargetKey = kvp.Key,
                            HitTime = currentTime
                        });

                        // Give visual feedback
                        kvp.Value.HitGlowTimer = 1.0f;
                    }
                }
            }
            else
            {
                // Edit Mode (Not Recording): Note adjustment
                // Simple select and move for demonstration
                // Real editor would need mouse picking or keyboard navigation
            }
        }

        private static void SaveNotes(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(NoteManager.Notes, options);
            File.WriteAllText(path, json);
        }

        private static void LoadNotes(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var loadedNotes = JsonSerializer.Deserialize<List<Note>>(json);
                if (loadedNotes != null)
                {
                    NoteManager.Notes = loadedNotes;
                }
            }
        }

        public static void Draw()
        {
            // Background & Keyboard
            VirtualKeyboard.Draw();

            // HUD
            Color cColor = new Color(52, 252, 13, 255); // Tertiary Green
            Raylib.DrawText("EDITOR MODE", 50, 50, 30, cColor);

            double time = AudioTimeManager.GetCurrentTime();
            string status = isRecording ? "RECORDING (Ctrl+Space to Pause)" : "PAUSED (Ctrl+Space to Record)";
            Raylib.DrawText($"Time: {time:F3}s - {status}", 50, 90, 20, Color.White);

            Raylib.DrawText("Ctrl+S: Save | Ctrl+L: Load | Ctrl+Arrow: Seek | Ctrl+ESC: Exit", 50, 120, 15, Color.Gray);

            // Draw a simple timeline at the bottom
            DrawTimeline(time);
        }

        private static void DrawTimeline(double currentTime)
        {
            int timelineY = 650;
            Raylib.DrawRectangle(0, timelineY, 1280, 70, new Color(25, 28, 34, 200));
            Raylib.DrawLine(0, timelineY, 1280, timelineY, new Color(0, 219, 231, 100));

            // Draw Playhead
            int playheadX = 1280 / 2;
            Raylib.DrawLine(playheadX, timelineY, playheadX, 720, new Color(255, 36, 228, 255));

            // Draw Notes on Timeline
            foreach (var note in NoteManager.Notes)
            {
                float dx = (float)(note.HitTime - currentTime) * timelineZoom;
                int noteX = playheadX + (int)dx;

                if (noteX > 0 && noteX < 1280)
                {
                    Raylib.DrawCircle(noteX, timelineY + 35, 5, new Color(0, 219, 231, 255));
                    VirtualKeyboard.Keys.TryGetValue(note.TargetKey, out VirtualKey? vKey);
                    if (vKey != null)
                    {
                         Raylib.DrawText(vKey.Label, noteX - 5, timelineY + 15, 10, Color.White);
                    }
                }
            }
        }

        public static void Unload()
        {
        }

        public static void OnEnter()
        {
            AudioTimeManager.Stop(); // Start paused
            isRecording = false;
        }
    }
}
