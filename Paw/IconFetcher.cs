using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Paw
{
    public class IconFetcher
    {
        public static string getPathToIcon(Growl.Connector.Application app) {
            string returnValue = "";
            if (app.Icon != null)
            {
                if (app.Icon.IsSet)
                {
                    if (app.Icon.IsUrl)
                    {
                        returnValue = app.Icon.Url;
                    }
                    else
                    {
                        returnValue = Path.GetTempFileName();
                        System.Drawing.Image Icon = app.Icon;
                        Icon.Save(returnValue);
                    }
                }
            }
            return returnValue;
        }

        public static string getPathToIcon(Growl.Connector.NotificationType nType)
        {
            string returnValue = "";
            if (nType.Icon != null)
            {
                if (nType.Icon.IsSet)
                {
                    if (nType.Icon.IsUrl)
                    {
                        returnValue = nType.Icon.Url;
                    }
                    else
                    {
                        returnValue = Path.GetTempFileName();
                        System.Drawing.Image Icon = nType.Icon;
                        Icon.Save(returnValue);
                    }
                }
            }
            return returnValue;
        }

        public static string getPathToIcon(Growl.Connector.Notification notification)
        {
            string returnValue = "";
            if (notification.Icon.IsSet)
            {
                if (notification.Icon.IsUrl)
                {
                    returnValue = notification.Icon.Url;
                }
                else
                {
                    returnValue = Path.GetTempFileName();
                    System.Drawing.Image Icon = notification.Icon;
                    Icon.Save(returnValue);
                }
            }
            return returnValue;
        }
    }
}
