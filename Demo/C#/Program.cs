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
*=============================================================================
*/
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheCodeKing.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Messenger());
        }
    }
}