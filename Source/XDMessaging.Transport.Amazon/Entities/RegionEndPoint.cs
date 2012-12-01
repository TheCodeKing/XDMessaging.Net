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
using System.Reflection;
using Amazon;

namespace XDMessaging.Transport.Amazon.Entities
{
    internal static class RegionEndPointExtensions
    {
        #region Public Methods

        public static RegionEndpoint ToRegionEndpoint(this RegionEndPoint region)
        {
            var name = Convert.ToString(region);
            var field = typeof(RegionEndpoint).GetField(name,
                                                         BindingFlags.Static | BindingFlags.Public |
                                                         BindingFlags.GetField);
            return field == null ? null : field.GetValue(null) as RegionEndpoint;
        }

        public static RegionEndpoint ToRegionEndpoint(this RegionEndPoint? region)
        {
            if (!region.HasValue)
            {
                return default(RegionEndpoint);
            }
            var name = Convert.ToString(region);
            var field = typeof(RegionEndpoint).GetField(name,
                                                         BindingFlags.Static | BindingFlags.Public |
                                                         BindingFlags.GetField);
            return field == null ? null : field.GetValue(null) as RegionEndpoint;
        }

        #endregion
    }

    /// <summary>
    /// Due to embedded AWS assembly, the actual RegionEndpoint is not visible. This enum maps to the 
    /// AWS class.
    /// </summary>
    public enum RegionEndPoint
    {
// ReSharper disable InconsistentNaming
        APNortheast1,
        APSoutheast1,
        APSoutheast2,
        EUWest1,
        SAEast1,
        USEast1,
        USGovCloudWest1,
        USWest1,
        USWest2
// ReSharper restore InconsistentNaming
    }
}