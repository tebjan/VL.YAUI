using Xenko.Core.Mathematics;
using System;
using VL.Lib.IO.Notifications;

namespace VL.Lib.UI
{
    public class MouseHandlerAdapter : IUIHandler
    {
        IMouseHandler FHandler;

        public MouseHandlerAdapter(IMouseHandler handler)
        {
            FHandler = handler;
        }

        public IUIHandler ProcessInput(object eventArgs)
        {
            FHandler = NotificationHelpers.NotificationSwitch(eventArgs, FHandler,
                mn => NotificationHelpers.MouseNotificationSwitch(mn, null,
                FHandler.MouseDown, FHandler.MouseMove, FHandler.MouseUp, FHandler.MouseClick, FHandler.MouseWheel));

            return FHandler != null ? this : null;
        }

        public void SetElement<T>(T element) where T : IUIElement
        {
            throw new NotImplementedException();
        }
    }

    public class MouseKeyboardHandlerAdapter : IUIHandler
    {
        IMouseKeyboardHandler FHandler;

        public MouseKeyboardHandlerAdapter(IMouseKeyboardHandler handler)
        {
            FHandler = handler;
        }

        public IUIHandler ProcessInput(object eventArgs)
        {
            FHandler = NotificationHelpers.NotificationSwitch(eventArgs, FHandler,
                mn => NotificationHelpers.MouseNotificationSwitch(mn, null,
                FHandler.MouseDown, FHandler.MouseMove, FHandler.MouseUp, FHandler.MouseClick, FHandler.MouseWheel),
                kn => NotificationHelpers.KeyNotificationSwitch(kn, null, 
                FHandler.KeyDown, FHandler.KeyUp, FHandler.KeyPress, FHandler.DeviceLost));

            return FHandler != null ? this : null;
        }

        public void SetElement<T>(T element) where T : IUIElement
        {
            throw new NotImplementedException();
        }
    }

    public class MouseKeyboardHandlerBoolAdapter : MouseKeyboardHandlerAdapter
    {
        public MouseKeyboardHandlerBoolAdapter(IMouseKeyboardHandlerBool handler)
        : base (new MouseKeyboardHandlerBoolWrapper(handler))
        { 
        }
    }

    public class MouseKeyboardHandlerBoolWrapper: IMouseKeyboardHandler
    {
        private readonly IMouseKeyboardHandlerBool FHandler;

        public MouseKeyboardHandlerBoolWrapper(IMouseKeyboardHandlerBool boolHandler)
        {
            FHandler = boolHandler;
        }

        public IMouseKeyboardHandler MouseDown(MouseDownNotification arg)
        {
            return FHandler.MouseDown(arg) ? this : null;
        }

        public IMouseKeyboardHandler MouseMove(MouseMoveNotification arg)
        {
            return FHandler.MouseMove(arg) ? this : null;
        }

        public IMouseKeyboardHandler MouseUp(MouseUpNotification arg)
        {
            return FHandler.MouseUp(arg) ? this : null;
        }

        public IMouseKeyboardHandler MouseClick(MouseClickNotification arg)
        {
            return FHandler.MouseClick(arg) ? this : null;
        }

        public IMouseKeyboardHandler MouseWheel(MouseWheelNotification arg)
        {
            return FHandler.MouseWheel(arg) ? this : null;
        }

        public IMouseKeyboardHandler KeyDown(KeyDownNotification arg)
        {
            return FHandler.KeyDown(arg) ? this : null;
        }

        public IMouseKeyboardHandler KeyUp(KeyUpNotification arg)
        {
            return FHandler.KeyUp(arg) ? this : null;
        }

        public IMouseKeyboardHandler KeyPress(KeyPressNotification arg)
        {
            return FHandler.KeyPress(arg) ? this : null;
        }

        public IMouseKeyboardHandler DeviceLost(KeyboardLostNotification arg)
        {
            return FHandler.DeviceLost(arg) ? this : null;
        }

    }
}
