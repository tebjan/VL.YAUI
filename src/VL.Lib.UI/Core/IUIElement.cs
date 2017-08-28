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

        void Enter();
        void Leave();

        void Focus();
        void Unfocus();

        void Select();
        void Deselect();

        IUIHandler ProcessInput(object eventArgs);
        void Update();
       
        void SetVisibility(bool visible);
        bool GetVisibility();

        void SetBounds(RectangleF bounds);
        RectangleF GetBounds();

        void AddElement(IUIElement newElement);
        bool RemoveElement(IUIElement oldElement);
        IEnumerable<IUIElement> GetChildren();

        object GetLayer();
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
