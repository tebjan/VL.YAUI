using System;
using SharpDX;

namespace VL.Lib.UI
{
    public static class RectangleExtensions
    {
        /// <summary>
        /// Gets a unit rect from a center position.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <returns></returns>
        public static RectangleF GetUnitRect(this Vector2 center)
        {
            return GetRect(center, Vector2.One);
        }

        /// <summary>
        /// Gets a rect with from given center and size
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static RectangleF GetRect(this Vector2 center, Vector2 size)
        {
            return new RectangleF(center.X - size.X * 0.5f, center.Y - size.Y * 0.5f, size.X, size.Y);
        }

        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF() { Left = left, Right = right, Top = top, Bottom = bottom };
        }

        public static void GetNullableValue<T>(this T? input, T defaultValue, out bool success, out T value)
            where T : struct
        {
            success = input.HasValue;
            value = success ? input.Value : defaultValue;
        }
    }
}
