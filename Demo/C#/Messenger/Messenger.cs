/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied. Please do not use commerically without permission.
*
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*		12/12/2009	Michael Carlisle				Version 2.0
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheCodeKing.Net.Messaging;

namespace TheCodeKing.Demo
{
    /// <summary>
    /// A demo messaging application which demostrates the cross AppDomain Messaging API.
    /// This independent instances of the application to receive and send messages between
    /// each other.
    /// </summary>
    public partial class Messenger : Form
    {
        /// <summary>
        /// The instance used to listen to broadcast messages.
        /// </summary>
        private IXDListener listener;

        /// <summary>
        /// The instance used to broadcast messages on a particular channel.
        /// </summary>
        private IXDBroadcast broadcast;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Messenger()
        {
            InitializeComponent();
        }
        /// <summary>
        /// The onload event which initializes the messaging API by registering
        /// for the Status and Message channels. This also assigns a delegate for
        /// processing messages received. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateDisplayText("Launch multiple instances to demo interprocess communication.\r\n", Color.Gray);

            // set the handle id in the form title
            this.Text += string.Format("Window Id: {0}", this.Handle);
            // creates an instance of the IXDListener object using the IOStream implementation  
            // which enabled communication with the Windows Services.
            listener = XDListener.CreateListener(XDTransportMode.IOStream);

            // *** NOTE **************************************************************************************
            // we could use a WindowsMessaging based listener instead for receiving messages
            // from forms apps using the WindowsMessaging implementation of IXDBroadcast.
            // listener = XDListener.CreateListener(XDTransportMode.WindowsMessaging);
            // ***********************************************************************************************

            // attach the message handler
            listener.MessageReceived += new XDListener.XDMessageHandler(OnMessageReceived);
            // register the channels we want to listen on
            listener.RegisterChannel("Status");
            listener.RegisterChannel("UserMessage");

            // create an instance of IXDBroadcast using the IOStream implmentation
            broadcast = XDBroadcast.CreateBroadcast(XDTransportMode.IOStream);

            // *** NOTE **************************************************************************************
            // we could use a WindowsMessaging based broadcast instance instead for sending messages
            // to other forms apps using the WindowsMessaging implementation of IXDListener.
            // broadcast = XDBroadcast.CreateBroadcast(XDTransportMode.WindowsMessaging);
            // ***********************************************************************************************

            // broadcast on the status channel that we have loaded
            broadcast.SendToChannel("Status", string.Format("Window {0} created!", this.Handle));
       }

        /// <summary>
        /// The closing overrride used to broadcast on the status channel that the window is
        /// closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            broadcast.SendToChannel("Status", string.Format("Window {0} closing!", this.Handle));
        }
        /// <summary>
        /// The delegate which processes all cross AppDomain messages and writes them to screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            // If called from a seperate thread, rejoin so that be can update form elements.
            if (InvokeRequired)
            {
                try
                {
                    // onClosing messages may fail if the form is being disposed.
                    Invoke((MethodInvoker)delegate() { UpdateDisplayText(e.DataGram); });
                }
                catch { }
            }
            else
            {
                UpdateDisplayText(e.DataGram);
            }
        }

        /// <summary>
        /// A helper method used to update the Windows Form.
        /// </summary>
        /// <param name="dataGram">dataGram</param>
        private void UpdateDisplayText(DataGram dataGram)
        {
            Color textColor;
            switch (dataGram.Channel.ToLower())
            {
                case "status":
                    textColor = Color.Green;
                    break;
                default:
                    textColor = Color.Blue;
                    break;
            }
            string msg = string.Format("{0}: {1}\r\n", dataGram.Channel, dataGram.Message);
            UpdateDisplayText(msg, textColor);
        }

        /// <summary>
        /// A helper method used to update the Windows Form.
        /// </summary>
        /// <param name="dataGram">dataGram</param>
        private void UpdateDisplayText(string message, Color textColor)
        {
            this.displayTextBox.AppendText(message);
            this.displayTextBox.Select(this.displayTextBox.Text.Length - message.Length + 1, this.displayTextBox.Text.Length);
            this.displayTextBox.SelectionColor = textColor;
            this.displayTextBox.Select(this.displayTextBox.Text.Length, this.displayTextBox.Text.Length);
            this.displayTextBox.ScrollToCaret();
        }

        /// <summary>
        /// Sends a user input string on the Message channel. A message is not sent if
        /// the string is empty.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text.Length > 0)
            {
                broadcast.SendToChannel("UserMessage", string.Format("{0}: {1}", this.Handle, this.inputTextBox.Text));
                this.inputTextBox.Text = "";
            }
        }

        /// <summary>
        /// Adds or removes the Message channel from the messaging API. This effects whether messages 
        /// sent on this channel will be received by the application. Status messages are broadcast 
        /// on the Status channel whenever this setting is changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msgCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (msgCheckBox.Checked)
            {
                listener.RegisterChannel("UserMessage");
                broadcast.SendToChannel("Status", string.Format("{0}: Registering for UserMessage.", this.Handle));
            }
            else
            {
                listener.UnRegisterChannel("UserMessage");
                broadcast.SendToChannel("Status", string.Format("{0}: UnRegistering for UserMessage.", this.Handle));
            }
        
        }

        /// <summary>
        /// Adds or removes the Status channel from the messaging API. This effects whether messages 
        /// sent on this channel will be received by the application. Status messages are broadcast 
        /// on the Status channel whenever this setting is changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (statusCheckBox.Checked)
            {
                listener.RegisterChannel("Status");
                broadcast.SendToChannel("Status", string.Format("{0}: Registering for Status.", this.Handle));
            }
            else
            {
                listener.UnRegisterChannel("Status");
                broadcast.SendToChannel("Status", string.Format("{0}: UnRegistering for Status.", this.Handle));
            }
        }

        /// <summary>
        /// Wire up the enter key to submit a message.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message m, Keys k)
        {
            // allow enter to send message
            if (m.Msg == 256 && k == Keys.Enter)
            {
                if (this.inputTextBox.Text.Length > 0)
                {
                    broadcast.SendToChannel("UserMessage", string.Format("{0}: {1}", this.Handle, this.inputTextBox.Text));
                    this.inputTextBox.Text = "";
                }
                return true;
            }
            return base.ProcessCmdKey(ref m, k);
        }
    }
}