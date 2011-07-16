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
using System.Text;
using System.Threading;
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    internal sealed class MailSlotReader : IDisposable
    {
        #region Constants and Fields

        private readonly string mailSlotLocation;
        private readonly FixedSizeCollection messageBuffer;
        private bool disposed;
        private IntPtr fileHandle;

        #endregion

        #region Constructors and Destructors

        public MailSlotReader(string mailSlotLocation)
        {
            if (string.IsNullOrEmpty(mailSlotLocation))
            {
                throw new ArgumentException("The mailSlotLocation cannot be null or empty.", "mailSlotLocation");
            }
            this.mailSlotLocation = mailSlotLocation;
            messageBuffer = new FixedSizeCollection(15);
        }

        #endregion

        #region Public Methods

        public void Listen(Action<string> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            const int bytesToRead = 424;
            var buffer = new byte[bytesToRead];

            while (!disposed)
            {
                while (!disposed && (int) fileHandle <= 0)
                {
                    fileHandle = GetFileHandle(mailSlotLocation);
                    // of open failed try again
                    if ((int) fileHandle <= 0)
                    {
                        try
                        {
                            Thread.Sleep(1000);
                        }
                        catch (ThreadInterruptedException)
                        {
                        }
                    }
                }

                uint bytesRead;
                string last = null;
                while (!disposed && Native.ReadFile(fileHandle, buffer, bytesToRead, out bytesRead, IntPtr.Zero))
                {
                    var encoding = new UTF8Encoding();
                    string message = encoding.GetString(buffer, 0, (int) bytesRead);
                    // remove dups
                    if (!string.IsNullOrEmpty(message) && message != last && messageBuffer.TryAddValue(message))
                    {
                        callback(message);
                    }
                    last = message;
                }
                // error so clode handle and re-open
                Native.CloseHandle(fileHandle);
                fileHandle = IntPtr.Zero;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if ((int) fileHandle > 0)
                {
                    Native.CloseHandle(fileHandle);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        private static IntPtr GetFileHandle(string mailSlotLocation)
        {
            return Native.CreateMailslot(mailSlotLocation, 0, Native.MAILSLOT_WAIT_FOREVER, IntPtr.Zero);
        }

        #endregion
    }
}