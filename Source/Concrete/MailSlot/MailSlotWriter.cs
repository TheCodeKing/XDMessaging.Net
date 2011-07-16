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
using System.IO;
using System.Runtime.InteropServices;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    internal sealed class MailSlotWriter
    {
        #region Public Methods

        public uint Write(string mailSlotLocation, byte[] bytes)
        {
            IntPtr writeHandle = Native.CreateFile(mailSlotLocation, FileAccess.Write, FileShare.Read, 0, FileMode.Open,
                                                   0,
                                                   IntPtr.Zero);
            if ((int) writeHandle > 0)
            {
                uint bytesWritten;
                Native.WriteFile(writeHandle, bytes, (uint) bytes.Length, out bytesWritten, IntPtr.Zero);

                // close the file handle
                Native.CloseHandle(writeHandle);
                return bytesWritten;
            }
            int errorCode = Marshal.GetLastWin32Error();
            throw new IOException(string.Format("{0} Unable to open mailslot. Try again later.", errorCode));
        }

        #endregion
    }
}