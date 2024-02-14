using Silk.NET.GLFW;
using Silk.NET.Maths;

namespace Novae.Core
{
    public static class Input
    {
        private static readonly InputAction[] _keyStates = new InputAction[(int)Keys.LastKey];
        private static readonly InputAction[] _keyLastStates = new InputAction[(int)Keys.LastKey];

        private static readonly InputAction[] _mouseStates = new InputAction[(int)MouseButton.Button8];
        private static readonly InputAction[] _mouseLastStates = new InputAction[(int)MouseButton.Button8];

        public static Vector2D<int> MousePosition { get; internal set; } = new Vector2D<int>();
        public static Vector2D<int> MouseScroll { get; internal set; } = new Vector2D<int>();

        internal static void SetKeyPressed(Keys key)
        {
            if (_keyStates[(int)key] == InputAction.Press || _keyStates[(int)key] == InputAction.Repeat)
                return;

            _keyStates[(int)key] = InputAction.Press;
        }

        internal static void SetKeyReleased(Keys key)
        {
            _keyStates[(int)key] = InputAction.Release;
        }

        internal static void SetKeyState(Keys key, InputAction action)
        {
            if (key == Keys.Unknown)
                return;

            switch (action)
            {
                case InputAction.Press:
                {
                    SetKeyPressed(key);
                    break;
                }

                case InputAction.Release:
                {
                    SetKeyReleased(key);
                    break;
                }

                case InputAction.Repeat:
                {
                    SetKeyPressed(key);
                    break;
                }

                default: break;
            }
        }

        internal static void SetMousePressed(MouseButton button)
        {
            if (_mouseStates[(int)button] == InputAction.Press || _mouseStates[(int)button] == InputAction.Repeat)
                return;

            _mouseStates[(int)button] = InputAction.Press;
        }

        internal static void SetMouseReleased(MouseButton button)
        {
            _mouseStates[(int)button] = InputAction.Release;
        }
        internal static void SetMouseState(MouseButton button, InputAction action)
        {
            switch (action)
            {
                case InputAction.Press:
                {
                    SetMousePressed(button);
                    break;
                }

                case InputAction.Release:
                {
                    SetMouseReleased(button);
                    break;
                }

                case InputAction.Repeat:
                {
                    SetMousePressed(button);
                    break;
                }

                default: break;
            }
        }

        internal static void Poll()
        {
            for (int i = 0; i < (int)Keys.LastKey; i++)
            {
                if (_keyStates[i] == InputAction.Press && _keyLastStates[i] == InputAction.Press)
                {
                    _keyStates[i] = InputAction.Repeat;
                }

                _keyLastStates[i] = _keyStates[i];
            }

            for (int i = 0; i < (int)MouseButton.Button8; i++)
            {
                if (_mouseStates[i] == InputAction.Press && _mouseLastStates[i] == InputAction.Press)
                {
                    _mouseStates[i] = InputAction.Repeat;
                }

                _mouseLastStates[i] = _mouseStates[i];
            }
        }

        public static bool IsKeyDown(Keys key)
        {
            return _keyStates[(int)key] == InputAction.Press || _keyStates[(int)key] == InputAction.Repeat;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return _keyStates[(int)key] == InputAction.Press;
        }

        public static bool IsKeyUp(Keys key)
        {
            return _keyStates[(int)key] == InputAction.Release && (_keyLastStates[(int)key] == InputAction.Press || _keyLastStates[(int)key] == InputAction.Repeat);
        }

        public static bool IsMouseDown(MouseButton button)
        {
            return _mouseStates[(int)button] == InputAction.Press || _mouseStates[(int)button] == InputAction.Repeat;
        }

        public static bool IsMousePressed(MouseButton button)
        {
            return _mouseStates[(int)button] == InputAction.Press;
        }

        public static bool IsMouseUp(MouseButton button)
        {
            return _mouseStates[(int)button] == InputAction.Release && (_mouseLastStates[(int)button] == InputAction.Press || _mouseLastStates[(int)button] == InputAction.Repeat);
        }
    }
}
