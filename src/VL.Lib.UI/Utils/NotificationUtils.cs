using SharpDX;
using System;
using VL.Lib.IO.Notifications;
using VL.Lib.UI.Notifications;
using VL.Lib.UI.Utils;

namespace VL.Lib.UI
{
    public static class NotificationUtils
    {
        public static TResult MouseKeyboardSwitch<TResult>(object eventArg, TResult defaultResult,
            Func<MouseDownNotification, TResult> onMouseDown = null,
            Func<MouseMoveNotification, TResult> onMouseMove = null,
            Func<MouseUpNotification, TResult> onMouseUp = null,
            Func<KeyDownNotification, TResult> onKeyDown = null,
            Func<KeyUpNotification, TResult> onKeyUp = null,
            Func<KeyPressNotification, TResult> onKeyPress = null)
        {
            return NotificationHelpers.NotificationSwitch(eventArg, defaultResult,
                mn => NotificationHelpers.MouseNotificationSwitch<TResult>(mn, defaultResult, onMouseDown, onMouseMove, onMouseUp, null, null, null),
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
