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
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace TheCodeKing.Net.Messaging.Helpers
{
    /// <summary>
    ///   A class for limiting system-wide access to a shared resource.
    /// </summary>
    internal sealed class ResourceLocker
    {
        #region Constants and Fields

        private const string baseMutexName = @"Global\";
        private static readonly MutexSecurity mutexSecurity;
        private readonly Mutex mutex;

        #endregion

        #region Constructors and Destructors

        static ResourceLocker()
        {
            mutexSecurity = new MutexSecurity();
            var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            mutexSecurity.SetAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
        }

        /// <summary>
        ///   Creates an instance of ResourceLocker to control access to a shared resource. Access to the resource
        ///   is controlled via a system-wide token (key).
        /// </summary>
        /// <param name = "key"></param>
        public ResourceLocker(string key)
        {
            var resourceKey = string.Concat(baseMutexName, key.Replace(@"\", "_").Replace(".", "_"));
            bool createdNew;
            mutex = new Mutex(false, resourceKey, out createdNew, mutexSecurity);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Block until access is gained to the resource lock, or return false
        ///   if the thread is abandoned.
        /// </summary>
        /// <returns></returns>
        public bool CreateLock()
        {
            try
            {
                return mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                return true;
            }
            catch (ThreadInterruptedException)
            {
                return false;
            }
        }

        public void ReleaseLock()
        {
            try
            {
                mutex.ReleaseMutex();
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(
                    "ReleaseLock called before lock has been aquired. Call CreateLock before releasing.", e);
            }
        }

        #endregion
    }
}