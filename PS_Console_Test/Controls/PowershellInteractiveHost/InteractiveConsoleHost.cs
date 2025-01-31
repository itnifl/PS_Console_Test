namespace Microsoft.PowerShell.Host
{
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Management.Automation;
   using System.Management.Automation.Host;
   using System.Management.Automation.Runspaces;
   using System.Text;
   using PowerShell = System.Management.Automation.PowerShell;
   using PS_Console_Test.Handlers;
   using System.Windows.Forms;

   /// <summary>
   /// Simple PowerShell interactive console host listener implementation. This class
   /// implements a basic read-evaluate-print loop or 'listener' allowing you to
   /// interactively work with the PowerShell engine.
   /// </summary>
   public class PSListenerConsole {
      /// <summary>
      /// Executes when powershell command(s) are done
      /// </summary>
      public event Action WhenCommandIsDone = () => { };
      /// <summary>
      /// Get and set the printer to the console we are using:
      /// </summary>
      public PanelHandler PowerShellConsolePrinter { get; set; }
        /// <summary>
        /// Holds a reference to the runspace for this interpeter.
        /// </summary>
        internal Runspace myRunSpace;

        /// <summary>
        /// Indicator to tell the host application that it should exit.
        /// </summary>
        private bool shouldExit;

        /// <summary>
        /// The exit code that the host application will use to exit.
        /// </summary>
        private int exitCode;

        /// <summary>
        /// Holds a reference to  the PSHost object for this interpreter.
        /// </summary>
        private MyHost myHost;

        /// <summary>
        /// Holds a reference to the currently executing pipeline so that 
        /// it can be stopped by the control-C handler.
        /// </summary>
        private PowerShell currentPowerShell;

        /// <summary>
        /// Used to serialize access to instance data.
        /// </summary>
        private object instanceLock = new object();

        /// <summary>
        /// Gets or sets a value indicating whether the host applcation
        /// should exit.
        /// </summary>
        public bool ShouldExit
        {
            get { return this.shouldExit; }
            set { this.shouldExit = value; }
        }

        /// <summary>
        /// Gets or sets the exit code that the host application will use 
        /// when exiting.
        /// </summary>
        public int ExitCode
        {
            get { return this.exitCode; }
            set { this.exitCode = value; }
        }

      /// <summary>
      /// Creates and initiates the listener.
      /// </summary>
      /// <param name="args">The parameter is not used.</param>
      public void ExecuteAsynchronously(string commandString) {
         // Display the welcome message.
         //Console.Title = "Windows PowerShell Interactive Console Host";
         //ConsoleColor oldFg = Console.ForegroundColor;
         //Console.ForegroundColor = ConsoleColor.Cyan;
         //PowerShellConsolePrinter.WriteDelayedMessage(string.Empty);
         //PowerShellConsolePrinter.WriteDelayedMessage(string.Empty);
         this.Execute(commandString);
         //Console.ForegroundColor = oldFg;

         // Create the listener and runs it. This method never returns.
         //PSListenerConsole listener = new PSListenerConsole();
         //listener.Run();
         WhenCommandIsDone();
      }

      /// <summary>
      /// Create an instance of the console listener.
      /// </summary>
      public PSListenerConsole(PanelHandler powerShellConsolePrinter) {
         this.PowerShellConsolePrinter = powerShellConsolePrinter;
         // Create the host and runspace instances for this interpreter. Note that
         // this application doesn't support console files so only the default snapins
         // will be available.
         this.myHost = new MyHost(this);
         this.myRunSpace = RunspaceFactory.CreateRunspace(this.myHost);
         this.myRunSpace.Open();

         // Create a PowerShell object to run the commands used to create 
         // $profile and load the profiles.
         lock (this.instanceLock) {
               this.currentPowerShell = PowerShell.Create();
         }

         try {
            this.currentPowerShell.Runspace = this.myRunSpace;

            PSCommand[] profileCommands = Microsoft.PowerShell.Host.HostUtilities.GetProfileCommands("InteractiveConsoleHost");
            foreach (PSCommand command in profileCommands) {
               this.currentPowerShell.Commands = command;
               this.currentPowerShell.Invoke();
            }
         } catch(Exception ex) {
            MessageBox.Show(ex.Message, "Error Message", 
               MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
         }
         finally {
            // Dispose of the pipeline line and set it to null, locked because currentPowerShell
            // may be accessed by the ctrl-C handler...
            lock (this.instanceLock) {
               this.currentPowerShell.Dispose();
               this.currentPowerShell = null;
            }
         }
      }

        /// <summary>
        /// A helper class that builds and executes a pipeline that writes to the
        /// default output path. Any exceptions that are thrown are just passed to
        /// the caller. Since all output goes to the default 
        /// outter, this method does not return anything.
        /// </summary>
        /// <param name="cmd">The script to run.</param>
        /// <param name="input">Any input arguments to pass to the script. 
        /// If null then nothing is passed in.</param>
        private void executeHelper(string cmd, object input)
        {
            //THIS IS THE METHOD THAT EXECUTES MOST OF THE SCRIPTS AND COMMANDS IN THIS SYSTEM :)
            // Ignore empty command lines.
            if (String.IsNullOrEmpty(cmd))
            {
                return;
            }

            // Create the pipeline object and make it available to the
            // ctrl-C handle through the currentPowerShell instance
            // variable.
            lock (this.instanceLock)
            {
                this.currentPowerShell = PowerShell.Create();
            }

            // Create a pipeline for this execution, and then place the 
            // result in the currentPowerShell variable so it is available 
            // to be stopped.
            try
            {
                this.currentPowerShell.Runspace = this.myRunSpace;

                this.currentPowerShell.AddScript(cmd);


                // Add the default outputter to the end of the pipe and then 
                // call the MergeMyResults method to merge the output and 
                // error streams from the pipeline. This will result in the 
                // output being written using the PSHost and PSHostUserInterface 
                // classes instead of returning objects to the host application.
                this.currentPowerShell.AddCommand("out-default");
                this.currentPowerShell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

                // If there is any input pass it in, otherwise just invoke the
                // the pipeline.
                if (input != null)
                {
                    this.currentPowerShell.Invoke(new object[] { input });
                }
                else
                {
                    this.currentPowerShell.Invoke();
                }
            }
            finally
            {
                // Dispose the PowerShell object and set currentPowerShell to null. 
                // It is locked because currentPowerShell may be accessed by the 
                // ctrl-C handler.
                lock (this.instanceLock)
                {
                    this.currentPowerShell.Dispose();
                    this.currentPowerShell = null;
                }
            }
        }

        /// <summary>
        /// To display an exception using the display formatter, 
        /// run a second pipeline passing in the error record.
        /// The runtime will bind this to the $input variable,
        /// which is why $input is being piped to the Out-String
        /// cmdlet. The WriteErrorLine method is called to make sure 
        /// the error gets displayed in the correct error color.
        /// </summary>
        /// <param name="e">The exception to display.</param>
        private void ReportException(Exception e)
        {
            if (e != null)
            {
                object error;
                IContainsErrorRecord icer = e as IContainsErrorRecord;
                if (icer != null)
                {
                    error = icer.ErrorRecord;
                }
                else
                {
                    error = (object)new ErrorRecord(e, "Host.ReportException", ErrorCategory.NotSpecified, null);
                }

                lock (this.instanceLock)
                {
                    this.currentPowerShell = PowerShell.Create();
                }

                this.currentPowerShell.Runspace = this.myRunSpace;

                try
                {
                    this.currentPowerShell.AddScript("$input").AddCommand("out-string");

                    // Do not merge errors, this function will swallow errors.
                    Collection<PSObject> result;
                    PSDataCollection<object> inputCollection = new PSDataCollection<object>();
                    inputCollection.Add(error);
                    inputCollection.Complete();
                    result = this.currentPowerShell.Invoke(inputCollection);

                    if (result.Count > 0)
                    {
                        string str = result[0].BaseObject as string;
                        if (!string.IsNullOrEmpty(str))
                        {
                            // Remove \r\n, which is added by the Out-String cmdlet.    
                            this.myHost.UI.WriteErrorLine(str.Substring(0, str.Length - 2));
                        }
                    }
                }
                finally
                {
                    // Dispose of the pipeline and set it to null, locking it  because 
                    // currentPowerShell may be accessed by the ctrl-C handler.
                    lock (this.instanceLock)
                    {
                        this.currentPowerShell.Dispose();
                        this.currentPowerShell = null;
                    }
                }
            }
        }

        /// <summary>
        /// Basic script execution routine. Any runtime exceptions are
        /// caught and passed back to the Windows PowerShell engine to 
        /// display.
        /// </summary>
        /// <param name="cmd">Script to run.</param>
        private void Execute(string cmd)
        {
            try
            {
                // Execute the command with no input.
                this.executeHelper(cmd, null);
            }
            catch (RuntimeException rte)
            {
                this.ReportException(rte);
            }
            PowerShellConsolePrinter.WriteDelayedMessage(((MyHostUserInterface)this.myHost.UI).ConsolePromptDefinator);
        }

        /// <summary>
        /// Method used to handle control-C's from the user. It calls the
        /// pipeline Stop() method to stop execution. If any exceptions occur
        /// they are printed to the console but otherwise ignored.
        /// </summary>
        /// <param name="sender">See sender property documentation of  
        /// ConsoleCancelEventHandler.</param>
        /// <param name="e">See e property documentation of 
        /// ConsoleCancelEventHandler.</param>
        private void HandleControlC(object sender, KeyEventArgs e)
        {
            try
            {
                lock (this.instanceLock)
                {
                    if (this.currentPowerShell != null && this.currentPowerShell.InvocationStateInfo.State == PSInvocationState.Running)
                    {
                        this.currentPowerShell.Stop();
                    }
                }
               //e.Cancel = true;
               this.ShouldExit = true;
            } catch (Exception exception) {
                this.myHost.UI.WriteErrorLine(exception.ToString());
            }
        }

      /// <summary>
      /// Reads a command when Enter is pressed.
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">event</param>
      private void HandleEnterCommand(object sender, KeyEventArgs e) {
         try {
            lock (this.instanceLock) {
               string cmd = PowerShellConsolePrinter.ReadLine(((MyHostUserInterface)this.myHost.UI).ConsolePromptDefinator);
               if (!String.IsNullOrEmpty(cmd))
                  this.Execute(cmd);
            }
         }
         catch (Exception exception) {
            this.myHost.UI.WriteErrorLine(exception.ToString());
         }
      }

      /// <summary>
      /// Implements the basic listener loop. It sets up the ctrl-C handler, then
      /// reads a command from the user, executes it and repeats until the ShouldExit
      /// flag is set.
      /// </summary>
      public void Run() {
         // Set up the control-C handler.

         PowerShellConsolePrinter.CtrlCHandler += new KeyEventHandler(this.HandleControlC);
         //PowerShellConsolePrinter.EnterKeyHandler += new KeyEventHandler(this.HandleEnterCommand);
         //Console.TreatControlCAsInput = false;

         //this.currentPowerShell.BeginStop
         //this.currentPowerShell.Runspace = this.myRunSpace;

         // loop reading commands to execute until ShouldExit is set by
         // the user calling "exit".
         string prompt;
         if (this.myHost.IsRunspacePushed) {
            prompt = string.Format("[{0}] PS>", this.myRunSpace.ConnectionInfo.ComputerName);
         }
         else {
            prompt = "PS>";
         }
         ((MyHostUserInterface)this.myHost.UI).ConsolePromptDefinator = String.IsNullOrEmpty(prompt) ? "PS>" : prompt;
         //this.myHost.UI.Write(ConsoleColor.Cyan, ConsoleColor.Black, prompt);

         while (!this.ShouldExit) {
            System.Threading.Thread.Sleep(100);
            string cmd = PowerShellConsolePrinter.ReadLine(prompt);
            if (!String.IsNullOrEmpty(cmd)) {
               lock (this.instanceLock) {
                  this.Execute(cmd);                  
               }
            }
         }

         // Exit with the desired exit code that was set by exit command.
         // This is set in the host by the MyHost.SetShouldExit() implementation.
         //Environment.Exit(this.ExitCode);
      }
   }
}
