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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    ///   Implementation of IXDBroadcast which sends messages over the local network and 
    ///   interprocess. Messages sent to a named channel will be received by the first available
    ///   instance of IXDListener in the same mode. Each message may only be read by ONE listener
    ///   on a single machine. Other listeners will queue until they can take ownership of the listener
    ///   singleton.
    /// </summary>
    public sealed class XDMailSlotBroadcast : IXDBroadcast
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates the base path of the MailSlot.
        /// </summary>
        internal const string SlotLocation = @"\mailslot\xdmessaging\";

        /// <summary>
        ///   The unique identifier for the MailSlot.
        /// </summary>
        private readonly string mailSlotIdentifier;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Internal constructor.
        /// </summary>
        internal XDMailSlotBroadcast(bool propagateNetwork)
        {
            mailSlotIdentifier = propagateNetwork
                                     ? string.Concat(@"\\*", SlotLocation)
                                     : string.Concat(@"\\", Environment.MachineName, SlotLocation);
        }

        #endregion

        #region Implemented Interfaces

        #region IXDBroadcast

        /// <summary>
        ///   Implementation of IXDBroadcast for sending messages to a named channel on the local network.
        /// </summary>
        /// <param name = "channelName"></param>
        /// <param name = "message">The message.</param>
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

            //synchronize writes to mailslot
            string mailSlotId = string.Concat(mailSlotIdentifier, channelName);

            IntPtr writeHandle = Native.CreateFile(mailSlotId, FileAccess.Write, FileShare.Read, 0, FileMode.Open, 0,
                                                   IntPtr.Zero);
            if ((int) writeHandle > 0)
            {
                // format the message, and add a unique id to avoid duplicates in listener instances
                // this is because mailslot is sent once for every protocol (TCP/IP NetBEU)
                var dataGram = new MailSlotDataGram(Guid.NewGuid(), channelName, message);

                string raw = dataGram.ToString();

                // serialize the data
                byte[] bytes;
                uint bytesWritten = 0;
                var b = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    b.Serialize(stream, raw);
                    // create byte array
                    bytes = stream.GetBuffer();
                }
                Native.WriteFile(writeHandle, bytes, (uint) bytes.Length, ref bytesWritten, IntPtr.Zero);

                // close the file handle
                Native.CloseHandle(writeHandle);
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new IOException(string.Format("{0} Unable to open mailslot. Try again later.", errorCode));
            }
        }

        #endregion

        #endregion
    }
}