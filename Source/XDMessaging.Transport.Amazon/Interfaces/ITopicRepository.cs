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
using XDMessaging.Transport.Amazon.Entities;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface ITopicRepository
    {
        Topic GetTopic(string channelName);
    }
}