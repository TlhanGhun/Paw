using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snarl;

namespace Paw
{
    class Notification
    {
        public int snarlId = 0;
        public string appName = "";
        public string className = "";
        public string gntpId = "";
        public string title = "";
        public string text = "";
        public bool sticky = false;
        public int priority = 0;
        public string icon = "";
        public string coalescingId = "";
        public string callbackContext = "";
        public string callbackContextType = "";
        public string callbackTarget = "";
        public IntPtr handleOfBelongingApp = IntPtr.Zero;

        public bool sendNotification(string defaultIcon)
        {
            if (handleOfBelongingApp != IntPtr.Zero)
            {
                SnarlConnector.ShowMessageEx(className, title, text, 10, parseIcon(defaultIcon), handleOfBelongingApp, WindowsMessage.WM_USER + 43, "");
            }
            else
            {
                SnarlConnector.ShowMessage("No class: " + this.title, this.text, 10, this.icon, IntPtr.Zero, Snarl.WindowsMessage.WM_USER + 56);
            }
            return true;
        }

        private string parseIcon(string defaultIconPath)
        {
            if (icon.ToLower().StartsWith("http"))
            {
                return icon;
            }
            return HelperFunctions.convertFileToWindowsPath(defaultIconPath);
        }
    }
}
