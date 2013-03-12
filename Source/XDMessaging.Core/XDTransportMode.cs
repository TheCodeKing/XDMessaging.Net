/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
namespace XDMessaging
{
    /// <summary>
    ///   This defines the tranport modes that can be used for interprocess communication.
    /// </summary>
    public enum XDTransportMode
    {
        /// <summary>
        ///   Use this mode for efficant communication between applications running within the 
        ///   context of a Desktop, such as Windows Forms or WPF.
        /// </summary>
        HighPerformanceUI,
        /// <summary>
        ///   Use Compatibility mode when using non-UI components such as Windows Services.
        /// </summary>
        Compatibility,
        /// <summary>
        ///   Use RemoteNetwork to broadcast to physically seperated servers.
        /// </summary>
        RemoteNetwork
    }
}