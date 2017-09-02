using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace VL.Lib.UI
{
    public interface IUIElement
    {

        void Layout(Vector2 offset);
        bool HitTest(RectangleF hitArea);

        void SetHovered(bool hovered);
        bool GetHovered();
        void Hover(object mouseMoveArgs);

        void SetFocused(bool focused);
        bool GetFocused();

        void SetSelected(bool selected);
        bool GetSelected();

        //interaction
        IUIHandler ProcessInput(object eventArgs);  

        //visuals
        void SetBounds(RectangleF bounds);
        RectangleF GetBounds();

        void SetVisibility(bool visible);
        bool GetVisibility();

        //void SetDirty(bool dirty);
        //bool GetDirty();

        void Update();

        //children
        IEnumerable<IUIElement> GetChildren();

        object GetLayer();
    }

    public interface IUIElementSetChildren : IUIElement
    {
        void SetChildren(IEnumerable<IUIElement> children);
    }

    public static class IUElementExtensions
    {
        public static IUIElement SetPosition(this IUIElement element, Vector2 newPosition)
        {
            var oldBounds = element.GetBounds();
            element.SetBounds(new RectangleF(newPosition.X, newPosition.Y, oldBounds.Width, oldBounds.Height));
            return element;
        }

        public static IUIElement SetSize(this IUIElement element, Vector2 newSize)
        {
            var oldBounds = element.GetBounds();
            element.SetBounds(new RectangleF(oldBounds.X, oldBounds.Y, newSize.X, newSize.Y));
            return element;
        }

        public static IUIElement Enter(this IUIElement element)
        {
            element.SetHovered(true);
            return element;
        }

        public static IUIElement Leave(this IUIElement element)
        {
            element.SetHovered(false);
            return element;
        }

        public static IUIElement Focus(this IUIElement element)
        {
            element.SetFocused(true);
            return element;
        }

        public static IUIElement Unfocus(this IUIElement element)
        {
            element.SetFocused(false);
            return element;
        }

        public static IUIElement Select(this IUIElement element)
        {
            element.SetSelected(true);
            return element;
        }

        public static IUIElement Deselect(this IUIElement element)
        {
            element.SetSelected(false);
            return element;
        }

        public static IUIElement Show(this IUIElement element)
        {
            element.SetVisibility(true);
            return element;
        }

        public static IUIElement Hide(this IUIElement element)
        {
            element.SetVisibility(false);
            return element;
        }
    }
}
