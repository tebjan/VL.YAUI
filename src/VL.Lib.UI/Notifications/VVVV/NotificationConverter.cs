using SharpDX;
using System;
using System.Windows.Forms;
using VL.Lib.UI.VVVV;
using VUtils = VVVV.Utils.IO;
using Drawing = System.Drawing;



namespace VL.Lib.UI.Notifications
{
    public class NotificationSpaceTransformer
    {
        readonly Vector2 Offset;
        readonly Vector2 Scaling;

        public NotificationSpaceTransformer(Vector2 offset, Vector2 scaling)
        {
            Offset = offset;
            Scaling = scaling;
        }

        public Vector2 Transform(Drawing.Point point)
        {
            return new Vector2(point.X * Scaling.X + Offset.X, point.Y * Scaling.Y + Offset.Y);
        }

        public Vector2 Transform(Drawing.Size size)
        {
            return new Vector2(size.Width * Scaling.X, size.Height * Scaling.Y);
        }
    }

    public static class NotificationConverter
    {

        public static object ConvertNotification(object notification, NotificationSpaceTransformer transformer)
        {
            return VVVVNotificationHelpers.NotificationSwitch(notification, null,
                
                //mouse
                mn => VVVVNotificationHelpers.MouseNotificationSwitch<object>(mn, null,

                    n => new MouseDownNotification(
                        transformer.Transform(n.Position),
                        transformer.Transform(n.ClientArea),
                        n.Buttons),

                    n => new MouseMoveNotification(
                        transformer.Transform(n.Position),
                        transformer.Transform(n.ClientArea)),

                    n => new MouseUpNotification(
                        transformer.Transform(n.Position),
                        transformer.Transform(n.ClientArea),
                        n.Buttons)
                ),

                //keyboard
                kn => VVVVNotificationHelpers.KeyNotificationSwitch<object>(kn, null,
                    
                    n => new KeyDownNotification(n.KeyCode),
                    n => new KeyUpNotification(n.KeyCode),
                    n => new KeyPressNotification(n.KeyChar)

                ),
                null, //touch
                null); //gesture
        }
    }
}
