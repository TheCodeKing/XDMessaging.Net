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
    public class XDMailSlotBroadcast : IXDBroadcast
    {
        /// <summary>
        /// Indicates the base path of the MailSlot.
        /// </summary>
        internal const string SlotLocation = @"\mailslot\xdmessaging\";
        /// <summary>
        /// The timeout period after which messages are deleted. 
        /// </summary>
        private const uint messageTimeoutMilliseconds = 5000;
        /// <summary>
        /// The unique identifier for the MailSlot.
        /// </summary>
        private static readonly string mailSlotIdentifier;

        /// <summary>
        /// Static constructor for initializing the current network domain or workgroup
        /// to which messages will be broadcast.
        /// </summary>
        static XDMailSlotBroadcast()
        {
            // this broadcasts to the connected workgroup or domain
            mailSlotIdentifier = string.Concat(@"\\", NetworkInformation.LocalComputer.DomainName, SlotLocation);
        }
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal XDMailSlotBroadcast()
        {
        }
        /// <summary>
        /// Implementation of IXDBroadcast for sending messages to a named channel on the local network.
        /// </summary>
        /// <param name="channel">The channel on which to send the message.</param>
        /// <param name="message">The message.</param>
        public void SendToChannel(string channel, string message)
        {
            // get a handle to the MailSlot
            IntPtr writeHandle = Native.CreateFile(string.Concat(mailSlotIdentifier, channel), FileAccess.Write, FileShare.Read, 0, FileMode.Open, 0, IntPtr.Zero);
            if ((int)writeHandle > 0)
            {
                // format the message
                string raw = string.Format("{0}:{1}", channel, message);

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
                if (!Native.WriteFile(writeHandle, bytes, (uint)bytes.Length, ref bytesWriten, ref overlap))
                {
                    throw new IOException("Unable to write to mailslot. Try again later.");
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
