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
        public IReadOnlyList<IUIElement> GetFocusedElements() => FFocusedElements;
        public IReadOnlyList<IUIElement> GetSelectedElements() => FSelectedElements;

        public UIController()
        {
            FFallbackHandler = new BasicUIHandler(this);
        }  

        public void AddElement(IUIElement element)
        {
            FElements.Add(element);
        }

        public bool RemoveElement(IUIElement element)
        {
            return FElements.Remove(element);
        }

        IUIHandler FFallbackHandler;
        IUIHandler FCurrentHandler;

        IReadOnlyList<IUIElement> FPickPath = new List<IUIElement>();
        public IReadOnlyList<IUIElement> GetPickPath() => FPickPath;

        Vector2 FOffset;
        public void Layout(Vector2 offset)
        {
            FOffset = offset;
            foreach (var elem in FElements)
            {
                elem.Layout(offset);
            }
        } 

        public void Update()
        {
            foreach (var elem in FElements)
            {
                elem.Update();
            }
        }

        public void ProcessInput(object eventArgs)
        {
            if (FCurrentHandler == null)
            {
                //get pick path 
                FPickPath = NotificationHelpers.PositionEvent(eventArgs, FPickPath, GetPickPath);

                //calc hover, active, selected
                FCurrentHandler = NotificationHelpers.MouseKeyboardSwitch(eventArgs, null, OnMouseDown, OnMouseMove, OnMouseUp);
            }
            else
            {
                //calc unhover
                NotificationHelpers.MouseKeyboardSwitch(eventArgs, null, null, OnMouseMoveUnhover, null);
            }

            //do handler
            FCurrentHandler = FCurrentHandler?.ProcessInput(eventArgs);
        }

        IUIHandler OnMouseDown(MouseDownNotification mn)
        {
            //focus
            foreach (var elem in FFocusedElements)
                elem.Unfocus();        

            FFocusedElements.Clear();           

            var last = FPickPath.LastOrDefault();

            if (last != null)
            {
                FFocusedElements.Add(last);
                last.Focus();
            }

            //selection
            if (!mn.CtrlKey)
            {
                foreach (var elem in FSelectedElements)
                    elem.Deselect();

                FSelectedElements.Clear();
            }

            if (last != null)
            {
                FSelectedElements.Add(last);
                last.Select();
            }

            return FFallbackHandler;
        }

        IUIHandler OnMouseMove(MouseMoveNotification mn)
        {
            CalculateEnterLeaveEvent(mn, FPickPath);
            return null;
        }

        IUIHandler OnMouseMoveUnhover(MouseMoveNotification mn)
        {
            CalculateEnterLeaveEvent(mn, FHoveredViews.Where(elem => elem.HitTest(mn.Position)).ToList());
            return null;
        }

        IUIHandler OnMouseUp(MouseUpNotification mn)
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
                    pickPath.Add(element);
                    GetPickPathRecursive(position, element.GetChildren(), pickPath);
                }
            }
        }

        List<IUIElement> FEnteredViews = new List<IUIElement>();
        List<IUIElement> FLeftViews = new List<IUIElement>();

        void CalculateEnterLeaveEvent(MouseMoveNotification arg, IReadOnlyList<IUIElement> pickedViews)
        {
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
            return NotificationHelpers.MouseKeyboardSwitch(eventArgs, null, OnMouseDown, OnMouseMove, OnMouseUp);
        }

        IUIElement ActiveElement;

        IUIHandler OnMouseDown(MouseDownNotification mn)
        {
            var activeLast = FController.GetFocusedElements().LastOrDefault();
            if (activeLast != null)
            {
                ActiveElement = activeLast;
                return ActiveElement.ProcessInput(mn) ?? this;
            }

            return null;
        }

        IUIHandler OnMouseMove(MouseMoveNotification mn)
        {
            if (ActiveElement != null)
                return ActiveElement.ProcessInput(mn) ?? this;

            return null;
        }

        IUIHandler OnMouseUp(MouseUpNotification mn)
        {
            return ActiveElement?.ProcessInput(mn) ?? null;
        }
    }
}
