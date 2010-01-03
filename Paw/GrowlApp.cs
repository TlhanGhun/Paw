using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snarl;
using System.Threading;


namespace Paw
{
    class GrowlApp
    {
        public string appName = "";
        public string iconPath = "";
        public int numberOfClasses = 0;
        public List<NotificationClass> notificationClasses = new List<NotificationClass>();
        public IntPtr commWindowHandle = IntPtr.Zero;
        private SnarlCommunicationWindow commWindow = null;

        public void createCommWindow()
        {    
            commWindow = new SnarlCommunicationWindow();      
            commWindowHandle = commWindow.Handle;
            
        }

        public void bringCommWindowToBackground()
        {
            if (commWindow != null)
            {
                System.Windows.Forms.Application.Run(commWindow);
            }
        }

        public override bool Equals(object obj)
        {
            return ((GrowlApp)obj).appName == this.appName;
        }

        ~GrowlApp() {
            if (commWindow != null)
            {
                SnarlConnector.RevokeConfig(this.commWindowHandle);
                commWindow.Close();
            }
        }


        public string parseIcon(string iconPath)
        {
            if(iconPath.ToLower().StartsWith("http")) {
                return iconPath;
            }
            return HelperFunctions.getPawDefaultIconPath();
        }
    }
}
