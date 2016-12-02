using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS_Console_Test.Attributes {
   [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum)]
   public class UsageType : System.Attribute {
      public ClassUsage Usage;
      public UsageType(ClassUsage usage) {
         this.Usage = usage;
      }
   }
   public enum ClassUsage {
      Handler, Console, FilePointer, Status
   }
}
