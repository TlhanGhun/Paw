using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snarl;

namespace Paw
{

    class RegisteredAppsController
    {
        private List<GrowlApp> listOfApps = new List<GrowlApp>();

        public bool registerApp(GrowlApp currentApp)
        {
            if (searchAppByName(currentApp.appName) != null)
            {
                // already regitered in the past
                return false;
            }
            currentApp.createCommWindow();
            
            SnarlConnector.RegisterConfig(currentApp.commWindowHandle, currentApp.appName, Snarl.WindowsMessage.WM_USER + 11, currentApp.parseIcon(currentApp.iconPath));
            foreach (NotificationClass thisClass in currentApp.notificationClasses)
            {
                SnarlConnector.RegisterAlert(currentApp.appName, thisClass.className);
            }
            listOfApps.Add(currentApp);
            currentApp.bringCommWindowToBackground();
            return true;
        }

        public bool unregisterApp(string name)
        {
            GrowlApp thisApp = searchAppByName(name);
            if (thisApp != null)
            {
                SnarlConnector.RevokeConfig(thisApp.commWindowHandle);

                return true;
            }
            else
            {
                return false;
            }
           
        }

        public GrowlApp searchAppByName(string appName)
        {

            List<GrowlApp> tList = listOfApps.FindAll(
                delegate(GrowlApp app) {
                    return app.appName.Equals(appName);
                }
            );
            if (tList.Count > 0)
            {
                return tList.First();
            }
            else
            {
                return null;
            }
        }

        private bool checkName(GrowlApp app)
        {
            return app.Equals(new GrowlApp());
        }

        ~RegisteredAppsController()
        {

        }
    }
}
