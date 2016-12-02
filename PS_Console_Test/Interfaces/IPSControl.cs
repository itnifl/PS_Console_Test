using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS_Console_Test.Handlers;
using Microsoft.PowerShell.Host;
using System.IO;

namespace PS_Console_Test.Interfaces {
   /// <summary>
   /// Defines how a User Control should look like when implementing access to Windows PowerShell
   /// </summary>
   interface IPSControl {
      event Action<bool> ControlVisibleChangeEvent;
      void ExecuteAsynchronously(string commandOrFileString);
   }
}
