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
using TheCodeKing.Net.Messaging.Concrete.IOStream;
using TheCodeKing.Net.Messaging.Concrete.MailSlot;
using TheCodeKing.Net.Messaging.Concrete.WindowsMessaging;

namespace TheCodeKing.Net.Messaging
{
    /// <summary>
    /// Class used to create listener implementations which listen for messages broadcast
    /// on a particular channel.
    /// </summary>
    public static class XDListener
    {
        #region Delegates

        /// <summary>
        /// The delegate used for handling cross AppDomain communications.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args containing the DataGram data.</param>
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        #endregion

        /// <summary>
        /// Creates an concrete implementation of IXDListener to listen for messages using
        /// either a specific XDTransportMode.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public static IXDListener CreateListener(XDTransportMode transport)
        {
            switch (transport)
            {
                case XDTransportMode.IOStream:
                    return new XDIOStreamListener();
                case XDTransportMode.MailSlot:
                    return new XDMailSlotListener();
                default:
                    return new XDWinMsgListener();
            }
        }
    }
}