using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Lib.UI.Utils
{
    public static class PointSizeAndVector2Utils
    {
        public static Vector2 ToVector2(this System.Drawing.Point p) => new Vector2(p.X, p.Y);
    }
}
