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
using System.Reflection;

namespace TheCodeKing.Utils.IoC
{
    public interface IocScanner
    {
        /// <summary>
        ///   Scan embedded resources for assembies.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        void ScanEmbeddedResources(Assembly assembly);

        /// <summary>
        /// Scan all loaded assemblies.
        /// </summary>
        void ScanLoadedAssemblies();

        /// <summary>
        /// Scan all loaded assemblies.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use when scanning assemblies.</param>
        void ScanLoadedAssemblies(string searchPattern);

        /// <summary>
        ///   Scan assembly.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        void ScanAssembly(Assembly assembly);

        /// <summary>
        ///   Scan assemblies at the same location as the executing assembly for concrete XD implementations.
        /// </summary>
        void ScanAllAssemblies();

        /// <summary>
        ///   Scan assemblies at the same location as the executing assembly for concrete XD implementations.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use when scanning assemblies.</param>
        void ScanAllAssemblies(string searchPattern);

        /// <summary>
        ///   Scan assemblies at given location for concrete XD implementations.
        /// </summary>
        /// <param name = "location">The search location.</param>
        void ScanAssemblies(string location);

        /// <summary>
        ///   Scan assemblies at location using the given serach pattern.
        /// </summary>
        /// <param name = "location">The search location.</param>
        /// <param name = "searchPattern">The search pattern.</param>
        void ScanAssemblies(string location, string searchPattern);
    }
}