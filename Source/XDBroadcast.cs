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
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*       08/09/2007  Michael Carlisle                Version 1.1
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using TheCodeKing.Utils;
using TheCodeKing.Native;

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// Class used to broadcast messages to other applications listening
    /// on a particular channel.
    /// </summary>
    public static class XDBroadcast
    {
        /// <summary>
        /// The API used to broadcast messages to a channel, and other applications that
        /// may be listening.
        /// </summary>
        /// <param name="channel">The channel name to broadcast on.</param>
        /// <param name="message">The string message data.</param>
        public static void SendToChannel(string channel, string message)
        {
            // create a DataGram instance
            DataGram dataGram = new DataGram(channel, message);
            // Allocate the DataGram to a memory address contained in COPYDATASTRUCT
            Win32.COPYDATASTRUCT dataStruct = dataGram.ToStruct();
            // Use a filter with the EnumWindows class to get a list of windows containing
            // a property name that matches the destination channel. These are the listening
            // applications.
            WindowEnumFilter filter = new WindowEnumFilter(XDListener.GetChannelKey(channel));
            WindowsEnum winEnum = new WindowsEnum(filter.WindowFilterHandler);
            foreach (IntPtr hWnd in winEnum.Enumerate(Win32.GetDesktopWindow()))
            {
                IntPtr outPtr = IntPtr.Zero;
                // For each listening window, send the message data. Return if hang or unresponsive within 1 sec.
                Win32.SendMessageTimeout(hWnd, Win32.WM_COPYDATA, (int)IntPtr.Zero, ref dataStruct, Win32.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out outPtr);
            }
            // free the memory
            dataGram.Dispose();
        }
    }
}
