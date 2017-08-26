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
        bool HitTest(Vector2 position);
        IUIHandler ProcessInput(object eventArgs);
        void Update(RectangleF bounds);

        void Enter();
        void Leave();

        void Focus();
        void Unfocus();

        void Select();
        void Deselect();

        IEnumerable<IUIElement> GetChildren();

        object GetLayer();
    }
}
