using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XDMessaging;
using XDMessaging.Entities.Amazon;
using XDMessaging.Messages;

namespace Messenger
{
    public partial class Messenger : Form
    {
        private IXDBroadcaster broadcast;
        private XDMessagingClient client;
        private IXDListener listener;
        private string uniqueInstanceName;

        public Messenger()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var message = $"{Handle} is shutting down";
            broadcast.SendToChannel("Status", message);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            uniqueInstanceName = $"{Environment.MachineName}-{Handle}";
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

            Text += $" - {uniqueInstanceName}";

            InitializeMode(XDTransportMode.HighPerformanceUI);

            var message = $"{uniqueInstanceName} has joined";
            broadcast.SendToChannel("Status", message);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys k)
        {
            if (m.Msg != 256 || k != Keys.Enter)
            {
                return base.ProcessCmdKey(ref m, k);
            }

            SendMessage();
            return true;
        }

        private void Channel1CheckedChanged(object sender, EventArgs e)
        {
            if (channel1Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel1");
                var message = $"{uniqueInstanceName} is registering Channel1.";
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("BinaryChannel1");
                var message = $"{uniqueInstanceName} is unregistering Channel1.";
                broadcast.SendToChannel("Status", message);
            }
        }

        private void Channel2CheckedChanged(object sender, EventArgs e)
        {
            if (channel2Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel2");
                var message = $"{uniqueInstanceName} is registering Channel2.";
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("BinaryChannel2");
                var message = $"{uniqueInstanceName} is unregistering Channel2.";
                broadcast.SendToChannel("Status", message);
            }
        }

        private void InitializeMode(XDTransportMode mode)
        {
            listener?.Dispose();
            listener = client.Listeners.GetListenerForMode(mode);

            listener.MessageReceived += OnMessageReceived;

            if (statusCheckBox.Checked)
            {
                listener.RegisterChannel("Status");
            }

            if (channel1Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel1");
            }

            if (channel2Check.Checked)
            {
                listener.RegisterChannel("BinaryChannel2");
            }

            if (broadcast != null)
            {
                var message = $"{uniqueInstanceName} is changing mode to {mode}";
                broadcast.SendToChannel("Status", message);
            }

            broadcast = client.Broadcasters.GetBroadcasterForMode(mode, propagateCheck.Checked);
        }

        private void ModeCheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Checked)
            {
                SetMode();
            }
        }

        private void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            if (InvokeRequired && !IsDisposed)
            {
                try
                {
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
                        UpdateDisplayText(e.DataGram.Channel, e.DataGram.Message);
                        break;
                    default:
                        if (e.DataGram.AssemblyQualifiedName == typeof (FormattedUserMessage).AssemblyQualifiedName)
                        {
                            TypedDataGram<FormattedUserMessage> typedDataGram = e.DataGram;
                            UpdateDisplayText(typedDataGram.Channel, typedDataGram.Message.FormattedTextMessage);
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Unknown message type: {0}",
                                e.DataGram.AssemblyQualifiedName));
                        }
                        break;
                }
            }
        }

        private void PropagateCheckCheckedChanged(object sender, EventArgs e)
        {
            var message = propagateCheck.Checked
                ? "Messages will be propagated across the network to all listeners.\r\n"
                : "Message are restricted to the current machine.\r\n";
            UpdateDisplayText(message, Color.Red);
            SetMode();
        }

        private void SendBtnClick(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (inputTextBox.Text.Length > 0)
            {
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

        private void StatusChannelCheckedChanged(object sender, EventArgs e)
        {
            if (statusCheckBox.Checked)
            {
                listener.RegisterChannel("Status");
                var message = $"{uniqueInstanceName} is registering Status.";
                broadcast.SendToChannel("Status", message);
            }
            else
            {
                listener.UnRegisterChannel("Status");
                var message = $"{uniqueInstanceName} is unregistering Status.";
                broadcast.SendToChannel("Status", message);
            }
        }

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
            var msg = $"{channelName}: {displayText}\r\n";
            UpdateDisplayText(msg, textColor);
        }

        private void UpdateDisplayText(string message, Color textColor)
        {
            if (IsDisposed)
            {
                return;
            }

            displayTextBox.AppendText(message);
            displayTextBox.Select(displayTextBox.Text.Length - message.Length + 1, displayTextBox.Text.Length);
            displayTextBox.SelectionColor = textColor;
            displayTextBox.Select(displayTextBox.Text.Length, displayTextBox.Text.Length);
            displayTextBox.ScrollToCaret();
        }
    }
}