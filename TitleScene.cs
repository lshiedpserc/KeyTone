using Raylib_cs;
using System;

namespace KeyTone
{
    public static class TitleScene
    {
        public static void Init() { }

        public static void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Program.CurrentState = GameState.Play;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.E))
            {
                Program.CurrentState = GameState.Edit;
            }
        }

        public static void Draw()
        {
            int screenWidth = 1280;
            int screenHeight = 720;

            Color primary = new Color(0, 219, 231, 255);
            Color secondary = new Color(255, 36, 228, 255);

            // Background Scanlines
            for (int i = 0; i < screenHeight; i += 4)
            {
                Raylib.DrawLine(0, i, screenWidth, i, new Color(0, 219, 231, 10));
            }

            // Title Glitch Effect
            float t = (float)Raylib.GetTime();
            int glitchOffset = (int)(Math.Sin(t * 30) * 2);

            int titleSize = 100;
            string title = "KEYTONE";
            int titleWidth = Raylib.MeasureText(title, titleSize);

            Raylib.DrawText(title, screenWidth/2 - titleWidth/2 - glitchOffset, 200, titleSize, new Color((byte)secondary.R, (byte)secondary.G, (byte)secondary.B, (byte)150));
            Raylib.DrawText(title, screenWidth/2 - titleWidth/2 + glitchOffset, 200, titleSize, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)150));
            Raylib.DrawText(title, screenWidth/2 - titleWidth/2, 200, titleSize, Color.White);

            // Pulse Prompt
            float alpha = (float)(Math.Sin(t * 4) * 0.5 + 0.5);
            int promptSize = 30;
            string prompt = "PRESS SPACE TO SYNC";
            int promptWidth = Raylib.MeasureText(prompt, promptSize);
            Raylib.DrawText(prompt, screenWidth/2 - promptWidth/2, 400, promptSize, new Color((byte)primary.R, (byte)primary.G, (byte)primary.B, (byte)(255 * alpha)));

            Raylib.DrawText("PRESS E FOR LEVEL EDITOR", screenWidth/2 - 150, 480, 20, Color.Gray);

            Raylib.DrawText("SYS.REQ: TRUE\nVER: 1.0.4", 30, 30, 15, new Color(0, 219, 231, 100));
        }

        public static void Unload() { }
    }
}
