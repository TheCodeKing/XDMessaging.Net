using System;
using System.Collections.Concurrent;
using System.Threading;
using Conditions;
using XDMessaging.Transport.Amazon.Interfaces;

namespace XDMessaging.Transport.Amazon
{
    internal class ResourceCounter : IResourceCounter
    {
        private static readonly ConcurrentDictionary<string, Semaphore> LocalTracker =
            new ConcurrentDictionary<string, Semaphore>(StringComparer.InvariantCultureIgnoreCase);

        public int Decrement(string name)
        {
            name.Requires("name").IsNotNullOrWhiteSpace();

            Semaphore semaphore;
            if (LocalTracker.TryGetValue(name, out semaphore))
            {
                try
                {
                    var count = int.MaxValue - semaphore.Release(1) - 1;
                    return count < 0 ? 0 : count;
                }
                catch (SemaphoreFullException)
                {
                    return 0;
                }
            }
            semaphore = GetSemaphore(name);
            return GetCurrentCount(semaphore);
        }

        public void Increment(string name)
        {
            name.Requires("name").IsNotNullOrWhiteSpace();

            var semaphore = LocalTracker.GetOrAdd(name, key => GetSemaphore(name));
            semaphore.WaitOne();
        }

        private static int GetCurrentCount(Semaphore semaphore)
        {
            semaphore.WaitOne();
            var count = int.MaxValue - semaphore.Release(1) - 1;
            return count < 0 ? 0 : count;
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