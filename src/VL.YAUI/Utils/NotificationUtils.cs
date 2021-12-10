using Stride.Core.Mathematics;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VL.Lib.IO.Notifications;
using VL.YAUI.Utils;

namespace VL.YAUI
{
    public static class NotificationUtils
    {
        public static string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        public static bool ToKeyPressNotification(KeyUpNotification keyUpNotification, out KeyPressNotification keyPressNotification)
        {
            var s = KeyCodeToUnicode(keyUpNotification.KeyData);
            var success = s.Length == 1;

            var c = success ? s[0] : ' ';

            keyPressNotification = new KeyPressNotification(c, keyUpNotification.Sender);

            return success;
        }

        public static TResult MouseKeyboardSwitch<TResult>(object eventArg, TResult defaultResult,
            Func<MouseDownNotification, TResult> onMouseDown = null,
            Func<MouseMoveNotification, TResult> onMouseMove = null,
            Func<MouseUpNotification, TResult> onMouseUp = null,
            Func<MouseWheelNotification, TResult> onMouseWheel = null,
            Func<KeyDownNotification, TResult> onKeyDown = null,
            Func<KeyUpNotification, TResult> onKeyUp = null,
            Func<KeyPressNotification, TResult> onKeyPress = null)
        {
            return NotificationHelpers.NotificationSwitch(eventArg, defaultResult,
                mn => NotificationHelpers.MouseNotificationSwitch(mn, defaultResult, onMouseDown, onMouseMove, onMouseUp, null, onMouseWheel, null),
                kn => NotificationHelpers.KeyNotificationSwitch(kn, defaultResult, onKeyDown, onKeyUp, onKeyPress));
        }

        public static IUIHandler DragMouseHandler(
                Action<MouseDownNotification> onDragStart,
                Action<MouseMoveNotification, Vector2> onDrag,
                Action<MouseNotification> onDragEnd)
        {
            return new MouseHandlerAdapter(Handler.DragMouseHandler(onDragStart, onDrag, onDragEnd));
        }

        public static IUIHandler SelectionRectHandler(
                Action<MouseDownNotification> onMouseDown,
                Action<MouseDownNotification> onSelectionStart,
                Action<MouseMoveNotification, RectangleF> onSelection,
                Action<MouseNotification> onSelectionFinish)
        {
            return new MouseHandlerAdapter(Handler.SelectionRectMouseHandler(onMouseDown, onSelectionStart, onSelection, onSelectionFinish));
        }

        public static TResult PositionEvent<TResult>(object eventArg, TResult defaultResult,
                Func<Vector2, TResult> onPositionEvent = null)
                where TResult : class
        {
            return NotificationHelpers.NotificationSwitch(eventArg, defaultResult,
                mn => onPositionEvent(mn.Position),
                null,
                tn => onPositionEvent(tn.Position),
                gn => onPositionEvent(gn.Position));
        }

        static bool SafeCall<TIn, TCast>(TIn value, Action<TCast> action)
            where TCast : class
        {
            if (action != null)
            {
                var cast = value as TCast;
                if (cast != null)
                {
                    action(cast);
                    return true;
                }
            }

            return false;
        }
    }
}
