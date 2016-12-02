using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using PS_Console_Test.Helpers;
using PS_Console_Test.Handlers;
using PS_Console_Test.Controls;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;

namespace PS_Console_Test {
   public partial class Form1 : Form {
      public static Thread PowershellConsoleWriteThread;
      private CancellationToken m_cancellationToken;
      public Form1() {
         InitializeComponent();
      }

      private void btnStart_Click(object sender, EventArgs e) {
         Task signinTask = Task.Factory.StartNew(() => {
            btnStart.HandleInvokeRequired(__btnStart => {
               __btnStart.Enabled = false;
            });
            string pWD = Directory.GetCurrentDirectory();
            Hashtable pArgs = new Hashtable();
            //Do stuff here:
            PowershellConsoleWriteThread = new Thread(() => m_powershellInteractiveControl1.ExecuteAsynchronously("cd " + pWD + ";" + pWD + "\\TestMe.ps1", pArgs));

            PowershellConsoleWriteThread.IsBackground = true;
            PowershellConsoleWriteThread.Priority = ThreadPriority.AboveNormal;
            PowershellConsoleWriteThread.Start();

            while (PowershellConsoleWriteThread.IsAlive) {
               Thread.Sleep(50);
               m_cancellationToken.ThrowIfCancellationRequested();
               // Poll on this property if you have to do
               // other cleanup before throwing.                     
               if (m_cancellationToken.IsCancellationRequested) {
                  // Clean up here, then...
                  m_cancellationToken.ThrowIfCancellationRequested();
               }
            }
            //Inform in the statusbar what we are doing, and wait for it to finish            
         }, m_cancellationToken).ContinueWith((taskResult) => {            
            btnStart.HandleInvokeRequired(__btnStart => {
               __btnStart.Enabled = true;
            });
         });
      }
   }
}
