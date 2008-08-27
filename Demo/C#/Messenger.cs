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
        /// The instance used to send and receive cross AppDomain messages.
        /// </summary>
        private XDListener listener;
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
            // set the handle id in the form title
            this.Text += string.Format("Window Id: {0}", this.Handle);
            // create an instance of the listener object
            listener = new XDListener();
            // attach the message handler
            listener.MessageReceived += new XDListener.XDMessageHandler(listener_MessageReceived);
            // register the channels we want to listen on
            listener.RegisterChannel("Status");
            listener.RegisterChannel("UserMessage");
            // broadcast on the status channel that we have loaded
            XDBroadcast.SendToChannel("Status", string.Format("Window {0} created!",this.Handle));

       }

        /// <summary>
        /// The closing overrride used to broadcast on the status channel that the window is
        /// closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            XDBroadcast.SendToChannel("Status", string.Format("Window {0} closing!",this.Handle));
        }
        /// <summary>
        /// The delegate which processes all cross AppDomain messages and writes them to screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listener_MessageReceived(object sender, XDMessageEventArgs e)
        {
            Color textColor;
            switch (e.DataGram.Channel.ToLower())
            {
                case "status":
                    textColor = Color.Green;
                    break;
                default:
                    textColor = Color.Blue;
                    break;
            }
            string msg = string.Format("{0}: {1}\r\n", e.DataGram.Channel, e.DataGram.Message);
            this.displayTextBox.AppendText(msg);
            this.displayTextBox.Select(this.displayTextBox.Text.Length - msg.Length+1, this.displayTextBox.Text.Length);
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
                XDBroadcast.SendToChannel("UserMessage", string.Format("{0}: {1}", this.Handle, this.inputTextBox.Text));
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
                XDBroadcast.SendToChannel("Status", string.Format("{0}: Registering for UserMessage.", this.Handle));
            }
            else
            {
                listener.UnRegisterChannel("UserMessage");
                XDBroadcast.SendToChannel("Status", string.Format("{0}: UnRegistering for UserMessage.", this.Handle));
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
                XDBroadcast.SendToChannel("Status", string.Format("{0}: Registering for Status.", this.Handle));
            }
            else
            {
                listener.UnRegisterChannel("Status");
                XDBroadcast.SendToChannel("Status", string.Format("{0}: UnRegistering for Status.", this.Handle));
            }
        }
    }
}