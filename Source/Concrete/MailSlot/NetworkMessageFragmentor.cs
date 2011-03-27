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
using System.Collections.Generic;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    internal sealed class NetworkMessageFragmentor
    {
        #region Constants and Fields

        private readonly int baseLength;
        private readonly string channel;
        private readonly string message;
        private readonly Guid uniqueId;

        #endregion

        #region Constructors and Destructors

        public NetworkMessageFragmentor(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentException("channel");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("message");
            }
            uniqueId = Guid.NewGuid();
            this.channel = channel;
            this.message = message;
            baseLength =
                new MailSlotDataGram(uniqueId, ushort.MaxValue - 1, ushort.MaxValue, channel, string.Empty).ToBytes().
                    Length;
        }

        #endregion

        #region Public Methods

        public IEnumerable<MailSlotDataGram> GetFragments(int maxBytes)
        {
            // assume worst case where each char is 2 bytes
            var maxByteSize = (maxBytes - baseLength);
            // this gives us max chars for each chunk
            int chunk = maxByteSize/2;
            int offset = 0;
            var count = (ushort) Math.Ceiling(decimal.Divide(message.Length, chunk));

            for (ushort i = 0; i < count; i++)
            {
                int size = Math.Min(chunk, message.Length - offset);
                yield return new MailSlotDataGram(uniqueId, i, count, channel, message.Substring(offset, size));
                offset += size;
            }
        }

        #endregion
    }
}