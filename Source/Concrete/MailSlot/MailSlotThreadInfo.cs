/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
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
using System.Threading;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    ///   A class used for keeping a reference to the current thread 
    ///   and read file handle.
    /// </summary>
    internal sealed class MailSlotThreadInfo
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        /// <param name = "channelName"></param>
        /// <param name = "thread"></param>
        public MailSlotThreadInfo(string channelName, Thread thread)
        {
            Thread = thread;
            FileHandle = IntPtr.Zero;
            ChannelName = channelName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The channel name for this thread.
        /// </summary>
        public string ChannelName { get; private set; }

        /// <summary>
        ///   The file handle used by the current handle.
        /// </summary>
        public IntPtr FileHandle { get; set; }

        /// <summary>
        ///   Indicates whether the current file handle is valid.
        /// </summary>
        public bool HasValidFileHandle
        {
            get { return ((int) FileHandle) > 0; }
        }

        /// <summary>
        ///   The current thread.
        /// </summary>
        public Thread Thread { get; set; }

        #endregion
    }
}