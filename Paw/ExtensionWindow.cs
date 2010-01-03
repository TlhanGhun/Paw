using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using melon;

namespace Paw
{

    [System.Runtime.InteropServices.ProgId("Paw.extension")]
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual), System.Runtime.InteropServices.ComSourceInterfaces(typeof(MWndProcSink))]

    class ExtensionWindow : MWndProcSink
    {
        #region MWndProcSink Members

        bool MWndProcSink.WndProc(int hWnd, int uMsg, int wParam, int lParam, int PrevWndProc, ref int ReturnValue)
        {
            /*
              Here we get notifications from Snarl.  Don't think of this as a normal
              WndProc() - it isn't; it's just a handy interface we can use.
              
              For all messages, hWnd is a handle to Snarl's main window.
              uMsg can be one of the following:
                  - SNARL_EXT_INIT: Do one-off initialization
                  - SNARL_EXT_START: Called when Snarl starts running
                  - SNARL_EXT_STOP: Called when Snarl stops running
                  - SNARL_EXT_QUIT: Called when Snarl is unloaded
              */
            switch (uMsg)
            {

                default:
                    // do nothing
                    break;


            }

            return true;
        }

        #endregion

    }
}
