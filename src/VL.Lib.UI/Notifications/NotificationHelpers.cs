using SharpDX;
using System;
using VL.Lib.UI.Notifications;

namespace VL.Lib.UI
{
    public static class NotificationHelpers
    {
        public static TResult MouseKeyboardSwitch<TResult>(object eventArg, TResult defaultResult,
            Func<MouseDownNotification, TResult> onMouseDown = null,
            Func<MouseMoveNotification, TResult> onMouseMove = null,
            Func<MouseUpNotification, TResult> onMouseUp = null,
            Func<KeyDownNotification, TResult> onKeyDown = null,
            Func<KeyUpNotification, TResult> onKeyUp = null,
            Func<KeyPressNotification, TResult> onKeyPress = null)
            where TResult : class
        {
            return NotificationSwitch(eventArg, defaultResult,
                mn => MouseNotificationSwitch(mn, defaultResult, onMouseDown, onMouseMove, onMouseUp),
                kn => KeyNotificationSwitch(kn, defaultResult, onKeyDown, onKeyUp, onKeyPress));
        }

        public static TResult PositionEvent<TResult>(object eventArg, TResult defaultResult,
            Func<Vector2, TResult> onPositionEvent = null)
            where TResult : class
        {
            return NotificationSwitch(eventArg, defaultResult,
                mn => onPositionEvent(mn.Position),
                null,
                tn => onPositionEvent(tn.Position),
                gn => onPositionEvent(gn.Position));
        }

        public static TResult NotificationSwitch<TResult>(object eventArg, TResult defaultResult, 
            Func<MouseNotification, TResult> mouseFunc = null,
            Func<KeyNotification, TResult> keyFunc = null,
            Func<TouchNotification, TResult> touchFunc = null,
            Func<GestureNotification, TResult> gestureFunc = null)
            where TResult : class
        {
            if (mouseFunc != null)
            {
                var mouseNotification = eventArg as MouseNotification;
                if (mouseNotification != null)
                {
                    return mouseFunc(mouseNotification);
                } 
            }

            if (touchFunc != null)
            {
                var touchNotification = eventArg as TouchNotification;
                if (touchNotification != null)
                {
                    return touchFunc(touchNotification);
                } 
            }

            if (keyFunc != null)
            {
                var keyNotification = eventArg as KeyNotification;
                if (keyNotification != null)
                {
                    return keyFunc(keyNotification);
                } 
            }

            if (gestureFunc != null)
            {
                var gestureNotification = eventArg as GestureNotification;
                if (gestureNotification != null)
                {
                    return gestureFunc(gestureNotification);
                } 
            }

            return defaultResult;
        }

        public static TResult MouseNotificationSwitch<TResult>(MouseNotification notification, TResult defaultResult,
            Func<MouseDownNotification, TResult> onDown = null,
            Func<MouseMoveNotification, TResult> onMove = null,
            Func<MouseUpNotification, TResult> onUp = null,
            Func<MouseClickNotification, TResult> onClick = null,
            Func<MouseWheelNotification, TResult> onWheel = null,
            Func<MouseHorizontalWheelNotification, TResult> onHorizontalWheel = null)
            where TResult : class
        {
            switch (notification.Kind)
            {
                case MouseNotificationKind.MouseDown:
                    return onDown?.Invoke((MouseDownNotification)notification) ?? defaultResult;
                case MouseNotificationKind.MouseUp:
                    return onUp?.Invoke((MouseUpNotification)notification) ?? defaultResult;
                case MouseNotificationKind.MouseMove:
                    return onMove?.Invoke((MouseMoveNotification)notification) ?? defaultResult;
                case MouseNotificationKind.MouseWheel:
                    return onWheel?.Invoke((MouseWheelNotification)notification) ?? defaultResult;
                case MouseNotificationKind.MouseHorizontalWheel:
                    return onHorizontalWheel?.Invoke((MouseHorizontalWheelNotification)notification) ?? defaultResult;
                case MouseNotificationKind.MouseClick:
                    return onClick?.Invoke((MouseClickNotification)notification) ?? defaultResult;
                default:
                    return defaultResult;
            }
        }

        public static TResult KeyNotificationSwitch<TResult>(KeyNotification notification, TResult defaultResult,
            Func<KeyDownNotification, TResult> onDown = null,
            Func<KeyUpNotification, TResult> onUp = null,
            Func<KeyPressNotification, TResult> onPress = null,
            Func<KeyboardLostNotification, TResult> onDeviceLost = null)
            where TResult : class
        {
            switch (notification.Kind)
            {
                case KeyNotificationKind.KeyDown:
                    return onDown?.Invoke((KeyDownNotification)notification) ?? defaultResult;
                case KeyNotificationKind.KeyPress:
                    return onPress?.Invoke((KeyPressNotification)notification) ?? defaultResult;
                case KeyNotificationKind.KeyUp:
                    return onUp?.Invoke((KeyUpNotification)notification) ?? defaultResult;
                case KeyNotificationKind.DeviceLost:
                    return onDeviceLost?.Invoke((KeyboardLostNotification)notification) ?? defaultResult;
                default:
                    return defaultResult;
            }
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
