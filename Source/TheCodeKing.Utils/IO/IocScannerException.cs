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
using System;
using System.IO;

namespace TheCodeKing.Utils.IO
{
    public class IocScannerException : FileLoadException
    {
        public IocScannerException(string message)
            : base(message)
        {
        }

        public IocScannerException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}