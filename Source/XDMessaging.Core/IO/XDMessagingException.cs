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
using TheCodeKing.Utils.IO;

namespace XDMessaging.IO
{
    public class XDMessagingException : IocScannerException
    {
        public XDMessagingException(string message)
            : base(message)
        {
        }

        public XDMessagingException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}