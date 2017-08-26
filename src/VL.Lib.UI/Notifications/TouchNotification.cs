using System;
using SharpDX;

namespace VL.Lib.UI.Notifications
{
    public enum TouchNotificationKind
    {
        TouchDown,
        TouchUp,
        TouchMove
    }

    public class TouchNotification : NotificationBase
    {
        public readonly TouchNotificationKind Kind;
        public readonly Vector2 Position;
        public readonly Vector2 ClientArea;
        public readonly int Id;
        public readonly bool Primary;
        public readonly Vector2 ContactArea;
        public readonly long TouchDeviceID;

        public TouchNotification(TouchNotificationKind kind, Vector2 position, Vector2 clientArea, int id, bool primary, Vector2 contactArea, long touchDeviceID)
        {
            Kind = kind;
            Position = position;
            ClientArea = clientArea;
            Id = id;
            Primary = primary;
            ContactArea = contactArea;
            TouchDeviceID = touchDeviceID;
        }
        
        public bool IsTouchDown { get { return Kind == TouchNotificationKind.TouchDown; } }
        public bool IsTouchUp { get { return Kind == TouchNotificationKind.TouchUp; } }
        public bool IsTouchMove { get { return Kind == TouchNotificationKind.TouchMove; } }
    }

    public class TouchDownNotification : TouchNotification
    {
        public TouchDownNotification(Vector2 position, Vector2 clientArea, int id, bool primary, Vector2 contactArea, long touchDeviceID)
            : base(TouchNotificationKind.TouchDown, position, clientArea, id, primary, contactArea, touchDeviceID)
        {
        }
    }

    public class TouchMoveNotification : TouchNotification
    {
        public TouchMoveNotification(Vector2 position, Vector2 clientArea, int id, bool primary, Vector2 contactArea, long touchDeviceID)
            : base(TouchNotificationKind.TouchMove, position, clientArea, id, primary, contactArea, touchDeviceID)
        {
        }
    }

    public class TouchUpNotification : TouchNotification
    {
        public TouchUpNotification(Vector2 position, Vector2 clientArea, int id, bool primary, Vector2 contactArea, long touchDeviceID)
            : base(TouchNotificationKind.TouchUp, position, clientArea, id, primary, contactArea, touchDeviceID)
        {
        }
    }
}