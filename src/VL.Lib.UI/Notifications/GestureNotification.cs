using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpDX;

namespace VL.Lib.UI.Notifications
{
    public enum GestureNotificationKind
    {
        GestureBegin = 1,
        GestureEnd = 2,
        GestureZoom = 3,
        GesturePan = 4,
        GestureRotate = 5,
        GestureTwoFingerTap = 6,
        GesturePressAndTap = 7
    }

    public class GestureNotification : NotificationBase
    {
        public readonly GestureNotificationKind Kind;
        public readonly Vector2 Position;
        public readonly Vector2 ClientArea;
        public readonly int Id;
        public readonly int SequenceId;
        public readonly long GestureDeviceID;
        public readonly int Flags;
        public readonly Int64 Arguments;
        public readonly int ExtraArguments;

        public GestureNotification(GestureNotificationKind kind, Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
        {
            Kind = kind;
            Position = position;
            ClientArea = clientArea;
            Id = id;
            SequenceId = sequenceId;
            GestureDeviceID = gestureDeviceID;
            Flags = flags;
            Arguments = ullArguments;
            ExtraArguments = cbExtraArgs;
        }

        public bool IsGestureBegin { get { return Kind == GestureNotificationKind.GestureBegin; } }
        public bool IsGestureEnd { get { return Kind == GestureNotificationKind.GestureEnd; } }
        public bool IsGestureZoom { get { return Kind == GestureNotificationKind.GestureZoom; } }
        public bool IsGesturePan { get { return Kind == GestureNotificationKind.GesturePan; } }
        public bool IsGestureRotate { get { return Kind == GestureNotificationKind.GestureRotate; } }
        public bool IsGestureTwoFingerTap { get { return Kind == GestureNotificationKind.GestureTwoFingerTap; } }
        public bool IsGesturePressAndTap { get { return Kind == GestureNotificationKind.GesturePressAndTap; } }
    }

    public class GestureBeginNotification : GestureNotification
    {
        public GestureBeginNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GestureBegin, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GestureEndNotification : GestureNotification
    {
        public GestureEndNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GestureEnd, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GestureZoomNotification : GestureNotification
    {
        public GestureZoomNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GestureZoom, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GesturePanNotification : GestureNotification
    {
        public GesturePanNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GesturePan, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GestureRotateNotification : GestureNotification
    {
        public GestureRotateNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GestureRotate, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GestureTwoFingerTapNotification : GestureNotification
    {
        public GestureTwoFingerTapNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GestureTwoFingerTap, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }

    public class GesturePressAndTapNotification : GestureNotification
    {
        public GesturePressAndTapNotification(Vector2 position, Vector2 clientArea, int id, int sequenceId, long gestureDeviceID, int flags, Int64 ullArguments, int cbExtraArgs)
            : base(GestureNotificationKind.GesturePressAndTap, position, clientArea, id, sequenceId, gestureDeviceID, flags, ullArguments, cbExtraArgs)
        {
        }
    }
}