/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using TheCodeKing.Demo;
using XDMessaging;
using XDMessaging.Messages;
using XDMessaging.Transport.Amazon.Entities;

namespace Test_Service
{
    /// <summary>
    ///   A sample Windows Service that demonstrates interprocess communication from a Windows Service. Run an
    ///   instance of Messager to view the broadcast messages.
    /// </summary>
    public partial class TestService : ServiceBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The instance used to broadcast messages on a particular channel.
        /// </summary>
        private IXDBroadcaster broadcast;

        /// <summary>
        ///   Cancellation token for timer.
        /// </summary>
        private CancellationTokenSource cancelToken;

        /// <summary>
        ///   The instance of XDmessagingClient used to create listeners and broadcasters.
        /// </summary>
        private XDMessagingClient client;

        private bool isTraceEnabled;

        /// <summary>
        ///   The instance used to listen for broadcast messages.
        /// </summary>
        private IXDListener listener;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        public TestService()
        {
            InitializeComponent();
            ServiceName = "Test Service";
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Broadcast a message that the service has started.
        /// </summary>
        /// <param name = "args"></param>
        protected override void OnStart(string[] args)
        {
            isTraceEnabled = true;

            client = new XDMessagingClient().WithAmazonSettings(RegionEndPoint.EUWest1);

            listener = client.Listeners.GetListenerForMode(XDTransportMode.Compatibility);
            listener.MessageReceived += OnMessageReceived;
            listener.RegisterChannel("BinaryChannel1");
            listener.RegisterChannel("BinaryChannel2");

            //only the following mode is supported in windows services
            broadcast = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.Compatibility);

            broadcast.SendToChannel("status", "Test Service has started");
            cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
                                      {
                                          while (!cancelToken.IsCancellationRequested)
                                          {
                                              if (isTraceEnabled)
                                              {
                                                  broadcast.SendToChannel("status",
                                                                          "The time is: " + DateTime.Now.ToString("f"));
                                                  Thread.Sleep(5000);
                                              }
                                          }
                                      });
        }

        /// <summary>
        ///   Broadcast a message that the service has stopped.
        /// </summary>
        protected override void OnStop()
        {
            broadcast.SendToChannel("status", "Test Service has stopped");
            cancelToken.Cancel();
            cancelToken = null;
            listener.Dispose();
        }

        /// <summary>
        ///   Handle the MessageReceived event and trace the message to standard debug.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            // view these debug messages using SysInternals Dbgview.
            TypedDataGram<FormattedUserMessage> dataGram = e.DataGram;

            Debug.WriteLine("Test Service: " + e.DataGram.Channel + " " + dataGram.Message);
            if (dataGram.Message.FormattedTextMessage.EndsWith("STOP"))
            {
                broadcast.SendToChannel("status", "Stopping trace");
                isTraceEnabled = false;
            }
            else if (dataGram.Message.FormattedTextMessage.EndsWith("START"))
            {
                broadcast.SendToChannel("status", "Starting trace");
                isTraceEnabled = true;
            }
        }

        #endregion
    }
}