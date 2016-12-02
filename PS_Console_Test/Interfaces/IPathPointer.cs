using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS_Console_Test.Interfaces {
   /// <summary>
   /// This interface defines methods for classes that handle files and file locations.
   /// </summary>
   interface IPathPointer {
      /// <summary>
      /// Get base path of the files.
      /// </summary>
      /// <returns>Base path as string</returns>
      string GetBasePath();
      /// <summary>
      /// Get all the files that are handeled under the base path.
      /// </summary>
      /// <returns>List of file names as strings.</returns>
      List<string> GetFilesNames();
   }
}
