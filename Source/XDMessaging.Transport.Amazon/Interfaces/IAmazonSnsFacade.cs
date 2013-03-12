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
namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IAmazonSnsFacade
    {
        #region Public Methods

        string CreateOrRetrieveTopic(string name);
        string PublishMessageToTopic(string topicArn, string subject, string message);
        string SubscribeQueueToTopic(string queueArn, string topicArn);
        string UnsubscribeQueueFromTopic(string subscriptionArn);

        #endregion
    }
}