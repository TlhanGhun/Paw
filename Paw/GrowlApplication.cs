using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Snarl;
using Growl.Daemon;
using Growl.Connector;
using Growl.CoreLibrary;
using Growl;

namespace Paw
{

    // Summary description for SnarlMsgWnd.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    
    
    public class GrowlApplication : NativeWindow
    {
        CreateParams cp = new CreateParams();

        public string pathToIcon { get; set; }
        public List<AlertClass> alertClasses { get; set; }
        public IntPtr windowHandle;
        public Growl.Connector.Application application;
        public Dictionary<string, string> alertClassIcons;

        int SNARL_GLOBAL_MESSAGE;

        public GrowlApplication(Growl.Connector.Application app, List<NotificationType> notiTypes)
        {
            // Create the actual window
            windowHandle = IntPtr.Zero;
            alertClasses = new List<AlertClass>();
            alertClassIcons = new Dictionary<string, string>();
            this.CreateHandle(cp);
            windowHandle = this.Handle;
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
            this.application = app;
            foreach (NotificationType alertClass in notiTypes)
            {
                AlertClass newClass = new AlertClass(alertClass);
                alertClasses.Add(newClass);
                alertClassIcons.Add(newClass.notificationType.Name,newClass.defaultIconLocation);
            }
            pathToIcon = IconFetcher.getPathToIcon(app);
            registerWithSnarl();
        }

        ~GrowlApplication()
        {
            if (System.IO.File.Exists(pathToIcon))
            {
                try
                {
                    System.IO.File.Delete(pathToIcon);
                }
                catch
                {
                    // should think about what to do in this case...
                }
            }
        }

        public class AlertClass {
            private List<string> cachedImages;
            public string defaultIconLocation {get;set;}
            public NotificationType notificationType {get;set;}

            public AlertClass(NotificationType nType) {
                this.notificationType = nType;
                this.defaultIconLocation = IconFetcher.getPathToIcon(nType);
            }

            ~AlertClass()
            {
                if(System.IO.File.Exists(defaultIconLocation)) {
                    System.IO.File.Delete(defaultIconLocation);
                }
            }
        }


        protected override void WndProc(ref Message m)
        {

            if (m.Msg == this.SNARL_GLOBAL_MESSAGE)
            {
                if ((int)m.WParam == Snarl.SnarlConnector.SNARL_LAUNCHED)
                {
                    // Snarl has been (re)started 
                    SnarlConnector.GetSnarlWindow(true);
                    registerWithSnarl();
               
                }
            }
     
            base.WndProc(ref m);

        }

            private void registerWithSnarl() {
                SnarlConnector.RegisterConfig(this.Handle, this.application.Name, Snarl.WindowsMessage.WM_USER + 55,pathToIcon);
                foreach (AlertClass alertClass in alertClasses)
                {
                    SnarlConnector.RegisterAlert(this.application.Name, alertClass.notificationType.Name);
                }
                    
            }


    }

}
