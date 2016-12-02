using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS_Console_Test.Helpers;

namespace PS_Console_Test.Interfaces {
   public interface IStatusHandler {
      void PrintStatus(string statusMessage, Status messagesStatus);
   }
}
