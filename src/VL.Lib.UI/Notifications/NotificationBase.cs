using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Lib.UI.Notifications
{
    public abstract class NotificationBase
    {
        public readonly bool AltKey;
        public readonly bool ShiftKey;
        public readonly bool CtrlKey;

        public NotificationBase()
        {
            var key = System.Windows.Forms.Control.ModifierKeys;
            AltKey = (key & System.Windows.Forms.Keys.Alt) != 0;
            ShiftKey = (key & System.Windows.Forms.Keys.Shift) != 0;
            CtrlKey = (key & System.Windows.Forms.Keys.Control) != 0;
        }
    }
}
