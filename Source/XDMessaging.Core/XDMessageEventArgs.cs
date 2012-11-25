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
using XDMessaging.Messages;

namespace XDMessaging
{
    /// <summary>
    ///   The event args used by the message handler. This enables DataGram data 
    ///   to be passed to the handler.
    /// </summary>
    public sealed class XDMessageEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   Stores the DataGram containing message and channel data.
        /// </summary>
        private readonly DataGram dataGram;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor used to create a new instance from a DataGram struct.
        /// </summary>
        /// <param name = "dataGram">The DataGram instance.</param>
        public XDMessageEventArgs(DataGram dataGram)
        {
            this.dataGram = dataGram;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the DataGram associated with this instance.
        /// </summary>
        public DataGram DataGram
        {
            get { return dataGram; }
        }

        #endregion
    }
}