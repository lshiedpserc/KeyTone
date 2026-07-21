using Raylib_cs;
using System;

namespace KeyTone
{
    class Program
    {
        public static GameState CurrentState = GameState.Title;
        private static GameState _previousState = GameState.Title;

        static void Main(string[] args)
        {
            const int screenWidth = 1280;
            const int screenHeight = 720;

            Raylib.InitWindow(screenWidth, screenHeight, "KeyTone");
            Raylib.InitAudioDevice();

            Raylib.SetTargetFPS(60);

            // Initialize scenes & managers
            AudioTimeManager.Init();
            VirtualKeyboard.Init(screenWidth, screenHeight);
            NoteManager.Init();

            TitleScene.Init();
            PlayScene.Init();
            EditScene.Init();
            ResultsScene.Init();

            while (!Raylib.WindowShouldClose())
            {
                // State Transition Hooks
                if (CurrentState != _previousState)
                {
                    if (CurrentState == GameState.Play) PlayScene.OnEnter();
                    if (CurrentState == GameState.Edit) EditScene.OnEnter();
                    _previousState = CurrentState;
                }

                // Update
                switch (CurrentState)
                {
                    case GameState.Title: TitleScene.Update(); break;
                    case GameState.Play: PlayScene.Update(); break;
                    case GameState.Edit: EditScene.Update(); break;
                    case GameState.Results: ResultsScene.Update(); break;
                }

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(16, 19, 26, 255)); // Surface Dim

                switch (CurrentState)
                {
                    case GameState.Title: TitleScene.Draw(); break;
                    case GameState.Play: PlayScene.Draw(); break;
                    case GameState.Edit: EditScene.Draw(); break;
                    case GameState.Results: ResultsScene.Draw(); break;
                }

                Raylib.EndDrawing();
            }

            // Unload
            AudioTimeManager.Unload();
            TitleScene.Unload();
            PlayScene.Unload();
            EditScene.Unload();
            ResultsScene.Unload();

            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
