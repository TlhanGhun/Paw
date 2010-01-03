using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Snarl;

namespace Paw
{

    // Summary description for SnarlMsgWnd.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    
    
    public class SnarlMsgWnd : NativeWindow
    {
        CreateParams cp = new CreateParams();

        public string pathToIcon = "";

        public IntPtr windowHandle = IntPtr.Zero;

        int SNARL_GLOBAL_MESSAGE;

        public SnarlMsgWnd()
        {
            // Create the actual window
            this.CreateHandle(cp);
            windowHandle = this.Handle;
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
            pathToIcon = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources\\images\\feed.png";
        }

   /*     public void memorizeNotificatedItem(UnreadItem item)
        {
            notifiedItems.Add(item);
        } */

  /*      public IEnumerable<UnreadItem> FindItemsIoSnarlId(Int32 snarlId)
        {
            return from item in notifiedItems
                   where item.SnarlNotificationId == snarlId
                   select item;
        } */




        protected override void WndProc(ref Message m)
        {

            if (m.Msg == this.SNARL_GLOBAL_MESSAGE)
            {
                if ((int)m.WParam == Snarl.SnarlConnector.SNARL_LAUNCHED)
                {
                    // Snarl has been (re)started 
                    SnarlConnector.GetSnarlWindow(true);
                    SnarlConnector.RegisterConfig(this.Handle, "Desktop Google Reader", Snarl.WindowsMessage.WM_USER + 55,pathToIcon);
                    SnarlConnector.RegisterAlert("Desktop Google Reader", "New entry");
                    SnarlConnector.RegisterAlert("Desktop Google Reader", "Number of new items");
               
                }
            }
     
            base.WndProc(ref m);

        }

        ~SnarlMsgWnd()
        {
            MessageBox.Show("Being deconstructed");
        }

    }

}
