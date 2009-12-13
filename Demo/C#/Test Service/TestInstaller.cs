﻿/*=============================================================================
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
*		12/12/2009	Michael Carlisle				Version 1.0
*=============================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Test_Service
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

            ServiceInstaller si = new ServiceInstaller();
            si.ServiceName = "Test Service";
            si.DisplayName = "Test Service";
            si.StartType = ServiceStartMode.Manual;
            this.Installers.Add(si);

            ServiceProcessInstaller spi = new ServiceProcessInstaller();
            spi.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            spi.Password = null;
            spi.Username = null;
            this.Installers.Add(spi);
        }
    }
}
