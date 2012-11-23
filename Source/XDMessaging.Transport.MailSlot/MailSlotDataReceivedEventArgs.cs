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

namespace XDMessaging.Core.Concrete.MailSlot
{
    internal sealed class MailSlotDataReceivedEventArgs : EventArgs
    {
        private readonly string data;

        public MailSlotDataReceivedEventArgs(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("Cannot be null or empty", "data");
            }
            this.data = data;
        }

        public string Data
        {
            get { return data; }
        }
    }
}