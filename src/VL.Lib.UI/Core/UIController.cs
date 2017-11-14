using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.UI.Notifications;
using VL.Lib.UI.Utils;
using VVVV.Utils.IO;

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

        IUIHandler FFallbackHandler;
        IUIHandler FDefaultHandler;
        IUIHandler FCurrentHandler;

        RectangleF? FSelectionRect;
        public RectangleF? GetSelectionRect() => FSelectionRect;

        public UIController()
        {
            FFallbackHandler = new BasicUIHandler(this);
            FDefaultHandler = FFallbackHandler;           
        }  

        public void AddElement(IUIElement element)
        {
            FElements.Add(element);
        }

        public bool RemoveElement(IUIElement element)
        {
            return FElements.Remove(element);
        }

        public void SetElements(IEnumerable<IUIElement> elements)
        {
            if(elements != null && elements.Any())
                FElements = elements.ToList();
        }

        public void ClearElements()
        {
            FElements.Clear();
        }

        public void OverrideDefaultHandler(IUIHandler defaultInteraction)
        {
            FDefaultHandler = defaultInteraction;
        }

        public void ResetDefaultHandler()
        {
            FDefaultHandler = FFallbackHandler;
        }

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

        IUIHandler SetupSelectionHandler()
        {
            return NotificationUtils.SelectionRectHandler(
                OnSelectionDown,
                null,
                OnSelection,
                OnEndSelection
                );
        }

        void OnEndSelection(MouseNotification mn)
        {
            FSelectionRect = null;
        }

        bool CtrlKey;

        public void ProcessInput(object eventArgs)
        {
            var kd = eventArgs as KeyDownNotification;
            if (kd?.KeyCode == System.Windows.Forms.Keys.Control)
                CtrlKey = true;

            var ku = eventArgs as KeyUpNotification;
            if (ku?.KeyCode == System.Windows.Forms.Keys.Control)
                CtrlKey = false;

            if (FCurrentHandler == null)
            {
                //get pick path 
                FPickPath = NotificationUtils.PositionEvent(eventArgs, FPickPath, pos => GetPickPath(pos.GetUnitRect()));

                //calc hover, active, selected
                FCurrentHandler = NotificationUtils.MouseKeyboardSwitch(eventArgs, FCurrentHandler, OnMouseDown, OnMouseMove);
            }
            else
            {
                //calc unhover
                //NotificationHelpers.MouseKeyboardSwitch(eventArgs, null, null, OnMouseMoveUnhover, null);
            }

            //do handler
            FCurrentHandler = FCurrentHandler?.ProcessInput(eventArgs);

            //System.Diagnostics.Debug.WriteLine("Notification: " + eventArgs?.ToString());
            //System.Diagnostics.Debug.WriteLine("FCurrentHandler: " + FCurrentHandler?.ToString());
        }

        void OnSelectionDown(MouseDownNotification mn)
        {
            //selection
            if (!CtrlKey)
            {
                foreach (var elem in FSelectedElements)
                    elem.Deselect();

                FSelectedElements.Clear();
            }

            var last = FPickPath.LastOrDefault();

            if (last != null)
            {
                if (!last.GetSelected())
                {
                    FSelectedElements.Add(last);
                    last.Select();
                }
                else
                {
                    last.Deselect();
                    FSelectedElements.Remove(last);
                }
            }
        }

        void OnSelection(MouseMoveNotification mn, RectangleF selection)
        {
            FSelectionRect = selection;
            var leftovers = FSelectedElements.ToList();
            var picks = GetPickPath(selection);

            foreach (var pick in picks)
            {
                if (!FSelectedElements.Contains(pick))
                {
                    FSelectedElements.Add(pick);
                    pick.Select();                   
                }

                leftovers.Remove(pick);
            }

            if (!CtrlKey)
            {
                foreach (var item in leftovers)
                {
                    item.Deselect();
                    FSelectedElements.Remove(item);
                } 
            }
        }

        IUIHandler OnMouseDown(MouseDownNotification mn)
        {
            if (FPickPath.Any())
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

                OnSelectionDown(mn);

                return FDefaultHandler;
            }
            else //marquee selection when no element hit
            {
                return SetupSelectionHandler();
            }
        }

        IUIHandler OnMouseMove(MouseMoveNotification mn)
        {
            CalculateEnterLeaveEvent(mn, FPickPath);

            foreach (var item in FHoveredViews)
            {
                item.Hover(mn);
            }

            return null;
        }

        IUIHandler OnMouseMoveUnhover(MouseMoveNotification mn)
        {
            CalculateEnterLeaveEvent(mn, FHoveredViews.Where(elem => elem.HitTest(mn.Position.ToVector2().GetUnitRect())).ToList());
            return null;
        }

        IReadOnlyList<IUIElement> GetPickPath(RectangleF pickArea)
        {
            var pickPath = new List<IUIElement>();
            GetPickPathRecursive(pickArea, FElements, pickPath);
            return pickPath;
        }

        static void GetPickPathRecursive(RectangleF pickArea, IEnumerable<IUIElement> elements, List<IUIElement> pickPath)
        {
            foreach (var element in elements)
            {
                if(element.HitTest(pickArea))
                {
                    pickPath.Add(element);
                    GetPickPathRecursive(pickArea, element.GetChildren(), pickPath);
                }
            }
        }

        List<IUIElement> FEnteredViews = new List<IUIElement>();
        List<IUIElement> FLeftViews = new List<IUIElement>();

        IEnumerable<T> LastOrDefaultAsSequence<T>(IEnumerable<T> input)
        {
            yield return input.LastOrDefault();
        }

        void CalculateEnterLeaveEvent(MouseMoveNotification arg, IReadOnlyList<IUIElement> pickedViews)
        {
            FEnteredViews.Clear();
            FLeftViews = FHoveredViews.ToList();
            FHoveredViews.Clear();

            if (pickedViews.Any())
            {
                //add new views
                foreach (var view in LastOrDefaultAsSequence(pickedViews))
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
