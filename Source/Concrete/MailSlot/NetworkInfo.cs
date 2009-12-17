/*=============================================================================
*
*	(C) Copyright 2007, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expresed or implied. Please do not use commerically without permission.
*
*-----------------------------------------------------------------------------
*	History:
*		11/02/2007	Michael Carlisle				Version 1.0
*		12/12/2009	Michael Carlisle				Version 2.0
*=============================================================================
*/
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheCodeKing.Net.Messaging.Concrete.MailSlot;

namespace TheCodeKing.Net.Messaging.Concrete.MailSlot
{
    /// <summary>
    /// This class is used to obtain the actual workgroup or domain name that the current
    /// user is a member of.
    /// </summary>
    internal sealed class NetworkInformation
    {
        /// <summary>
        /// Instance representing current machine.
        /// </summary>
        private static NetworkInformation local;
        /// <summary>
        /// Represents the current computer name.
        /// </summary>
        private string computerName;
        /// <summary>
        /// Represents the current domain or workgroup.
        /// </summary>
        private string domainName;
        /// <summary>
        /// Determines whether the machine is a member of a workgroup or domain.
        /// </summary>
        private Native.JoinStatus status = Native.JoinStatus.Unknown;

        /// <summary>
        /// Constructor takes the machine name.
        /// </summary>
        /// <param name="computerName"></param>
        public NetworkInformation(string computerName)
        {
            if (computerName == null || 0 == computerName.Length)
            {
                throw new ArgumentNullException("computerName");
            }

            this.computerName = computerName;
            LoadInformation();
        }

        /// <summary>
        /// Constructor initalizes the instance from the current machine.
        /// </summary>
        private NetworkInformation()
        {
            LoadInformation();
        }

        /// <summary>
        /// Providers access to the local machine info instance.
        /// </summary>
        public static NetworkInformation LocalComputer
        {
            get { return local ?? (local = new NetworkInformation()); }
        }

        /// <summary>
        /// Gets the computername that represents the given machine name.
        /// </summary>
        public string ComputerName
        {
            get
            {
                return computerName ?? "(local)";
            }
        }

        /// <summary>
        /// Gets the workgroup or domain name to which the current machine is a member.
        /// </summary>
        public string DomainName
        {
            get { return domainName; }
        }

        /// <summary>
        /// Indicates how the machine is joined to the network.
        /// </summary>
        public Native.JoinStatus Status
        {
            get { return status; }
        }

        /// <summary>
        /// Initializes the information from Win32 APIs.
        /// </summary>
        private void LoadInformation()
        {
            IntPtr pBuffer = IntPtr.Zero;
            Native.JoinStatus status = default(Native.JoinStatus);

            try
            {
                int result = Native.NetGetJoinInformation(computerName, ref pBuffer, ref status);
                if (0 != result)
                {
                    throw new Win32Exception();
                }
                this.status = status;
                this.domainName = Marshal.PtrToStringUni(pBuffer);
            }
            finally
            {
                if (!IntPtr.Zero.Equals(pBuffer))
                {
                    Native.NetApiBufferFree(pBuffer);
                }
            }
        }
    }
}