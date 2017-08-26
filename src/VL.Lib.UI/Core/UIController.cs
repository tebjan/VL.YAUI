using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.UI.Notifications;

namespace VL.Lib.UI
{
    public class UIController
    {
        List<IUIElement> FElements = new List<IUIElement>();
        HashSet<IUIElement> FHoveredViews = new HashSet<IUIElement>();
        List<IUIElement> FFocusedElements = new List<IUIElement>();
        List<IUIElement> FSelectedElements = new List<IUIElement>();

        public IReadOnlyList<IUIElement> GetElements() => FElements;

        public void AddElement(IUIElement element)
        {
            FElements.Add(element);
            FFallbackHandler = new BasicUIHandler(this);
        }

        public bool RemoveElement(IUIElement element)
        {
            return FElements.Remove(element);
        }

        IUIHandler FFallbackHandler;
        IUIHandler FCurrentHandler;

        IReadOnlyList<IUIElement> FPickPath = new List<IUIElement>();
        public IReadOnlyList<IUIElement> GetPickPath() => FPickPath;


        public void ProcessInput(object eventArgs)
        {
            //get pick path 
            FPickPath = NotificationHelpers.PositionEvent(eventArgs, FPickPath, GetPickPath);

            //calc hover, active, selected
            NotificationHelpers.MouseKeyboardSwitch(eventArgs, this, OnMouseDown, OnMouseMove, OnMouseUp);

            //do handler
            FCurrentHandler = FCurrentHandler?.ProcessInput(eventArgs) ?? FFallbackHandler.ProcessInput(eventArgs);
        }

        object OnMouseDown(MouseDownNotification mn)
        {
            //focus
            foreach (var elem in FFocusedElements)
                elem.Unfocus();        

            FFocusedElements.Clear();           

            var first = FPickPath.FirstOrDefault();

            if (first != null)
            {
                FFocusedElements.Add(first);
                first.Focus();
            }

            //selection
            if (!mn.CtrlKey)
            {
                foreach (var elem in FSelectedElements)
                    elem.Deselect();

                FSelectedElements.Clear();
            }

            if (first != null)
            {
                FSelectedElements.Add(first);
                first.Select();
            }

            return null;
        }

        object OnMouseMove(MouseMoveNotification mn)
        {
            CalculateEnterLeaveEvent(mn);

            return null;
        }

        object OnMouseUp(MouseUpNotification mn)
        {
            return null;
        }

        IReadOnlyList<IUIElement> GetPickPath(Vector2 position)
        {
            var pickPath = new List<IUIElement>();
            GetPickPathRecursive(position, FElements, pickPath);
            return pickPath;
        }

        static void GetPickPathRecursive(Vector2 position, IEnumerable<IUIElement> elements, List<IUIElement> pickPath)
        {
            foreach (var element in elements)
            {
                if(element.HitTest(position))
                {
                    GetPickPathRecursive(position, element.GetChildren(), pickPath);
                    pickPath.Add(element);
                }
            }
        }

        List<IUIElement> FEnteredViews = new List<IUIElement>();
        List<IUIElement> FLeftViews = new List<IUIElement>();

        void CalculateEnterLeaveEvent(MouseMoveNotification arg)
        {
            var pickedViews = FPickPath;

            FEnteredViews.Clear();
            FLeftViews = FHoveredViews.ToList();
            FHoveredViews.Clear();

            if (pickedViews.Any())
            {
                //add new views
                foreach (var view in pickedViews)
                {
                    FHoveredViews.Add(view);

                    if (!FLeftViews.Remove(view))
                    {
                        FEnteredViews.Add(view);
                    }
                }

                //remove old views
                foreach (var view in FLeftViews)
                {
                    FHoveredViews.Remove(view);
                }

                ViewsEnteredOrLeft(FEnteredViews, FLeftViews);
            }
            else
            {
                FHoveredViews.Clear();
                ViewsEnteredOrLeft(FEnteredViews, FLeftViews);
            }
        }

        void ViewsEnteredOrLeft(IEnumerable<IUIElement> enteredViews, IEnumerable<IUIElement> leftViews)
        {
            foreach (var elem in leftViews)
            {
                elem.Leave();
            }

            foreach (var elem in enteredViews)
            {
                elem.Enter();
            }
        }
    }

    public class BasicUIHandler : IUIHandler
    {
        readonly UIController FController;

        public BasicUIHandler(UIController controller)
        {
            FController = controller;
        }

        public IUIHandler ProcessInput(object eventArgs)
        {
            return NotificationHelpers.MouseKeyboardSwitch(eventArgs, this, OnMouseDown, OnMouseMove, OnMouseUp);
        }

        IUIHandler OnMouseDown(MouseDownNotification mn)
        {
            var picks = FController.GetPickPath();
            return this;
        }

        IUIHandler OnMouseMove(MouseMoveNotification mn)
        {
            return this;
        }

        IUIHandler OnMouseUp(MouseUpNotification mn)
        {
            return this;
        }
    }
}
