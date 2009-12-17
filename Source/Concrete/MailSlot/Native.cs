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
*       12/12/2009  Michael Carlisle                Version 2.0
 *                  Added XDIOStream implementation which can be used from Windows Services.
*=============================================================================
*/
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    /// The native Win32 APIs used by the library.
    /// </summary>
    internal static class Native
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateMailslot(string lpName, uint nMaxMessageSize,
           uint lReadTimeout, IntPtr lpSecurityAttributes);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            int securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
                IntPtr hFile,
                byte[] lpBuffer,
                uint nNumberOfBytesToWrite,
                [In] ref uint lpNumberOfBytesWritten,
                [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool GetMailslotInfo(IntPtr hMailslot, int lpMaxMessageSize,
           int lpNextSize, ref int lpMessageCount, IntPtr lpReadTimeout);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        public enum JoinStatus
        {
            Unknown = 0,
            UnJoined = 1,
            Workgroup = 2,
            Domain = 3
        }

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetGetJoinInformation(string computerName, ref IntPtr buffer, ref JoinStatus status);

        [DllImport("netapi32.dll", SetLastError = true)]
        public static extern int NetApiBufferFree(IntPtr buffer);
    }

}
