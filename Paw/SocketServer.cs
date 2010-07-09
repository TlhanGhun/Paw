using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using Snarl;
using System.IO;

namespace Paw
{
    /// <summary>
    /// Description of SocketServer.	
    /// </summary>
    public class SocketServer : System.Windows.Forms.Form
    {

        private KnownBinaryDataController knownBinaryDataController = new KnownBinaryDataController();
        private RegisteredAppsController registeredAppsController = new RegisteredAppsController();


        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxReceivedMsg;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxMsg;
        private System.Windows.Forms.Button buttonStopListen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBoxSendMsg;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Button buttonStartListen;
        private System.Windows.Forms.Button buttonSendMsg;
        private System.Windows.Forms.Button buttonClose;

        public delegate void UpdateRichEditCallback(string text);
        public delegate void UpdateClientListCallback();

        public AsyncCallback pfnWorkerCallBack;
        private Socket m_mainSocket;

        // An ArrayList is used to keep track of worker sockets that are designed
        // to communicate with each connected client. Make it a synchronized ArrayList
        // For thread safety
        private System.Collections.ArrayList m_workerSocketList =
                ArrayList.Synchronized(new System.Collections.ArrayList());

        // The following variable will keep track of the cumulative 
        // total number of clients connected at any time. Since multiple threads
        // can access this variable, modifying this variable should be done
        // in a thread safe manner
        private int m_clientCount = 0;

        private System.Windows.Forms.ListBox listBoxClientList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnClear;

        public SocketServer()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            // Display the local IP address on the GUI
            textBoxIP.Text = GetIP();

            ButtonStartListenClick(null, null);

            if (BonjourService.IsSupported)
            {
                BonjourService myBonjour = new BonjourService(String.Format("Paw on {0}", Environment.MachineName), "_gntp._tcp");
                myBonjour.Start(23053);
            }
        }


        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SocketServer));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSendMsg = new System.Windows.Forms.Button();
            this.buttonStartListen = new System.Windows.Forms.Button();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.richTextBoxSendMsg = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStopListen = new System.Windows.Forms.Button();
            this.textBoxMsg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.richTextBoxReceivedMsg = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxClientList = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(112, 357);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(88, 24);
            this.buttonClose.TabIndex = 11;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.ButtonCloseClick);
            // 
            // buttonSendMsg
            // 
            this.buttonSendMsg.Location = new System.Drawing.Point(16, 144);
            this.buttonSendMsg.Name = "buttonSendMsg";
            this.buttonSendMsg.Size = new System.Drawing.Size(192, 24);
            this.buttonSendMsg.TabIndex = 7;
            this.buttonSendMsg.Text = "Send Message";
            this.buttonSendMsg.Click += new System.EventHandler(this.ButtonSendMsgClick);
            // 
            // buttonStartListen
            // 
            this.buttonStartListen.BackColor = System.Drawing.Color.Blue;
            this.buttonStartListen.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartListen.ForeColor = System.Drawing.Color.Yellow;
            this.buttonStartListen.Location = new System.Drawing.Point(227, 16);
            this.buttonStartListen.Name = "buttonStartListen";
            this.buttonStartListen.Size = new System.Drawing.Size(88, 40);
            this.buttonStartListen.TabIndex = 4;
            this.buttonStartListen.Text = "Start Listening";
            this.buttonStartListen.UseVisualStyleBackColor = false;
            this.buttonStartListen.Click += new System.EventHandler(this.ButtonStartListenClick);
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(88, 16);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.ReadOnly = true;
            this.textBoxIP.Size = new System.Drawing.Size(120, 20);
            this.textBoxIP.TabIndex = 12;
            // 
            // richTextBoxSendMsg
            // 
            this.richTextBoxSendMsg.Location = new System.Drawing.Point(16, 87);
            this.richTextBoxSendMsg.Name = "richTextBoxSendMsg";
            this.richTextBoxSendMsg.Size = new System.Drawing.Size(192, 57);
            this.richTextBoxSendMsg.TabIndex = 6;
            this.richTextBoxSendMsg.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port";
            // 
            // buttonStopListen
            // 
            this.buttonStopListen.BackColor = System.Drawing.Color.Red;
            this.buttonStopListen.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStopListen.ForeColor = System.Drawing.Color.Yellow;
            this.buttonStopListen.Location = new System.Drawing.Point(321, 16);
            this.buttonStopListen.Name = "buttonStopListen";
            this.buttonStopListen.Size = new System.Drawing.Size(88, 40);
            this.buttonStopListen.TabIndex = 5;
            this.buttonStopListen.Text = "Stop Listening";
            this.buttonStopListen.UseVisualStyleBackColor = false;
            this.buttonStopListen.Click += new System.EventHandler(this.ButtonStopListenClick);
            // 
            // textBoxMsg
            // 
            this.textBoxMsg.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMsg.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBoxMsg.Location = new System.Drawing.Point(112, 340);
            this.textBoxMsg.Name = "textBoxMsg";
            this.textBoxMsg.ReadOnly = true;
            this.textBoxMsg.Size = new System.Drawing.Size(99, 13);
            this.textBoxMsg.TabIndex = 14;
            this.textBoxMsg.Text = "None";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Broadcast Message To Clients";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(217, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(192, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Message Received From Clients";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(88, 40);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(40, 20);
            this.textBoxPort.TabIndex = 0;
            this.textBoxPort.Text = "23053";
            // 
            // richTextBoxReceivedMsg
            // 
            this.richTextBoxReceivedMsg.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.richTextBoxReceivedMsg.Location = new System.Drawing.Point(217, 88);
            this.richTextBoxReceivedMsg.Name = "richTextBoxReceivedMsg";
            this.richTextBoxReceivedMsg.ReadOnly = true;
            this.richTextBoxReceivedMsg.Size = new System.Drawing.Size(526, 466);
            this.richTextBoxReceivedMsg.TabIndex = 9;
            this.richTextBoxReceivedMsg.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Server IP";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 338);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "Status Message:";
            // 
            // listBoxClientList
            // 
            this.listBoxClientList.BackColor = System.Drawing.SystemColors.Control;
            this.listBoxClientList.Location = new System.Drawing.Point(16, 199);
            this.listBoxClientList.Name = "listBoxClientList";
            this.listBoxClientList.Size = new System.Drawing.Size(192, 121);
            this.listBoxClientList.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Connected Clients";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(3, 357);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 24);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // SocketServer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(755, 566);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.listBoxClientList);
            this.Controls.Add(this.textBoxMsg);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.richTextBoxReceivedMsg);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSendMsg);
            this.Controls.Add(this.richTextBoxSendMsg);
            this.Controls.Add(this.buttonStopListen);
            this.Controls.Add(this.buttonStartListen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SocketServer";
            this.Text = "Paw (GNTP to Snarl bridge)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        void ButtonStartListenClick(object sender, System.EventArgs e)
        {
            try
            {
                // Check the port value
                if (textBoxPort.Text == "")
                {
                    MessageBox.Show("Please enter a Port Number");
                    return;
                }
                string portStr = textBoxPort.Text;
                int port = System.Convert.ToInt32(portStr);
                // Create the listening socket...
                m_mainSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, port);
                // Bind to local IP Address...
                m_mainSocket.Bind(ipLocal);
                // Start listening...
                m_mainSocket.Listen(4);
                // Create the call back for any client connections...
                m_mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);

                UpdateControls(true);

            }
            catch (SocketException se)
            {
              //  MessageBox.Show(se.Message);
            }

        }
        private void UpdateControls(bool listening)
        {
            buttonStartListen.Enabled = !listening;
            buttonStopListen.Enabled = listening;
        }
        // This is the call back function, which will be invoked when a client is connected
        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                Socket workerSocket = m_mainSocket.EndAccept(asyn);

                // Now increment the client count for this client 
                // in a thread safe manner
                Interlocked.Increment(ref m_clientCount);

                // Add the workerSocket reference to our ArrayList
                m_workerSocketList.Add(workerSocket);

                // Send a welcome message to client
                string msg = "Welcome client " + m_clientCount + "\n";
                SendMsgToClient(msg, m_clientCount);

                // Update the list box showing the list of clients (thread safe call)
                UpdateClientListControl();

                // Let the worker Socket do the further processing for the 
                // just connected client
                WaitForData(workerSocket, m_clientCount);

                // Since the main Socket is now free, it can go back and wait for
                // other clients who are attempting to connect
                m_mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
              //  MessageBox.Show(se.Message);
            }

        }
        public class SocketPacket
        {
            // Constructor which takes a Socket and a client number
            public SocketPacket(System.Net.Sockets.Socket socket, int clientNumber)
            {
                m_currentSocket = socket;
                m_clientNumber = clientNumber;
            }
            public System.Net.Sockets.Socket m_currentSocket;
            public int m_clientNumber;
            // Buffer to store the data sent by the client
            public byte[] dataBuffer = new byte[1024000];
        }
        // Start waiting for data from the client
        public void WaitForData(System.Net.Sockets.Socket soc, int clientNumber)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    // Specify the call back function which is to be 
                    // invoked when there is any write activity by the 
                    // connected client
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket(soc, clientNumber);

                soc.BeginReceive(theSocPkt.dataBuffer, 0,
                    theSocPkt.dataBuffer.Length,
                    SocketFlags.None,
                    pfnWorkerCallBack,
                    theSocPkt);
            }
            catch (SocketException se)
            {
                //MessageBox.Show(se.Message);
            }
        }
        // This the call back function which will be invoked when the socket
        // detects any client writing of data on the stream
        public void OnDataReceived(IAsyncResult asyn)
        {
            SocketPacket socketData = (SocketPacket)asyn.AsyncState;
            try
            {
                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream 
                // by the client
                int iRx = socketData.m_currentSocket.EndReceive(asyn);
               
                char[] chars = new char[iRx + 1];
                // Extract the characters as a buffer
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                
                int charLen = d.GetChars(socketData.dataBuffer,
                    0, iRx, chars, 0);
               /*
                MemoryStream byteStream = new MemoryStream(socketData.dataBuffer,0,iRx);

                System.IO.BinaryReader myBinReader = new System.IO.BinaryReader(byteStream);
                myBinReader.
                */
                System.String szData = new System.String(chars);
                
                //System.String szData = HelperFunctions.EncodeTo64(socketData.dataBuffer);
              //  System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                //string szData = encoding.GetString(socketData.dataBuffer);
                                // Send back the reply to the client
                string replyMsg = "";
                string command = "";
                // Convert the reply to byte array

                replyMsg = "GNTP/1.0 -OK NONE";
                string[] separators = new string[1];
                separators[0] = "\r\n";
                string[] lines = szData.Split(separators, StringSplitOptions.None);

                if(!szData.StartsWith("GNTP/1.0 ")) {
                    replyMsg = "GNTP/1.0 -ERROR 301";

                }
                else
                {
                    replyMsg = "GNTP/1.0 -OK NONE";
                    separators[0] = " ";
                    string[] parsedString = lines[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    command = parsedString[1];
                }

                if (command.ToLower() == "register")
                {
                    GrowlApp thisRequest =  parseRegister(lines);
                    if (thisRequest != null)
                    {
                        registeredAppsController.registerApp(thisRequest);
                    }

                }
                else if (command.ToLower() == "notify")
                {
                    getNotification(lines);

                }

                

                string msg = "" + socketData.m_clientNumber + ":";
                AppendToRichEditControl(msg + szData);


                byte[] byData = System.Text.Encoding.ASCII.GetBytes(replyMsg);

                Socket workerSocket = (Socket)socketData.m_currentSocket;
                workerSocket.Send(byData);

                //socketData.m_currentSocket.Close();
                // Continue the waiting for data on the Socket
                WaitForData(socketData.m_currentSocket, socketData.m_clientNumber);

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    string msg = "Client " + socketData.m_clientNumber + " Disconnected" + "\n";
                    AppendToRichEditControl(msg);

                    // Remove the reference to the worker socket of the closed client
                    // so that this object will get garbage collected
                    m_workerSocketList[socketData.m_clientNumber - 1] = null;
                    UpdateClientListControl();
                }
                else
                {
                   // MessageBox.Show(se.Message);
                }
            }
        }

        private void getNotification(string[] lines)
        {
              Notification currentNotification = parseNotifiy(lines);
                    if (currentNotification != null)
                    {
                        GrowlApp appOfThisNotification = registeredAppsController.searchAppByName(currentNotification.appName);
                        string defaultIconPath = "";
                        if (appOfThisNotification != null)
                        {
                            currentNotification.handleOfBelongingApp = appOfThisNotification.commWindowHandle;
                            defaultIconPath = appOfThisNotification.parseIcon(appOfThisNotification.iconPath);
                        }
                        currentNotification.sendNotification(defaultIconPath);
                    }
        }

        private GrowlApp parseRegister(string[] lines)
        {
            GrowlApp myRegisterRequest = new GrowlApp();
            long currentLine = 0;
            foreach (string line in lines) 
            {
                currentLine++;
                if (line.StartsWith("GNTP/1.0 REGISTER"))
                {
                    // just the first line
                    continue;
                }
                else if (line.StartsWith("GNTP/1.0 NOTIFY"))
                {
                    string[] partialArray = new string[lines.Length - currentLine + 1];
                    Array.Copy(lines, currentLine - 1, partialArray, 0, lines.Length - currentLine + 1);
                    lines = null;                    
                    getNotification(partialArray);
                    break;
                }
                Attribute attribute = parseAttribute(line);
                switch (attribute.name)
                {
                    case "Application-Name":
                        myRegisterRequest.appName = attribute.value;
                        break;
                    case "Application-Icon":
                        myRegisterRequest.iconPath = attribute.value;
                        break;
                    case "Notifications-Count":
                        myRegisterRequest.numberOfClasses = Convert.ToInt32(attribute.value);
                        break;
                    case "Notification-Name":
                        // description of classes about to begin
                        string[] partialArray = new string[lines.Length-currentLine+1];
                        Array.Copy(lines,currentLine-1,partialArray,0,lines.Length-currentLine+1);
                        myRegisterRequest = parseClasses(partialArray, myRegisterRequest);
                        break;
                    default:
                        // bla
                        break;
                }
            }
            return myRegisterRequest;
        }

        private GrowlApp parseClasses(string[] lines, GrowlApp myRegisterRequest)
        {
            NotificationClass currentClass = new NotificationClass();
            long currentLine = 0;

            foreach (string line in lines)
            {
                Attribute attribute = parseAttribute(line);
                currentLine++;  
                switch (attribute.name)
                {
                    case "Notification-Name":
                        if (currentClass.className != "")
                        {
                            myRegisterRequest.notificationClasses.Add(currentClass);
                        }
                        currentClass = new NotificationClass();
                        currentClass.className = attribute.value;
                        currentClass.isEnabled = true;
                        break;
                    case "Notification-Icon":
                        currentClass.defaultIconPath = attribute.value;
                        break;
                    case "Identifier":
                        // for now we ignore binary :)
                        if (currentClass.className != "")
                        {
                            myRegisterRequest.notificationClasses.Add(currentClass);
                        }
                        string[] partialArray = new string[lines.Length - currentLine + 1];
                        Array.Copy(lines, currentLine - 1, partialArray, 0, lines.Length - currentLine + 1);
                        parseIdentifiers(partialArray);
                        return myRegisterRequest;
                    default:
                        // bla
                        break;
                }

            }
            return myRegisterRequest;

        }

        private Notification parseNotifiy(string[] lines)
        {
            Notification myNotification = new Notification();
            long currentLine = 0;
            foreach (string line in lines)
            {
                currentLine++;
                if (line.StartsWith("GNTP/1.0 NOTIFY"))
                {
                    // just the first line
                    continue;
                }
                Attribute attribute = parseAttribute(line);
                switch (attribute.name)
                {
                    case "Application-Name":
                        myNotification.appName = attribute.value;
                        break;
                    case "Notification-Icon":
                        myNotification.icon = attribute.value;
                        break;
                    case "Notification-Name":
                        myNotification.className = attribute.value;
                        break;
                    case "Notification-Title":
                        myNotification.title = attribute.value;
                        break;
                    case "Notification-Text":
                        myNotification.text = attribute.value;
                        break;
                    default:
                        // bla
                        break;
                }
            }
            return myNotification;
        }

        private void parseIdentifiers(string[] lines)
        {
            long currentLine = 0;
            bool withinBinaryBlob = false;
            bool onFirstLineOfBinaryBlob = true;

            string currentIdentifier = "";
            string binaryData = "";
            Int64 length = 0;

            foreach (string line in lines)
            {
                if (line.StartsWith("GNTP/1.0 NOTIFY"))
                {
                    string[] partialArray = new string[lines.Length - currentLine + 1];
                    Array.Copy(lines, currentLine - 1, partialArray, 0, lines.Length - currentLine + 1);
                    lines = null;
                    getNotification(partialArray);
                    return;
                }
                if (withinBinaryBlob)
                {
                    if (onFirstLineOfBinaryBlob == true && line.Trim() == string.Empty)
                    {
                        binaryData = "";
                        continue;
                    }
                    else
                    {
                        onFirstLineOfBinaryBlob = false;
                        if (line.Trim() != string.Empty && !line.StartsWith("Identifier"))
                        {
                            binaryData += line;
                            currentLine++;
                            continue;
                        }
                        else
                        {
                            knownBinaryDataController.storeDataLocally(currentIdentifier, binaryData, length);
                            currentIdentifier = "";
                            binaryData = "";
                            length = 0;
                            withinBinaryBlob = false;
                        }
                    }
                }
                Attribute attribute = parseAttribute(line);
                currentLine++;
                switch (attribute.name)
                {
                    case "Identifier":
                        currentIdentifier = attribute.value;
                        break;
                    case "Length":
                        length = Convert.ToInt64(attribute.value);
                        onFirstLineOfBinaryBlob = true;
                        withinBinaryBlob = true;
                        break;
                    default:
                        // bla
                        break;
                }

            }
            return;

        }

        private Attribute parseAttribute(string line)
        {
            Attribute returnCode = new Attribute();
            string[] separators = new string[1];
            separators[0] = ": ";
            string[] parsebleCode = line.Split(separators, StringSplitOptions.None);
            if(parsebleCode.Length > 1) {
                returnCode.name = parsebleCode[0];
                returnCode.value = line.Substring(parsebleCode[0].Length + 2);
            }

            return returnCode;
        }
        // This method could be called by either the main thread or any of the
        // worker threads
        private void AppendToRichEditControl(string msg)
        {
            // Check to see if this method is called from a thread 
            // other than the one created the control
            if (InvokeRequired)
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                object[] pList = { msg };
                richTextBoxReceivedMsg.BeginInvoke(new UpdateRichEditCallback(OnUpdateRichEdit), pList);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                OnUpdateRichEdit(msg);
            }
        }
        // This UpdateRichEdit will be run back on the UI thread
        // (using System.EventHandler signature
        // so we don't need to define a new
        // delegate type here)
        private void OnUpdateRichEdit(string msg)
        {
            richTextBoxReceivedMsg.AppendText(msg);
        }

        private void UpdateClientListControl()
        {
            if (InvokeRequired) // Is this called from a thread other than the one created
            // the control
            {
                // We cannot update the GUI on this thread.
                // All GUI controls are to be updated by the main (GUI) thread.
                // Hence we will use the invoke method on the control which will
                // be called when the Main thread is free
                // Do UI update on UI thread
                listBoxClientList.BeginInvoke(new UpdateClientListCallback(UpdateClientList), null);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                UpdateClientList();
            }
        }
        void ButtonSendMsgClick(object sender, System.EventArgs e)
        {
            try
            {
                string msg = richTextBoxSendMsg.Text;
                msg = "Server Msg: " + msg + "\n";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
                Socket workerSocket = null;
                for (int i = 0; i < m_workerSocketList.Count; i++)
                {
                    workerSocket = (Socket)m_workerSocketList[i];
                    if (workerSocket != null)
                    {
                        if (workerSocket.Connected)
                        {
                            workerSocket.Send(byData);
                        }
                    }
                }
            }
            catch (SocketException se)
            {
             //   MessageBox.Show(se.Message);
            }
        }

        void ButtonStopListenClick(object sender, System.EventArgs e)
        {
            CloseSockets();
            UpdateControls(false);
        }

        String GetIP()
        {
            return "0.0.0.0";
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                IPStr = ipaddress.ToString();
                return IPStr;
            }
            return IPStr;
        }
        void ButtonCloseClick(object sender, System.EventArgs e)
        {
            CloseSockets();
            Close();
        }
        void CloseSockets()
        {
            if (m_mainSocket != null)
            {
                m_mainSocket.Close();
            }
            Socket workerSocket = null;
            for (int i = 0; i < m_workerSocketList.Count; i++)
            {
                workerSocket = (Socket)m_workerSocketList[i];
                if (workerSocket != null)
                {
                    workerSocket.Close();
                    workerSocket = null;
                }
            }
        }
        // Update the list of clients that is displayed
        void UpdateClientList()
        {
            listBoxClientList.Items.Clear();
            for (int i = 0; i < m_workerSocketList.Count; i++)
            {
                string clientKey = Convert.ToString(i + 1);
                Socket workerSocket = (Socket)m_workerSocketList[i];
                if (workerSocket != null)
                {
                    if (workerSocket.Connected)
                    {
                        listBoxClientList.Items.Add(clientKey);
                    }
                }
            }
        }
        void SendMsgToClient(string msg, int clientNumber)
        {
            // Convert the reply to byte array
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);

            Socket workerSocket = (Socket)m_workerSocketList[clientNumber - 1];
            workerSocket.Send(byData);
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            richTextBoxReceivedMsg.Clear();
        }
    }
}
