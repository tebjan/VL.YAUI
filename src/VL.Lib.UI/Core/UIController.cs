using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.IO.Notifications;
using VL.Lib.UI.Utils;

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
        public ImmutableDictionary<string, IUIElement> GetAllValues() => FAllUIElements;

        IUIHandler FFallbackHandler;
        IUIHandler FDefaultHandler;
        IUIHandler FCurrentHandler;

        RectangleF? FSelectionRect;
        public RectangleF? GetSelectionRect() => FSelectionRect;
        readonly bool FIsUIRoot;

        public UIController(bool isUIRoot)
        {
            FIsUIRoot = isUIRoot;
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
                //if(elem.GetDirty()) //wip
                    elem.Update();
            }

            if (FIsUIRoot)
            {
                FAllUIElementsBuilder.Clear();
                DoRecursive(FElements, elem => GetValue(elem, FAllUIElementsBuilder));
                FAllUIElements = FAllUIElementsBuilder.ToImmutable();
            }
        }

        ImmutableDictionary<string, IUIElement>.Builder FAllUIElementsBuilder = ImmutableDictionary.CreateBuilder<string, IUIElement>();
        ImmutableDictionary<string, IUIElement> FAllUIElements = ImmutableDictionary<string, IUIElement>.Empty;
        void GetValue(IUIElement elem, ImmutableDictionary<string, IUIElement>.Builder builder)
        {
            builder[elem.GetId()] = elem;
        }

        static void DoRecursive(IEnumerable<IUIElement> elements, Action<IUIElement> forEach)
        {
            foreach(var elem in elements)
            {
                forEach(elem);
            }

            foreach (var elem in elements)
            {
                DoRecursive(elem.GetChildren(), forEach);
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

        public void ProcessInput(object eventArgs)
        {
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
            if (mn.CtrlKey)
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

            if (!mn.CtrlKey)
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
            CalculateEnterLeaveEvent(mn, FHoveredViews.Where(elem => elem.HitTest(mn.Position.GetUnitRect())).ToList());
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

        //inform ui elements about enter or leave
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
}
