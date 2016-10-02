using System;
using System.Reflection;
using Amazon;

namespace XDMessaging.Entities.Amazon
{
    internal static class RegionEndPointExtensions
    {
        public static RegionEndpoint ToRegionEndpoint(this RegionEndPoint region)
        {
            var name = Convert.ToString(region);
            var field = typeof (RegionEndpoint).GetField(name,
                BindingFlags.Static | BindingFlags.Public |
                BindingFlags.GetField);

            return field?.GetValue(null) as RegionEndpoint;
        }

        public static RegionEndpoint ToRegionEndpoint(this RegionEndPoint? region)
        {
            if (!region.HasValue)
            {
                return default(RegionEndpoint);
            }
            var name = Convert.ToString(region);
            var field = typeof (RegionEndpoint).GetField(name,
                BindingFlags.Static | BindingFlags.Public |
                BindingFlags.GetField);

            return field?.GetValue(null) as RegionEndpoint;
        }
    }

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