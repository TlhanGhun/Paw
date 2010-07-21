using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Growl.Daemon;
using Growl.Connector;
using Growl.CoreLibrary;
using Growl;
using System.Collections;
using Snarl;

namespace Paw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PasswordManager passwordManager;
        GrowlServer growl;
        Dictionary<string, GrowlApplication> registeredApps;
        string pawIconPath;
        bool isRunning = false;
        string appDataPath;
        string appProgramPath;
        private WindowState m_storedWindowState = WindowState.Normal;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenu m_notifyMenu;

        public MainWindow()
        {
            InitializeComponent();
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tlhan Ghun\\Paw\\";
            appProgramPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            try
            {
                if (Properties.Settings.Default.useAutomaticUpdate)
                {
                    Winkle.VersionCheck myUpdateChecker = new Winkle.VersionCheck("Paw", "http://tlhan-ghun.de/files/pawWinkle.xml");
                    Winkle.UpdateInfo myUpdateResponse = myUpdateChecker.checkForUpdate(System.Reflection.Assembly.GetExecutingAssembly(), true);
                    Console.WriteLine("Update check done");
                }
            }
            catch 
            {
                
            }
            passwordManager = new PasswordManager();
            registeredApps = new Dictionary<string, GrowlApplication>();
            pawIconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Paw.ico";
            if (!System.IO.Directory.Exists(appDataPath))
            {
                System.IO.Directory.CreateDirectory(appDataPath);
            }
            Growl.Connector.Application pawApp = new Growl.Connector.Application("Paw");
            List<NotificationType> pawClasses = new List<NotificationType>();
            pawClasses.Add(new NotificationType("General notifications"));
            pawClasses.Add(new NotificationType("Growl server messages"));
            pawClasses.Add(new NotificationType("Error messages"));
            pawApp.Icon = Growl.CoreLibrary.ImageConverter.ImageFromUrl(appProgramPath + "\\Paw.ico");
            

            GrowlApplication comWindow = new GrowlApplication(pawApp, pawClasses);

            registeredApps.Add("Paw", comWindow);

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Text = "Paw - a GNTP to Snarl bridge";
            m_notifyIcon.Icon = new System.Drawing.Icon(pawIconPath);
            m_notifyIcon.Click += new EventHandler(trayClick);  

            m_notifyMenu = new System.Windows.Forms.ContextMenu();
            m_notifyMenu.MenuItems.Add("Paw");
            m_notifyMenu.MenuItems.Add("-");
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Start / stop", new System.EventHandler(trayStartStop)));
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Preferences", new System.EventHandler(trayClick)));
            m_notifyMenu.MenuItems.Add("-");
            m_notifyMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Quit", new System.EventHandler(trayClose)));

            m_notifyIcon.ContextMenu = m_notifyMenu;
        }

        ~MainWindow()
        {
            foreach(GrowlApplication wnd in registeredApps.Values) {
                SnarlConnector.RevokeConfig(wnd.windowHandle);
                wnd.DestroyHandle();
            }
            Properties.Settings.Default.Save();
        }

        private void startServer() {
             if (growl == null)
                {
                    growl = new GrowlServer(Properties.Settings.Default.GrowlListenPort, passwordManager, System.IO.Path.GetTempPath());

                    growl.AllowFlash = Properties.Settings.Default.AllowFlash;
                    growl.AllowNetworkNotifications = Properties.Settings.Default.AllowNetworkNotifications;

                    growl.RegisterReceived += new GrowlServer.RegisterReceivedEventHandler(registerReceived);
                    growl.NotifyReceived += new GrowlServer.NotifyReceivedEventHandler(notificationReceived);
                    growl.SubscribeReceived += new GrowlServer.SubscribeReceivedEventHandler(growl_SubscribeReceived);
                    growl.FailedToStart += new EventHandler(growl_FailedToStart);
                    growl.ServerMessage += new GrowlServer.ServerMessageEventHandler(growl_ServerMessage);
                }
                growl.Start();
                isRunning = true;
                this.button_startStop.Content = "Stop";
                label_serverState.Content = "GNTP Listener running";
                ellipse_ServerState.Fill = Brushes.Green;
                m_notifyIcon.Text = "Paw - listening";


        }

        private void stopServer()
        {
            growl.Stop();
            this.button_startStop.Content = "Start";
            isRunning = false;
            label_serverState.Content = "GNTP Listener stopped";
            ellipse_ServerState.Fill = Brushes.Red;
            m_notifyIcon.Text = "Paw stopped";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                startServer();
            }
            else
            {
                stopServer();
            } 
        }

        void growl_ServerMessage(GrowlServer sender, GrowlServer.LogMessageType type, string message)
        {
            if (Properties.Settings.Default.ShowServerMessages)
            {
                SnarlConnector.ShowMessageEx("Growl server messages", sender.ServerName, message, 10, pawIconPath, registeredApps["Paw"].windowHandle, Snarl.WindowsMessage.WM_USER + 24, "");
            }
        }

        void growl_FailedToStart(object sender, EventArgs e)
        {
            SnarlConnector.ShowMessageEx("Error messages", "Growl server failed to start", "Tried to start the Growl server but it failed", 10, pawIconPath, registeredApps["Paw"].windowHandle, Snarl.WindowsMessage.WM_USER + 24, "");
        }

        SubscriptionResponse growl_SubscribeReceived(Subscriber subscriber, RequestInfo requestInfo)
        {
            SubscriptionResponse returnCode = new SubscriptionResponse(0);
            return returnCode;
        }

        private Response notificationReceived(Notification notification, CallbackInfo callbackInfo, RequestInfo requestInfo)
        {
            Response returnCode = new Response();
            if (!registeredApps.ContainsKey(notification.ApplicationName.ToUpper()))
            {
                returnCode = new Response(ErrorCode.UNKNOWN_APPLICATION, "Application not registered");
                SnarlConnector.ShowMessageEx("Error messages", "Growl notification from unregistered app", "The application " + notification.ApplicationName + " tried to send a notification without being registered", 10, pawIconPath, registeredApps["Paw"].windowHandle, Snarl.WindowsMessage.WM_USER + 24, "");
            }
            else
            {
                bool deleteIcon = false;
                string iconPath = "";
                iconPath = IconFetcher.getPathToIcon(notification);
                if (iconPath == "")
                {
                    GrowlApplication thisApp = registeredApps[notification.ApplicationName.ToUpper()];
                    if (notification.Name != null)
                    {
                        if (thisApp.alertClassIcons.ContainsKey(notification.Name))
                        {
                            iconPath = thisApp.alertClassIcons[notification.Name];
                        }
                    }
                    if (iconPath == "")
                    {
                        iconPath = registeredApps[notification.ApplicationName.ToUpper()].pathToIcon;
                    }
                }
                else
                {
                    deleteIcon = true;
                }
                SnarlConnector.ShowMessageEx(notification.Name, notification.Title, notification.Text, 10, iconPath, registeredApps[notification.ApplicationName.ToUpper()].windowHandle, Snarl.WindowsMessage.WM_USER + 24, "");
                if (deleteIcon)
                {
                    try
                    {
                        System.IO.File.Delete(iconPath);
                    }
                    catch
                    { }
                }
            }
            return returnCode;
        }

        private Response registerReceived(Growl.Connector.Application application, List<NotificationType> notificationTypes, RequestInfo requestInfo)
        {
            Response returnCode = new Response();
            if(registeredApps.ContainsKey(application.Name.ToUpper())) {
                returnCode = new Response(ErrorCode.ALREADY_PROCESSED, "Already registered");
            }
            else
            {
                GrowlApplication comWindow = new GrowlApplication(application, notificationTypes);
                
                registeredApps.Add(application.Name.ToUpper(), comWindow);
            }
            return returnCode;
        }

        private void checkBoxAllowFlash_Checked(object sender, RoutedEventArgs e)
        {
            if (growl != null)
            {
                growl.AllowFlash = Properties.Settings.Default.AllowFlash;
            }
        }


        private void checkBoxAllowNetwork_Checked(object sender, RoutedEventArgs e)
        {
            if (growl != null)
            {
                growl.AllowNetworkNotifications = Properties.Settings.Default.AllowNetworkNotifications;
            }
        }

        #region TrayIcon

        private void trayClick(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
            OnStateChanged(null, null);
        }

        private void trayStartStop(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void trayClose(object sender, EventArgs e)
        {
            this.Close();
        }
        
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                m_storedWindowState = WindowState.Minimized;
                Hide();
            }
            if (this.WindowState != WindowState.Minimized)
            {
                Show();
                this.WindowState = WindowState.Normal;
               
                this.BringIntoView();
                this.Visibility = Visibility.Visible;
                this.UpdateLayout();
            }

        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startServer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_notifyIcon.Visible = false;
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

    }
}
