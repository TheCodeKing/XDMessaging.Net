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
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.IO;

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
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Messenger());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            using (StreamWriter log = File.CreateText(Path.Combine(Environment.CurrentDirectory, "XDMessaging.log")))
            {
                log.WriteLine(e.Exception);
            }
            MessageBox.Show("Something when wrong.\r\nSee XDMessaging.log for details.", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.Exit();
        }
    }
}