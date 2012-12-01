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
using System.Diagnostics;
using System.ServiceProcess;
using XDMessaging;

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
        ///   The instance of XDmessagingClient used to create listeners and broadcasters.
        /// </summary>
        private XDMessagingClient client;

        /// <summary>
        ///   The instance used to listen to broadcast messages.
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
            client = new XDMessagingClient();

            listener = client.Listeners.GetListenerForMode(XDTransportMode.Compatibility);
            listener.MessageReceived += OnMessageReceived;
            listener.RegisterChannel("BinaryChannel1");
            listener.RegisterChannel("BinaryChannel2");

            //only the following mode is supported in windows services
            broadcast = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.Compatibility);

            // broadcast to all processes listening on the status channel that the service has started
            broadcast.SendToChannel("status", "Test Service has started");
        }

        /// <summary>
        ///   Broadcast a message that the service has stopped.
        /// </summary>
        protected override void OnStop()
        {
            // broadcast to all processes listening on the status channel that the service has stoped
            broadcast.SendToChannel("status", "Test Service has stopped");
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
            Debug.WriteLine("Test Service: " + e.DataGram.Channel + " " + e.DataGram.Message);
        }

        #endregion
    }
}