using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using melon;

namespace Paw
{

    [System.Runtime.InteropServices.ProgId("Paw.extension")]
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual), System.Runtime.InteropServices.ComSourceInterfaces(typeof(MVersionInfo))]

    class extension : MVersionInfo
    {
        #region MVersionInfo Members

        string MVersionInfo.Date
        {
            get { return "2010-01-10"; }
        }

        string MVersionInfo.Name
        {
            get { return "Paw (GNTP bridge)"; }
        }

        int MVersionInfo.Revision
        {
            get { return 1; }
        }

        int MVersionInfo.Version
        {
            get { return 1; }
        }

        

        #endregion



    }
}
