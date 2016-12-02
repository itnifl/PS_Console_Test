using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PS_Console_Test.Interfaces {
   public interface IProgressBarHandler {
      void ResetProgressbar(bool force = false);
      void StepProgress(int stepGoal);
      ProgressBar ProgressBar { get; set; }
   }
}
