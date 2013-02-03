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
using System.Collections.Concurrent;
using System.Threading;
using TheCodeKing.Utils.Contract;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    public class ResourceCounter : IResourceCounter
    {
        private static readonly ConcurrentDictionary<string, Semaphore> localTracker =
            new ConcurrentDictionary<string, Semaphore>(StringComparer.InvariantCultureIgnoreCase);

        #region IResourceCounter Members

        public int Decrement(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            Semaphore semaphore;
            if (localTracker.TryGetValue(name, out semaphore))
            {
                try
                {
                    var count = (int.MaxValue - semaphore.Release(1)) - 1;
                    return (count < 0) ? 0 : count;
                }
                catch (SemaphoreFullException e)
                {
                    return 0;
                }
            }
            semaphore = GetSemaphore(name);
            return GetCurrentCount(semaphore);
        }

        public void Increment(string name)
        {
            Validate.That(name).IsNotNullOrEmpty();

            var semaphore = localTracker.GetOrAdd(name, (key) => GetSemaphore(name));
            semaphore.WaitOne();
        }

        #endregion

        private static int GetCurrentCount(Semaphore semaphore)
        {
            semaphore.WaitOne();
            var count = (int.MaxValue - semaphore.Release(1)) - 1;
            return (count < 0) ? 0 : count;
        }

        private static Semaphore GetSemaphore(string name)
        {
            name = name.ToLowerInvariant();
            try
            {
                return Semaphore.OpenExisting(name);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return new Semaphore(int.MaxValue, int.MaxValue, name);
            }
        }
    }
}