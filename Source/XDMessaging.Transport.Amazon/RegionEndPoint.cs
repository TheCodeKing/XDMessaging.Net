using Amazon;
using XDMessaging.Core.IoC;

namespace XDMessaging.Transport.Amazon
{
    public class RegionEndPoint
    {
        static RegionEndPoint()
        {
            var container = SimpleIoCContainerBootstrapper.GetInstance();
            container.Scan.ScanEmbeddedAssemblies(typeof(AmazonAccountSettings).Assembly);
        }

        #region Constants and Fields

        /// <summary>
        ///   The Asia Pacific (Tokyo) endpoint.
        /// </summary>
        public static readonly RegionEndPoint APNortheast1 = RegionEndpoint.APNortheast1;

        /// <summary>
        ///   The Asia Pacific (Singapore) endpoint.
        /// </summary>
        public static readonly RegionEndPoint APSoutheast1 = RegionEndpoint.APSoutheast1;

        /// <summary>
        ///   The Asia Pacific (Sydney) endpoint.
        /// </summary>
        public static readonly RegionEndPoint APSoutheast2 = RegionEndpoint.APSoutheast2;

        /// <summary>
        ///   The EU West (Ireland) endpoint.
        /// </summary>
        public static readonly RegionEndPoint EUWest1 = RegionEndpoint.EUWest1;

        /// <summary>
        ///   The South America (Sao Paulo)endpoint.
        /// </summary>
        public static readonly RegionEndPoint SAEast1 = RegionEndpoint.SAEast1;

        /// <summary>
        ///   The US East (Virginia) endpoint.
        /// </summary>
        public static readonly RegionEndPoint USEast1 = RegionEndpoint.USEast1;

        /// <summary>
        ///   The US GovCloud West (Oregon)endpoint.
        /// </summary>
        public static readonly RegionEndPoint USGovCloudWest1 = RegionEndpoint.USGovCloudWest1;

        /// <summary>
        ///   The US West (N. California) endpoint.
        /// </summary>
        public static readonly RegionEndPoint USWest1 = RegionEndpoint.USWest1;

        /// <summary>
        ///   The US West (Oregon) endpoint.
        /// </summary>
        public static readonly RegionEndPoint USWest2 = RegionEndpoint.USWest2;

        private readonly RegionEndpoint amazonEndPoint;

        #endregion

        #region Constructors and Destructors

        private RegionEndPoint(RegionEndpoint amazonEndPoint)
        {
            this.amazonEndPoint = amazonEndPoint;
        }

        #endregion

        #region Operators

        public static implicit operator RegionEndpoint(RegionEndPoint regionEndPoint)
        {
            return regionEndPoint == null ? null : regionEndPoint.amazonEndPoint;
        }

        public static implicit operator RegionEndPoint(RegionEndpoint regionEndPoint)
        {
            return regionEndPoint == null ? null : new RegionEndPoint(regionEndPoint);
        }

        #endregion
    }
}