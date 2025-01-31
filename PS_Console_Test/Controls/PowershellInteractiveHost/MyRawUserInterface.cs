namespace Microsoft.PowerShell.Host {
   using System;
   using System.Management.Automation.Host;
   using PS_Console_Test.Handlers;
    
    /// <summary>
    /// A implementation of the PSHostRawUserInterface for a console
    /// application. Members of this class that map easily to the .NET Console
    /// APIs are implemented. More complex methods are not implemented and will
    /// throw a NotImplementedException exception.
    /// </summary>
    internal class MyRawUserInterface : PSHostRawUserInterface {
      public PanelHandler PowerShellConsolePrinter { get; set; }
      public MyRawUserInterface(PanelHandler printer) {
         this.PowerShellConsolePrinter = printer;
      }
      /// <summary>
      /// Gets or sets the background color of text to be written.
      /// This maps pretty directly onto the corresponding .NET Console
      /// property.
      /// </summary>
      public override ConsoleColor BackgroundColor {
         get { return ColorExtension.GetConsoleColor(PowerShellConsolePrinter.BackgroundColor); }
         set { PowerShellConsolePrinter.BackgroundColor = ColorExtension.GetColor(value); }
      }

      /// <summary>
      /// Gets or sets the host buffer size adapted from on the 
      /// .NET Console buffer size.
      /// </summary>
      public override Size BufferSize {
         get { return new Size(PowerShellConsolePrinter.BufferWidth, PowerShellConsolePrinter.BufferHeight); }
         set {
            PowerShellConsolePrinter.BufferHeight = value.Height;
            PowerShellConsolePrinter.BufferHeight = value.Width;
         }
      }

      /// <summary>
      /// Gets or sets the cursor position. This functionality is not 
      /// implemented. The call fails with an exception.
      /// </summary>
      public override Coordinates CursorPosition {
         get { return new Coordinates(0, 0); }
         set { //Do nothing
         }
      }

      /// <summary>
      /// Gets or sets the cursor size taken directly from the .NET 
      /// Console cursor size.
      /// </summary>
      public override int CursorSize {
         get { throw new NotImplementedException("The CursorSize property is not implemented by MyRawUserInterface."); }
         set { throw new NotImplementedException("The CursorSize property is not implemented by MyRawUserInterface."); }
      }

      /// <summary>
      /// Gets or sets the foreground color of the text to be written.
      /// This maps pretty directly onto the corresponding .NET Console
      /// property.
      /// </summary>
      public override ConsoleColor ForegroundColor {
         get { return ColorExtension.GetConsoleColor(PowerShellConsolePrinter.TextColor); }
         set { PowerShellConsolePrinter.TextColor = ColorExtension.GetColor(value); }
      }

      /// <summary>
      /// Gets a value that indicates whether a key is available. 
      /// This implementation maps directly to the corresponding 
      /// .NET Console property.
      /// </summary>
      public override bool KeyAvailable {
         get { throw new NotImplementedException("The KeyAvailable property is not implemented by MyRawUserInterface."); }
         //get { return Console.KeyAvailable; }
      }

      /// <summary>
      /// Gets the maximum physical size of the window adapted from the  
      /// .NET Console LargestWindowWidth and LargestWindowHeight properties.
      /// </summary>
      public override Size MaxPhysicalWindowSize {
         get { throw new NotImplementedException("The MaxPhysicalWindowSize property is not implemented by MyRawUserInterface."); }
         //get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
      }

      /// <summary>
      /// Gets the maximum window size adapted from the .NET Console
      /// LargestWindowWidth and LargestWindowHeight properties.
      /// </summary>
      public override Size MaxWindowSize {
         get { throw new NotImplementedException("The MaxWindowSize property is not implemented by MyRawUserInterface."); }
         //get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
      }

      /// <summary>
      /// Gets or sets the window position adapted from the Console window position 
      /// information.
      /// </summary>
      public override Coordinates WindowPosition {
         get { throw new NotImplementedException("The WindowPosition property is not implemented by MyRawUserInterface."); }
         set { throw new NotImplementedException("The WindowPosition property is not implemented by MyRawUserInterface."); }
         //get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
         //set { Console.SetWindowPosition(value.X, value.Y); }
      }

      /// <summary>
      /// Gets or sets the window size adapted from the corresponding .NET Console calls.
      /// </summary>
      public override Size WindowSize {
         get { return new Size(PowerShellConsolePrinter.WindowWidth, PowerShellConsolePrinter.WindowHeight); }
         set {
            PowerShellConsolePrinter.WindowWidth = value.Width;
            PowerShellConsolePrinter.WindowHeight = value.Height;
         }
      }

      /// <summary>
      /// Gets or sets the title of the window mapped to the PowerShellConsolePrinter.WindowTitle property.
      /// </summary>
      public override string WindowTitle {
         get {
            return PowerShellConsolePrinter.WindowTitle;
         }
         set {           
            PowerShellConsolePrinter.WindowTitle = value; 
         }
      }

      /// <summary>
      /// Resets the input buffer. This method is not currently 
      /// implemented and returns silently.
      /// </summary>
      public override void FlushInputBuffer() {
         // Do nothing.
      }

      /// <summary>
      /// Retrieves a rectangular region of the screen buffer. This method 
      /// is not implemented. The call fails with an exception.
      /// </summary>
      /// <param name="rectangle">A Rectangle object that defines the size of the rectangle</param>
      /// <returns>Throws a NotImplementedException exception.</returns>
      public override BufferCell[,] GetBufferContents(Rectangle rectangle) {
         throw new NotImplementedException("The GetBufferContents method is not implemented by MyRawUserInterface.");
      }

      /// <summary>
      /// Reads a pressed, released, or pressed and released keystroke 
      /// from the keyboard device, blocking processing until a keystroke 
      /// is typed that matches the specified keystroke options. This 
      /// functionality is not implemented. The call fails with an 
      /// exception.
      /// </summary>
      /// <param name="options">A bit mask of the options to be used when 
      /// reading from the keyboard. </param>
      /// <returns>KeyInfo</returns>
      public override KeyInfo ReadKey(ReadKeyOptions options) {
         PowerShellConsolePrinter.WaitForKey = true;
         KeyInfo keyInfo = new KeyInfo();
         PowerShellConsolePrinter.KeyPressedHandler += (object sender, System.Windows.Forms.KeyPressEventArgs e) => {
            if (!options.HasFlag(ReadKeyOptions.NoEcho)) {
               PowerShellConsolePrinter.WriteDelayedMessage((e.KeyChar).ToString());
            }
            keyInfo.KeyDown = true;
            keyInfo.Character = e.KeyChar;
         };
         while (!keyInfo.KeyDown) {
            //Wait for the key
         }
         return keyInfo;
         //throw new NotImplementedException("The ReadKey() method is not implemented by MyRawUserInterface.");
      }

      /// <summary>
      /// Crops a region of the screen buffer. This functionality is not 
      /// implemented. The call fails with an exception.
      /// </summary>
      /// <param name="source">A Rectangle structure that identifies the 
      /// region of the screen to be scrolled.</param>
      /// <param name="destination">A Coordinates structure that 
      /// identifies the upper-left coordinates of the region of the 
      /// screen to receive the source region contents.</param>
      /// <param name="clip">A Rectangle structure that identifies the 
      /// region of the screen to include in the operation.</param>
      /// <param name="fill">A BufferCell structure that identifies the 
      /// character and attributes to be used to fill all cells within 
      /// the intersection of the source rectangle and clip rectangle 
      /// that are left "empty" by the move.</param>
      public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill) {
         throw new NotImplementedException("The ScrollBufferContents() method is not implemented by MyRawUserInterface.");
      }

      /// <summary>
      /// Copies a given character to a region of the screen buffer based 
      /// on the coordinates of the region. This method is not implemented. 
      /// </summary>
      /// <param name="origin">The coordnates of the region.</param>
      /// <param name="contents">A BufferCell structure that defines the fill character.</param>
      public override void SetBufferContents(Coordinates origin, BufferCell[,] contents) {
         //do nothing
      }

      /// <summary>
      /// Copies a given character to a rectangular region of the screen 
      /// buffer. This method is not implemented. 
      /// </summary>
      /// <param name="rectangle">A Rectangle structure that defines the area to be filled.</param>
      /// <param name="fill">A BufferCell structure that defines the fill character.</param>
      public override void SetBufferContents(Rectangle rectangle, BufferCell fill) {
         //do nothing
      }
   }
}

