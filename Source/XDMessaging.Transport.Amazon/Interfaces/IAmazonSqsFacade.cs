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
using Amazon.SQS.Model;

namespace XDMessaging.Transport.Amazon.Interfaces
{
    internal interface IAmazonSqsFacade
    {
        #region Public Methods

        string GetQueueArn(Uri queueUrl);
        Uri CreateOrRetrieveQueue(string name);
        string DeleteMessage(Uri queueUrl, string receiptHandle);
        string DeleteQueue(Uri queueUri);
        IEnumerable<Message> ReadQueue(Uri queueUri);
        string SetSqsPolicyForSnsPublish(Uri queueUrl, string queueArn, string mytopicArn);

        #endregion
    }
}