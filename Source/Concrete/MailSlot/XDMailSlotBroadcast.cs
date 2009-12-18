/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied.
*
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    /// Implementation of IXDBroadcast which sends messages over the local network and 
    /// interprocess. Messages sent to a named channel will be received by the first available
    /// instance of IXDListener in the same mode. Each message may only be read by ONE listener
    /// on a single machine. Other listeners will queue until they can take ownership of the listener
    /// singleton.
    /// </summary>
    public sealed class XDMailSlotBroadcast : IXDBroadcast
    {
        /// <summary>
        /// Indicates the base path of the MailSlot.
        /// </summary>
        internal const string SlotLocation = @"\mailslot\xdmessaging\";
        /// <summary>
        /// The unique identifier for the MailSlot.
        /// </summary>
        private readonly string mailSlotIdentifier;
        /// <summary>
        /// Static constructor for initializing the current network domain or workgroup
        /// to which messages will be broadcast.
        /// </summary>
        static XDMailSlotBroadcast()
        {
        }
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal XDMailSlotBroadcast(bool propagateNetwork)
        {
            if (propagateNetwork)
            {
                // address of network MailSlot
                this.mailSlotIdentifier = string.Concat(@"\\*", SlotLocation); 
            }
            else
            {
                // address of local MailSlot
                this.mailSlotIdentifier = string.Concat(@"\\", Environment.MachineName, SlotLocation); 
            }
        }
        /// <summary>
        /// Implementation of IXDBroadcast for sending messages to a named channel on the local network.
        /// </summary>
        /// <param name="channel">The channel on which to send the message.</param>
        /// <param name="message">The message.</param>
        public void SendToChannel(string channelName, string message)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(channelName, "The channel name must be defined");
            }
            if (message == null)
            {
                throw new ArgumentNullException(message, "The messsage packet cannot be null");
            }
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("The channel name may not contain the ':' character.", "channelName");
            }
            // get a handle to the MailSlot
            IntPtr writeHandle = Native.CreateFile(string.Concat(mailSlotIdentifier, channelName), FileAccess.Write, FileShare.Read, 0, FileMode.Open, 0, IntPtr.Zero);
            if ((int)writeHandle > 0)
            {
                // format the message
                string raw = string.Format("{0}:{1}", channelName, message);

                // serialize the data
                byte[] bytes;
                NativeOverlapped overlap = new System.Threading.NativeOverlapped();
                BinaryFormatter b = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    b.Serialize(stream, raw);
                    stream.Flush();
                    int dataSize = (int)stream.Length;

                    // create byte array
                    bytes = new byte[dataSize];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(bytes, 0, dataSize);
                }

                // write the message to the MailSlot
                uint bytesWriten = 0;
                int retryCount = 0;
                while (!Native.WriteFile(writeHandle, bytes, (uint)bytes.Length, ref bytesWriten, ref overlap))
                {
                    // if MailSlot fails, retry up to 10 times
                    if (retryCount > 10)
                    {
                        throw new IOException("Unable to write to mailslot. Try again later.");
                    }
                    retryCount++;
                }
            }
            else
            {
                throw new IOException("Unable to open mailslot. Try again later.");
            }
            // close the handle
            Native.CloseHandle(writeHandle);
        }
    }
}
