using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PS_Console_Test.Handlers;
using PS_Console_Test.Helpers;
using PS_Console_Test.Interfaces;
using Microsoft.PowerShell.Host;
using System.IO;
using System.Threading;

namespace PS_Console_Test.Controls {
   public partial class PowershellInteractiveControl : UserControl, IPSControl {
      /// <summary>
      /// The printer that does the printing to console
      /// </summary>
      public PanelHandler PowerShellConsolePrinter;
      /// <summary>
      /// Let subscribers know when visibility of the control is being changed
      /// </summary>
      public event Action<bool> ControlVisibleChangeEvent = (bool visibleState) => { };
      /// <summary>
      /// Executes when powershell command(s) are done
      /// </summary>
      public event Action WhenCommandIsDone = () => { };
      /// <summary>
      /// Let subscribers know when Ctrl-C was pressed
      /// </summary>
      public event KeyEventHandler CtrlCPressed = (object sender, KeyEventArgs e) => { };
      /// <summary>
      /// Handle Enter Key Events
      /// </summary>
      public event KeyEventHandler EnterKeyHandler = (object sender, KeyEventArgs e) => { };
      private static Thread m_PowershellConsoleListenerThread;

      public PowershellInteractiveControl() {
         InitializeComponent();
         PowerShellConsolePrinter = new PanelHandler(txtPowerShellOutput);
         PowerShellConsolePrinter.StatusUpdateList = new List<string>() { "Setting Network configuration", "Setting up Admin PC", "Evaluating prerequisites",
            "Installing required software", "Deploying virtual machines..", "Setting up virtual machines..", "Checking setup after install.." };
         PowerShellConsolePrinter.StatusSuccessString = "If no errors are found, you are good to go!";
         PowerShellConsolePrinter.PrintDelay = 1;
         PowerShellConsolePrinter.TextColor = Color.White;
         Load += PowershellInteractiveControl_Load;
      }

      private void PowershellInteractiveControl_Load(object sender, EventArgs e) {
         PowerShellConsolePrinter.WriteDelayedMessage("       Windows PowerShell Interactive Console Host\n");
         PowerShellConsolePrinter.WriteDelayedMessage("    ==================================================\n");
         PowerShellConsolePrinter.WriteDelayedMessage("       Interactvity activates after script execution.\n\n");
      }
      public void ExecuteAsynchronously(string commandOrFileString, Hashtable args) {
         StringBuilder commandString = new StringBuilder();
         string filePath = commandOrFileString.Substring(0, commandOrFileString.LastIndexOf("ps1") + 3);
         if (File.Exists(filePath)) {
            string tempCommandString = String.Empty;
            using (StreamReader fileReader = new StreamReader(filePath)) {
               tempCommandString = fileReader.ReadToEnd();
            }
            foreach (DictionaryEntry entry in args) {
               commandString.Append("$" + entry.Key + "=\"" + entry.Value + "\";");
            }
            commandString.Append(tempCommandString);
         }
         else {
            commandString.Append(commandOrFileString);
         }
         this.ExecuteAsynchronously(commandString.ToString());
      }
      public void ExecuteAsynchronously(string commandString) {         
         PSListenerConsole psConsoleListener = new PSListenerConsole(PowerShellConsolePrinter);        
         //Start the listener to listen for commands:
         try { 
            m_PowershellConsoleListenerThread = new Thread(() => {
               psConsoleListener.WhenCommandIsDone += () => {
                  this.WhenCommandIsDone();
               };
               psConsoleListener.Run();
            });
            m_PowershellConsoleListenerThread.IsBackground = true;
            m_PowershellConsoleListenerThread.Priority = ThreadPriority.BelowNormal;
            m_PowershellConsoleListenerThread.Start();
         } catch (AggregateException ae) {
            foreach (var e in ae.InnerExceptions) {
               PowerShellConsolePrinter.WriteDelayedMessage("Error: " + e.Message.ToString() + "\n");

               //Can be removed
               MessageBox.Show(e.Message, "Error Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
         }

         try {
            psConsoleListener.ExecuteAsynchronously(commandString);
            //When the commands are considered done, notify!
         }
         catch (Exception e) {
            PowerShellConsolePrinter.WriteDelayedMessage("Error: " + e.Message.ToString() + "\n");
            PowerShellConsolePrinter.SetPanelStatus("Error: " + e.Message.ToString(), Status.Failed);

            //Can be removed
            MessageBox.Show(e.Message, "Error Message",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
         }
      }

      private void btnClose_Click(object sender, EventArgs e) {
         this.Visible = !this.Visible;
         ControlVisibleChangeEvent(this.Visible);
      }

      private void txtPowerShellOutput_Clicked(object sender, EventArgs e) {
         PowerShellConsolePrinter.SetCursorAtLastLine();
      }

      private void txtPowerShellOutput_KeyUp(object sender, KeyEventArgs e) {
         if (e.KeyData == (Keys.Control | Keys.C)) {
            e.SuppressKeyPress = true;
            CtrlCPressed(this, e);
            PowerShellConsolePrinter.OnCtrlC(this, e);
         } else if (PowerShellConsolePrinter.WaitForKey) {
            char key = new KeysConverter().ConvertToString(e.KeyData)[0];
            PowerShellConsolePrinter.PerformKeyPressedHandler(this, new KeyPressEventArgs(key));
         } else if (e.KeyData == Keys.Enter) {
            EnterKeyHandler(this, e);
            PowerShellConsolePrinter.OnEnter(this, e);
         }
      }

      private void txtPowerShellOutput_DoubleClicked(object sender, EventArgs e) {
         //m_powerShellConsolePrinter.WindowHeight = 1000;
         //m_powerShellConsolePrinter.WindowWidth = 2000;
      }
   }
}
