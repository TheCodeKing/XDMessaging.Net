/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XDMessaging;
using XDMessaging.Messages;
using XDMessaging.Transport.Amazon.Entities;

namespace TheCodeKing.Demo
{
    /// <summary>
    /// 	A demo messaging application which demostrates the cross AppDomain Messaging API.
    /// 	This independent instances of the application to receive and send messages between
    /// 	each other.
    /// </summary>
    public partial class Messenger : Form
    {
        #region Constants and Fields

        /// <summary>
        /// 	The instance used to broadcast messages on a particular channel.
        /// </summary>
        private IXDBroadcaster broadcast;

        /// <summary>
        /// 	The XDMessaging client API.
        /// </summary>
        private XDMessagingClient client;

        /// <summary>
        /// 	The instance used to listen to broadcast messages.
        /// </summary>
        private IXDListener listener;

        /// <summary>
        /// 	Uniqe name for this application instance.
        /// </summary>
        private string uniqueInstanceName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// 	Default constructor.
        /// </summary>
        public Messenger()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	The closing overrride used to broadcast on the status channel that the window is
        /// 	closing.
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var message = string.Format("{0} is shutting down", Handle);
            broadcast.SendToChannel("Status", message);
        }

        /// <summary>
        /// 	The onload event which initializes the messaging API by registering
        /// 	for the Status and Message channels. This also assigns a delegate for
        /// 	processing messages received.
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            uniqueInstanceName = string.Format("{0}-{1}", Environment.MachineName, Handle);
            // create instance of the XDmessaging client and set region, crendentials should be in app.config
            client = new XDMessagingClient().WithAmazonSettings(RegionEndPoint.EUWest1);

            if (!client.HasValidAmazonSettings())
            {
                MessageBox.Show("Azazon AWS crendentials not set. Enter your credentials in the app.config.",
                                "Missing AWS Crendentials",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                propagateCheck.CheckState = CheckState.Unchecked;
                propagateCheck.Enabled = false;
                mailRadio.Enabled = false;
            }

            var tooltips = new ToolTip();
            tooltips.SetToolTip(sendBtn, "Broadcast message on Channel 1\r\nand Channel2");
            tooltips.SetToolTip(groupBox1, "Choose which channels\r\nthis instance will\r\nlisten on");
            tooltips.SetToolTip(Mode, "Choose which mode\r\nto use for sending\r\nand receiving");

            UpdateDisplayText(
                "Launch multiple instances of this application to demo interprocess communication. Run multiple instances on different machines to demo network propogation.\r\n",
                Color.Gray);

            // set the handle id in the form title
            Text += string.Format(" - {0}", uniqueInstanceName);

            InitializeMode(XDTransportMode.HighPerformanceUI);

            // broadcast on the status channel that we have loaded
            var message = string.Format("{0} has joined", uniqueInstanceName);
            broadcast.SendToChannel("Status", message);
        }

        /// <summary>
        /// 	Wire up the enter key to submit a message.
        /// </summary>
        /// <param name = "m"></param>
        /// <param name = "k"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message m, Keys k)
        {
            // allow enter to send message
            if (m.Msg == 256 && k == Keys.Enter)
            {
                SendMessage();
                return true;
            }
            return base.ProcessCmdKey(ref m, k);
        }

        /// <summary>
        /// 	Initialize the broadcast and listener mode.
        /// </summary>
        /// <param name = "mode">The new mode.</param>
        private void InitializeMode(XDTransportMode mode)
        {
            if (listener != null)
            {
                // ensure we dispose any previous listeners, dispose should aways be
                // called on IDisposable objects when we are done with it to avoid leaks
                listener.Dispose();
            }

            // creates an instance of the IXDListener object using the given implementation  
            listener = client.Listeners.GetListenerForMode(mode);

            // attach the message handler
            listener.MessageReceived += OnMessageReceived;

            // register the channels we want to listen on
            if (statusCheckBox.Checked)
            {
                listener.RegisterChannel("Status");
            }

            // register if checkbox is checked
            if (channel1Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel1");
            }

            // register if checkbox is checked
            if (channel2Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel2");
            }

            // if we already have a broadcast instance
            if (broadcast != null)
            {
                // send in plain text
                var message = string.Format("{0} is changing mode to {1}", uniqueInstanceName, mode);
                broadcast.SendToChannel("Status", message);
            }

            // create an instance of IXDBroadcast using the given mode
            broadcast = client.Broadcasters.GetBroadcasterForMode(mode, propagateCheck.Checked);
        }

        /// <summary>
        /// 	The delegate which processes all cross AppDomain messages and writes them to screen.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            // If called from a seperate thread, rejoin so that be can update form elements.
            if (InvokeRequired && !IsDisposed)
            {
                try
                {
                    // onClosing messages may fail if the form is being disposed.
                    Invoke((MethodInvoker) (() => OnMessageReceived(sender, e)));
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
            else
            {
                switch (e.DataGram.Channel.ToLower())
                {
                    case "status":
                        // pain text
                        UpdateDisplayText(e.DataGram.Channel, e.DataGram.Message);
                        break;
                    default:
                        // all other channels contain serialized FormattedUserMessage object 
                        if (e.DataGram.AssemblyQualifiedName == typeof(FormattedUserMessage).AssemblyQualifiedName)
                        {
                            TypedDataGram<FormattedUserMessage> typedDataGram = e.DataGram;
                            UpdateDisplayText(typedDataGram.Channel, typedDataGram.Message.FormattedTextMessage);
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Unknown message type: {0}", e.DataGram.AssemblyQualifiedName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 	Helper method for sending message.
        /// </summary>
        private void SendMessage()
        {
            if (inputTextBox.Text.Length > 0)
            {
                // send FormattedUserMessage object to all channels
                var message = new FormattedUserMessage("{0} says {1}", uniqueInstanceName, inputTextBox.Text);

                if (channel1Check.Checked)
                {
                    broadcast.SendToChannel("BinaryChannel1", message);
                }
                if (channel2Check.Checked)
                {
                    broadcast.SendToChannel("BinaryChannel2", message);
                }
                inputTextBox.Text = "";
            }
        }

        private void SetMode()
        {
            if (wmRadio.Checked)
            {
                InitializeMode(XDTransportMode.HighPerformanceUI);
            }
            else if (ioStreamRadio.Checked)
            {
                InitializeMode(XDTransportMode.Compatibility);
            }
            else
            {
                InitializeMode(XDTransportMode.RemoteNetwork);
            }
        }

        /// <summary>
        /// 	A helper method used to update the Windows Form.
        /// </summary>
        /// <param name = "channelName">The channel to display</param>
        /// <param name = "displayText">The message to display</param>
        private void UpdateDisplayText(string channelName, string displayText)
        {
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrEmpty(displayText))
            {
                return;
            }

            Color textColor;
            switch (channelName.ToLower())
            {
                case "status":
                    textColor = Color.Green;
                    break;
                default:
                    textColor = Color.Blue;
                    break;
            }
            var msg = string.Format("{0}: {1}\r\n", channelName, displayText);
            UpdateDisplayText(msg, textColor);
        }

        /// <summary>
        /// 	A helper method used to update the Windows Form.
        /// </summary>
        /// <param name = "message">The message to be displayed on the form.</param>
        /// <param name = "textColor">The colour text to use for the message.</param>
        private void UpdateDisplayText(string message, Color textColor)
        {
            if (!IsDisposed)
            {
                displayTextBox.AppendText(message);
                displayTextBox.Select(displayTextBox.Text.Length - message.Length + 1, displayTextBox.Text.Length);
                displayTextBox.SelectionColor = textColor;
                displayTextBox.Select(displayTextBox.Text.Length, displayTextBox.Text.Length);
                displayTextBox.ScrollToCaret();
            }
        }

        /// <summary>
        /// 	Adds or removes the Message channel from the messaging API. This effects whether messages 
        /// 	sent on this channel will be received by the application. Status messages are broadcast 
        /// 	on the Status channel whenever this setting is changed.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void Channel1CheckedChanged(object sender, EventArgs e)
        {
            if (channel1Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel1");
                // send in pain text
                var message = string.Format("{0} is registering Channel1.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("BinaryChannel1");
                // send in pain text
                var message = string.Format("{0} is unregistering Channel1.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
        }

        /// <summary>
        /// 	Adds or removes the Message channel from the messaging API. This effects whether messages 
        /// 	sent on this channel will be received by the application. Status messages are broadcast 
        /// 	on the Status channel whenever this setting is changed.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void Channel2CheckedChanged(object sender, EventArgs e)
        {
            if (channel2Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel2");
                // send in pain text
                var message = string.Format("{0} is registering Channel2.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("BinaryChannel2");
                // send in pain text
                var message = string.Format("{0} is unregistering Channel2.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
        }

        /// <summary>
        /// 	On form changed mode.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void ModeCheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Checked)
            {
                SetMode();
            }
        }

        private void PropagateCheckCheckedChanged(object sender, EventArgs e)
        {
            if (propagateCheck.Checked)
            {
                UpdateDisplayText("Messages will be propagated across the network to all listeners.\r\n",
                                  Color.Red);
            }
            else
            {
                UpdateDisplayText("Message are restricted to the current machine.\r\n", Color.Red);
            }
            SetMode();
        }

        /// <summary>
        /// 	Sends a user input string on the Message channel. A message is not sent if
        /// 	the string is empty.
        /// </summary>
        /// <param name = "sender">The event sender.</param>
        /// <param name = "e">The event args.</param>
        private void SendBtnClick(object sender, EventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// 	Adds or removes the Status channel from the messaging API. This effects whether messages 
        /// 	sent on this channel will be received by the application. Status messages are broadcast 
        /// 	on the Status channel whenever this setting is changed.
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void StatusChannelCheckedChanged(object sender, EventArgs e)
        {
            if (statusCheckBox.Checked)
            {
                listener.RegisterChannel("Status");
                // send in plain text
                var message = string.Format("{0} is registering Status.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("Status");
                // send in plain text
                var message = string.Format("{0} is unregistering Status.", uniqueInstanceName);
                broadcast.SendToChannel("Status", message);
            }
        }

        #endregion
    }
}