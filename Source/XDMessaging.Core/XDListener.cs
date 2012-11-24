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
using TheCodeKing.Utils.IoC;
using XDMessaging.Core.IoC;

namespace XDMessaging.Core
{
    /// <summary>
    ///   Class used to create listener implementations which listen for messages broadcast
    ///   on a particular channel.
    /// </summary>
    public static class XDListener
    {
        static XDListener()
        {
            Container = SimpleIoCContainerBootstrapper.GetInstance();
        }

        public static IocContainer Container { get; set; }

        #region Delegates

        /// <summary>
        ///   The delegate used for handling cross AppDomain communications.
        /// </summary>
        /// <param name = "sender">The event sender.</param>
        /// <param name = "e">The event args containing the DataGram data.</param>
        public delegate void XDMessageHandler(object sender, XDMessageEventArgs e);

        #endregion

        #region Public Methods

        /// <summary>
        ///   Creates an concrete implementation of IXDListener to listen for messages using
        ///   either a specific XDTransportMode.
        /// </summary>
        /// <param name="transportMode">The mode to use when listening to broadcasts.</param>
        /// <returns></returns>
        public static IXDListener CreateListener(XDTransportMode transportMode)
        {
            return Container.Resolve<IXDListener>(transportMode.ToString());
        }

        #endregion
    }
}