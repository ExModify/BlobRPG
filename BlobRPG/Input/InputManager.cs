using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Input
{
    // OpenTK's KeyPressed function malfunctions
    // Probably I'm just unaware how to configure it properly
    public static class InputManager
    {
        static List<Keys> keysDown;
        static KeyboardState keyboardState;
        static MouseState mouseState;

        static float scroll;

        public static float X { get; private set; } = 0;
        public static float Y { get; private set; } = 0;
        public static float XDelta { get; private set; } = 0;
        public static float YDelta { get; private set; } = 0;
        public static float ScrollDelta { get; private set; } = 0;

        public static bool MouseLocked { get; private set; } = true;

        public static void Init(GameWindow window)
        {
            keysDown = new List<Keys>();
            window.CursorGrabbed = true;
        }

        public static void ToggleMouse(GameWindow window)
        {
            window.CursorGrabbed = !window.CursorGrabbed;

            if (!window.CursorGrabbed)
                window.CursorVisible = true;

            MouseLocked = !MouseLocked;
        }

        public static void Update(GameWindow window)
        {
            keyboardState = window.KeyboardState;
            mouseState = window.MouseState;

            for (int i = 0; i < keysDown.Count;)
            {
                if (!keyboardState.IsKeyDown(keysDown[i]))
                    keysDown.RemoveAt(i);
                else i++;
            }

            XDelta = mouseState.X - X;
            YDelta = mouseState.Y - Y;
            ScrollDelta = mouseState.Scroll.Y - scroll;

            X = mouseState.X;
            Y = mouseState.Y;
            scroll = mouseState.Scroll.Y;
        }
        public static bool IsKeyDown(Keys key)
        {
            bool down = keyboardState.IsKeyDown(key);

            if (down && !keysDown.Contains(key))
                keysDown.Add(key);
            return down;
        }
        public static bool IsKeyUp(Keys key)
        {
            bool down = keyboardState.IsKeyDown(key);
            return !down;
        }
        public static bool IsKeyPressed(Keys key)
        {
            bool down = keyboardState.IsKeyDown(key);
            bool wasPressed = keysDown.Contains(key);

            if (down && !wasPressed)
                keysDown.Add(key);
            return down && !wasPressed;
        }
    }
}
