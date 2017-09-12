using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.UI.Notifications;

namespace VL.Lib.UI
{
    public interface IUIHandler
    {
        IUIHandler ProcessInput(object eventArgs);
        void SetElement<T>(T element) where T : IUIElement;
    }

    /// <summary>
    /// Mouse event handler interface
    /// </summary>
    public interface IMouseHandler : IDisposable
    {
        IMouseHandler MouseDown(MouseDownNotification arg);
        IMouseHandler MouseMove(MouseMoveNotification arg);
        IMouseHandler MouseUp(MouseUpNotification arg);
        IMouseHandler MouseClick(MouseClickNotification arg);
    }

    public interface IKeyboardHandler : IDisposable
    {
        IKeyboardHandler KeyDown(KeyDownNotification arg);
        IKeyboardHandler KeyUp(KeyUpNotification arg);
        IKeyboardHandler KeyPress(KeyPressNotification arg);
        IKeyboardHandler DeviceLost(KeyboardLostNotification arg);
    }

    public interface IMouseKeyboardHandler 
    {
        IMouseKeyboardHandler MouseDown(MouseDownNotification arg);
        IMouseKeyboardHandler MouseMove(MouseMoveNotification arg);
        IMouseKeyboardHandler MouseUp(MouseUpNotification arg);
        IMouseKeyboardHandler MouseClick(MouseClickNotification arg);

        IMouseKeyboardHandler KeyDown(KeyDownNotification arg);
        IMouseKeyboardHandler KeyUp(KeyUpNotification arg);
        IMouseKeyboardHandler KeyPress(KeyPressNotification arg);
        IMouseKeyboardHandler DeviceLost(KeyboardLostNotification arg);
    }

    public interface IMouseKeyboardHandlerBool
    {
        bool MouseDown(MouseDownNotification arg);
        bool MouseMove(MouseMoveNotification arg);
        bool MouseUp(MouseUpNotification arg);
        bool MouseClick(MouseClickNotification arg);

        bool KeyDown(KeyDownNotification arg);
        bool KeyUp(KeyUpNotification arg);
        bool KeyPress(KeyPressNotification arg);
        bool DeviceLost(KeyboardLostNotification arg);
    }
}
