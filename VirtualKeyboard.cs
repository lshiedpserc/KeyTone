using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace KeyTone
{
    public class VirtualKey
    {
        public KeyboardKey RaylibKey { get; set; }
        public string Label { get; set; } = "";
        public Vector2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public KeyboardKey SymmetricKey { get; set; }
        public float HitGlowTimer { get; set; } = 0f;
        public float SymmetricGlowTimer { get; set; } = 0f;
    }

    public static class VirtualKeyboard
    {
        public static Dictionary<KeyboardKey, VirtualKey> Keys = new Dictionary<KeyboardKey, VirtualKey>();

        public static void Init(int screenWidth, int screenHeight)
        {
            float keySize = 64f;
            float padding = 8f;
            float startY = screenHeight / 2f - keySize; // Center vertically slightly offset

            // Define rows
            string[] row1 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };
            KeyboardKey[] row1Keys = { KeyboardKey.One, KeyboardKey.Two, KeyboardKey.Three, KeyboardKey.Four, KeyboardKey.Five, KeyboardKey.Six, KeyboardKey.Seven, KeyboardKey.Eight, KeyboardKey.Nine, KeyboardKey.Zero, KeyboardKey.Minus, KeyboardKey.Equal };

            string[] row2 = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]" };
            KeyboardKey[] row2Keys = { KeyboardKey.Q, KeyboardKey.W, KeyboardKey.E, KeyboardKey.R, KeyboardKey.T, KeyboardKey.Y, KeyboardKey.U, KeyboardKey.I, KeyboardKey.O, KeyboardKey.P, KeyboardKey.LeftBracket, KeyboardKey.RightBracket };

            string[] row3 = { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'" };
            KeyboardKey[] row3Keys = { KeyboardKey.A, KeyboardKey.S, KeyboardKey.D, KeyboardKey.F, KeyboardKey.G, KeyboardKey.H, KeyboardKey.J, KeyboardKey.K, KeyboardKey.L, KeyboardKey.Semicolon, KeyboardKey.Apostrophe };

            string[] row4 = { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/" };
            KeyboardKey[] row4Keys = { KeyboardKey.Z, KeyboardKey.X, KeyboardKey.C, KeyboardKey.V, KeyboardKey.B, KeyboardKey.N, KeyboardKey.M, KeyboardKey.Comma, KeyboardKey.Period, KeyboardKey.Slash };

            // Stagger offsets to match realistic keyboard layout
            float[] rowOffsets = { 0f, keySize * 0.5f, keySize * 0.75f, keySize * 1.2f };

            // Calculate base X to center the keyboard
            float baseWidth = 12 * (keySize + padding);
            float startX = (screenWidth - baseWidth) / 2f;

            AddRow(row1, row1Keys, startX + rowOffsets[0], startY, keySize, padding);
            AddRow(row2, row2Keys, startX + rowOffsets[1], startY + keySize + padding, keySize, padding);
            AddRow(row3, row3Keys, startX + rowOffsets[2], startY + (keySize + padding) * 2, keySize, padding);
            AddRow(row4, row4Keys, startX + rowOffsets[3], startY + (keySize + padding) * 3, keySize, padding);

            SetupSymmetricKeys();
        }

        private static void AddRow(string[] labels, KeyboardKey[] raylibKeys, float startX, float y, float size, float padding)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                Keys[raylibKeys[i]] = new VirtualKey
                {
                    RaylibKey = raylibKeys[i],
                    Label = labels[i],
                    Position = new Vector2(startX + i * (size + padding), y),
                    Width = size,
                    Height = size
                };
            }
        }

        private static void SetupSymmetricKeys()
        {
            // Define symmetric pairs across the center (T-Y / G-H / B-N axis)
            var pairs = new (KeyboardKey left, KeyboardKey right)[]
            {
                // Row 1
                (KeyboardKey.One, KeyboardKey.Zero),
                (KeyboardKey.Two, KeyboardKey.Nine),
                (KeyboardKey.Three, KeyboardKey.Eight),
                (KeyboardKey.Four, KeyboardKey.Seven),
                (KeyboardKey.Five, KeyboardKey.Six),
                // Row 2
                (KeyboardKey.Q, KeyboardKey.P),
                (KeyboardKey.W, KeyboardKey.O),
                (KeyboardKey.E, KeyboardKey.I),
                (KeyboardKey.R, KeyboardKey.U),
                (KeyboardKey.T, KeyboardKey.Y),
                // Row 3
                (KeyboardKey.A, KeyboardKey.Semicolon),
                (KeyboardKey.S, KeyboardKey.L),
                (KeyboardKey.D, KeyboardKey.K),
                (KeyboardKey.F, KeyboardKey.J),
                (KeyboardKey.G, KeyboardKey.H),
                // Row 4
                (KeyboardKey.Z, KeyboardKey.Slash),
                (KeyboardKey.X, KeyboardKey.Period),
                (KeyboardKey.C, KeyboardKey.Comma),
                (KeyboardKey.V, KeyboardKey.M),
                (KeyboardKey.B, KeyboardKey.N),
            };

            foreach (var pair in pairs)
            {
                if (Keys.ContainsKey(pair.left) && Keys.ContainsKey(pair.right))
                {
                    Keys[pair.left].SymmetricKey = pair.right;
                    Keys[pair.right].SymmetricKey = pair.left;
                }
            }
        }

        public static void Update(float dt)
        {
            foreach (var kvp in Keys)
            {
                var key = kvp.Value;

                // Detect input
                if (Raylib.IsKeyPressed(key.RaylibKey))
                {
                    key.HitGlowTimer = 1.0f; // 1 second decay
                    if (key.SymmetricKey != KeyboardKey.Null && Keys.ContainsKey(key.SymmetricKey))
                    {
                        Keys[key.SymmetricKey].SymmetricGlowTimer = 1.0f;
                    }
                }

                // Decay timers
                if (key.HitGlowTimer > 0) key.HitGlowTimer -= dt * 3f; // Faster fade
                if (key.HitGlowTimer < 0) key.HitGlowTimer = 0;

                if (key.SymmetricGlowTimer > 0) key.SymmetricGlowTimer -= dt * 3f;
                if (key.SymmetricGlowTimer < 0) key.SymmetricGlowTimer = 0;
            }
        }

        public static void Draw()
        {
            Color primary = new Color(0, 219, 231, 255); // Cyan
            Color secondary = new Color(255, 36, 228, 255); // Magenta
            Color background = new Color(25, 28, 34, 150); // Glass panel

            foreach (var kvp in Keys)
            {
                var key = kvp.Value;

                // Base drawing
                Rectangle rect = new Rectangle(key.Position.X, key.Position.Y, key.Width, key.Height);
                Raylib.DrawRectangleRounded(rect, 0.2f, 10, background);
                // DrawRectangleRoundedLines needs 6 args in Raylib-cs 6+: rec, roundness, segments, lineThick, color
                Raylib.DrawRectangleRoundedLinesEx(rect, 0.2f, 10, 1f, new Color(0, 219, 231, 70));

                // Hit Glow (Direct press = Magenta)
                if (key.HitGlowTimer > 0)
                {
                    Color glowColor = new Color(secondary.R, secondary.G, secondary.B, (byte)(255 * key.HitGlowTimer));
                    Raylib.DrawRectangleRounded(rect, 0.2f, 10, new Color(secondary.R, secondary.G, secondary.B, (byte)(50 * key.HitGlowTimer)));
                    Raylib.DrawRectangleRoundedLinesEx(rect, 0.2f, 10, 2f, glowColor);
                }

                // Symmetric Glow (Opposite key press = Cyan bloom)
                if (key.SymmetricGlowTimer > 0)
                {
                    Color glowColor = new Color(primary.R, primary.G, primary.B, (byte)(255 * key.SymmetricGlowTimer));
                    Raylib.DrawRectangleRounded(rect, 0.2f, 10, new Color(primary.R, primary.G, primary.B, (byte)(50 * key.SymmetricGlowTimer)));
                    Raylib.DrawRectangleRoundedLinesEx(rect, 0.2f, 10, 2f, glowColor);
                }

                // Text
                int fontSize = 20;
                int textWidth = Raylib.MeasureText(key.Label, fontSize);
                Raylib.DrawText(key.Label, (int)(key.Position.X + key.Width / 2 - textWidth / 2), (int)(key.Position.Y + key.Height / 2 - fontSize / 2), fontSize, primary);
            }
        }
    }
}
