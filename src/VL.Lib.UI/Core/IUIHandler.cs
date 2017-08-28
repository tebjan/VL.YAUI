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
}
