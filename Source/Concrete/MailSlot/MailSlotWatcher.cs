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
using TheCodeKing.Net.Messaging.Helpers;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    internal sealed class MailSlotWatcher : IDisposable
    {
        #region Constants and Fields

        public EventHandler<MailSlotDataReceivedEventArgs> DataReceived;
        private readonly string location;
        private bool disposed;
        private MailSlotReader mailSlotReader;

        #endregion

        #region Constructors and Destructors

        public MailSlotWatcher(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentException("location");
            }
            this.location = location;
        }

        ~MailSlotWatcher()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Starts or stops the class form monitoring the MailSlot, and
        ///   enables or disables the raising of OnChanged events.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get { return mailSlotReader != null; }
            set
            {
                if (value)
                {
                    BeginReading();
                }
                else
                {
                    EndReading();
                }
            }
        }

        #endregion

        #region Public Methods

        public void Dispose(bool disposeManaged)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposeManaged)
                {
                    EndReading();
                }
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        private void BeginReading()
        {
            EndReading();
            Action action = () =>
                                {
                                    // provide system-wide synhronious access to the MailSlot
                                    var helper = new ResourceLocker(location);
                                    if (helper.CreateLock())
                                    {
                                        using (mailSlotReader = new MailSlotReader(location))
                                        {
                                            // blocks whilst reading the mailslot
                                            mailSlotReader.Listen(OnDataReceived);
                                        }
                                        // unlock the resource so another process can take over monitoring
                                        helper.ReleaseLock();
                                    }
                                };
            // async start monitoring the mailslot
            action.BeginInvoke(action.EndInvoke, null);
        }

        private void EndReading()
        {
            // close the read handle to unblock and close the reader thread.
            if (mailSlotReader != null)
            {
                mailSlotReader.Dispose();
            }
        }

        private void OnDataReceived(string data)
        {
            if (DataReceived != null)
            {
                // raise event on data received
                DataReceived(this, new MailSlotDataReceivedEventArgs(data));
            }
        }

        #endregion
    }
}