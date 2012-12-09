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
using System.Threading;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    public class ResourceCounter : IResourceCounter
    {
        public int Decrement(string name)
        {
            try
            {
                var semephore = GetSemephore(name);
                var count =  (int.MaxValue - semephore.Release())-1;
                return count;
            }
            catch (SemaphoreFullException)
            {
                return 0;
            }
        }

        public void Increment(string name)
        {
            var semephore = GetSemephore(name);
            semephore.WaitOne();
        }

        private static Semaphore GetSemephore(string name)
        {
            return new Semaphore(int.MaxValue, int.MaxValue, name);
        }
    }
}