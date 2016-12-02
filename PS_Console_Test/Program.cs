using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS_Console_Test.Attributes;
using PS_Console_Test.Interfaces;
using System.ComponentModel;

namespace PS_Console_Test {
   static class Program {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main() {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new Form1());
      }
   }
}
namespace PS_Console_Test.Helpers {
   public class ToolTipHelper {
      private readonly Dictionary<string, ToolTip> tooltips;

      /// <summary>
      /// Constructor
      /// </summary>
      public ToolTipHelper() {
         this.tooltips = new Dictionary<string, ToolTip>();
      }

      /// <summary>
      /// Key a tooltip by its control name
      /// </summary>
      /// <param name="controlName"></param>
      /// <returns></returns>
      public ToolTip GetControlToolTip(string controlName) {
         if (tooltips.ContainsKey(controlName)) {
            return tooltips[controlName];
         } else {
            ToolTip tt = new ToolTip();
            tooltips.Add(controlName, tt);
            return tt;
         }
      }
   }
   [UsageType(ClassUsage.FilePointer)]
   class PDFFAQFiles : IPathPointer {
      public string FaqPath;
      public string Insert_Windows_Licenses;
      public string Enable_Firewall;
      public string Change_CDP_IP_Address;
      public string Disable_UAC;
      public string Nagios_Code_Of_13;

      public PDFFAQFiles(string faqPath) {
         FaqPath = String.IsNullOrEmpty(faqPath) ? ".\\FAQ-Files\\" : faqPath;
         Insert_Windows_Licenses = "Insert_Windows_Licenses.pdf";
         Enable_Firewall = "Enable_Firewall.pdf";
         Change_CDP_IP_Address = "Change_CDP_IP_Address.pdf";
         Disable_UAC = "Turn_Off_UAC.pdf";
         Nagios_Code_Of_13 = "Nagios_code_of_13_is_out_of_bounds.pdf";
      }
      public string GetBasePath() {
         return FaqPath;
      }
      public List<string> GetFilesNames() {
         return new List<string>() { Insert_Windows_Licenses, Enable_Firewall, Change_CDP_IP_Address };
      }
   }
   [UsageType(ClassUsage.FilePointer)]
   class IniFiles : IPathPointer {
      public string IniPath;
      public string Licenses;
      public string Time;

      public IniFiles(string iniPath) {
         IniPath = String.IsNullOrEmpty(iniPath) ? ".\\ConfigurationFiles\\" : iniPath;
         Licenses = "licenses.ini";
         Time = "time.ini";
      }
      public string GetBasePath() {
         return IniPath;
      }
      public List<string> GetFilesNames() {
         return new List<string>() { Licenses, Time };
      }
   }
   [UsageType(ClassUsage.FilePointer)]
   class SupportScripts : IPathPointer {
      public string ScriptsPath;
      public string RebuildVirtualNetwork;
      public string ReCheckvSphere;
      public string LicenseActivateVMs;
      public string DownloadServersTxt;

      public SupportScripts(string iniPath) {
         ScriptsPath = String.IsNullOrEmpty(iniPath) ? ".\\FAQ-Files\\SupportScripts\\" : iniPath;
         RebuildVirtualNetwork = "RebuildVirtualNetwork.ps1";
         ReCheckvSphere = "ReCheckvSphere.ps1";
         LicenseActivateVMs = "ActivateVMs.ps1";
         DownloadServersTxt = "DownloadServersTxt.ps1";
      }
      public string GetBasePath() {
         return ScriptsPath;
      }
      public List<string> GetFilesNames() {
         return new List<string>() { RebuildVirtualNetwork, ReCheckvSphere, LicenseActivateVMs, DownloadServersTxt };
      }
   }
   /// <summary>
   /// The definitions of status in this application
   /// </summary>
   [UsageType(ClassUsage.Status)]
   public enum Status {
      Success, InProgress, Failed
   };

   public static partial class ExtensionMethods {
      public static void HandleInvokeRequired<T>(this T control, Action<T> action) where T : Control, ISynchronizeInvoke {
         //Check to see is the control is not null
         if (control == null)
            throw new ArgumentNullException(string.Format("Cannot execute {0} on {1}.  {1} is null.", action, control));

         //Check to see if the control is disposed.
         if (control is Control && (control as Control).IsDisposed)
            throw new ObjectDisposedException(string.Format("Cannot execute {0} on {1}.  {1} is disposed.", action, control));

         //Check to see if the handle is created for the control.
         if (control is Control && !(control as Control).IsHandleCreated)
            throw new InvalidOperationException(string.Format("Cannot execute {0} on {1}.  Handle is not created for {1}.", action, control));

         //Check to see if the control's InvokeRequired property is true
         if (control.InvokeRequired) {
            try {
               //Use Invoke() to invoke your action
               control.Invoke(action, new object[] { control });
            } catch (Exception ex) {
               throw new Exception(string.Format("Cannot execute {0} on {1}.  {2}.", action, control, ex.Message));
            }
         } else {
            try {
               //Perform the action
               action(control);
            } catch (Exception ex) {
               throw new Exception(string.Format("Cannot execute {0} on {1}.  {2}.", action, control, ex.Message));
            }
         }
      }
   }
}

