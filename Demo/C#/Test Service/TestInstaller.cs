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
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TheCodeKing.Demo
{
    /// <summary>
    /// The demo installer for deploying the test service. Install using the Install.bat provided.
    /// </summary>
    [RunInstaller(true)]
    public partial class TestInstaller : Installer
    {
        public TestInstaller()
        {
            InitializeComponent();

            var si = new ServiceInstaller
                         {
                             ServiceName = "Test Service",
                             DisplayName = "Test Service",
                             StartType = ServiceStartMode.Manual
                         };
            Installers.Add(si);

            var spi = new ServiceProcessInstaller
                          {
                              Account = ServiceAccount.LocalSystem,
                              Password = null,
                              Username = null
                          };
            Installers.Add(spi);
        }
    }
}