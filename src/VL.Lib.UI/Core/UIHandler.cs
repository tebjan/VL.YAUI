using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.IO.Notifications;

namespace VL.Lib.UI
{
    public class BasicUIHandler : IUIHandler
    {
        readonly UIController FController;

        public BasicUIHandler(UIController controller)
        {
            FController = controller;
        }

        public void SetElement<T>(T element) where T : IUIElement
        {
            throw new NotImplementedException();
        }

        public IUIHandler ProcessInput(object eventArgs)
        {
            var active = NotificationUtils.MouseKeyboardSwitch(eventArgs, this, OnMouseDown, OnMouseMove, OnMouseUp);

            if (ActiveElement != null)
                return ActiveElement.ProcessInput(eventArgs) ?? active;

            return active;
        }

        IUIElement ActiveElement;

        IUIHandler OnMouseDown(MouseDownNotification mn)
        {
            var activeLast = FController.GetFocusedElements().LastOrDefault();
            if (activeLast != null)
            {
                ActiveElement = activeLast;
            }

            return this;
        }

        IUIHandler OnMouseMove(MouseMoveNotification mn)
        {
            return this;
        }

        IUIHandler OnMouseUp(MouseUpNotification mn)
        {
            return null;
        }
    }
}
