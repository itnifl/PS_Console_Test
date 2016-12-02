using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PS_Console_Test.Helpers;
using System.Drawing;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using PS_Console_Test.Interfaces;
using PS_Console_Test.Attributes;
using System.ComponentModel;

namespace PS_Console_Test.Handlers {
   [UsageType(ClassUsage.Handler)]
   public class PanelHandler {
      private delegate void PrintTextCallback(Char c, RichTextBox theTextbox, Color printColor);
      private delegate string GetRtfCallback(RichTextBox theTextbox);
      private delegate string GetTextCallback(RichTextBox theTextbox);
      private delegate void SetCursorAtLastLineCallback(RichTextBox theTextbox);
      private delegate void SetRtfCallback(string rtf, RichTextBox theTextbox);
      private object m_LockObject = new object();
            
      /// <summary>
      /// The progressbar that we want to show progress on
      /// </summary>
      public IProgressBarHandler ActivityProgressbar { get; set; }
      /// <summary>
      /// The title we can give the text box, as of now it is not in use anywhere.
      /// </summary>
      public string WindowTitle = String.Empty;
      /// <summary>
      /// The RichTextBox textbox we are going to write to
      /// </summary>
      [Required(ErrorMessage = "RichTextBox is required for a instance of this class, it is the textbox we want to handle with this class.")]
      public RichTextBox TheTextBox { get; }
      /// <summary>
      /// Handle Ctrl-C events
      /// </summary>
      public event  KeyEventHandler CtrlCHandler = (object sender, KeyEventArgs e) => { };
      /// <summary>
      /// Handle Enter Key Events
      /// </summary>
      public event KeyEventHandler EnterKeyHandler = (object sender, KeyEventArgs e) => { };
      /// <summary>
      /// Handle when status string is encountered(defined in StatusUpdateList) in WriteDelayedMessage, passes what number in list StatusUpdateList has been encountered.
      /// </summary>
      public event Action<int> StatusStepEvent = (int CurrentStep) => { };
      /// <summary>
      /// The delay between each print of characters in milliseconds
      /// </summary>
      public int PrintDelay { get; set; } = 35;
      /// <summary>
      /// The color that we want to do our printing in, the default is black.
      /// </summary>
      public Color TextColor { get; set; } = Color.Black;
      /// <summary>
      /// The background color that we want for our RichTextBox
      /// </summary>
      public Color BackgroundColor { get; set; }
      /// <summary>
      /// Sepcifies if we should listen for any key pressed and notify what has been pressed when this happens
      /// </summary>
      public bool WaitForKey = false;
      /// <summary>
      /// Let subscribers know what key has been pressed when WaitForKey is true.
      /// </summary>
      public event KeyPressEventHandler KeyPressedHandler = (object sender, KeyPressEventArgs e) => { };
      /// <summary>
      /// Set the width of the RichTextBox, emulates Console.BufferWidth as the same as Console.WindowWidth
      /// </summary>            
      public int BufferWidth {
         get {
            lock (m_LockObject) {
               return TheTextBox.Width;
            }
         }
         set {
            lock (m_LockObject) {
               TheTextBox.Width = value;
            }
         }
      }
      /// <summary>
      /// Set the height of the RichTextBox, emulates Console.BufferHeight as the same as Console.WindowHeight
      /// </summary>
      public int BufferHeight {
         get {
            lock (m_LockObject) {
               return TheTextBox.Height;
            }
         }
         set {
            lock (m_LockObject) {
               TheTextBox.Height = value;
            }
         }
      }
      /// <summary>
      /// Set the width of the RichTextBox, emulates Console.WindowWidth
      /// </summary>
      public int WindowWidth {
         get {
            lock (m_LockObject) {
               return TheTextBox.Width;
            }
         }
         set {
            lock (m_LockObject) {
               TheTextBox.Width = value;
            }
         }
      }
      /// <summary>
      /// Set the height of the RichTextBox, emulates Console.WindowHeight
      /// </summary>
      public int WindowHeight {
         get {
            lock (m_LockObject) {
               return TheTextBox.Height;
            }
         }
         set {
            lock (m_LockObject) {
               TheTextBox.Height = value;
            }
         }
      }
      /// <summary>
      /// This is the string that triggers the StatusHandler to show success when it is envountered via the WriteDelayedMessage method
      /// </summary>
      public string StatusSuccessString;
      /// <summary>
      /// If set, this is the list of status updates that our IStatusHandler should update with when used in the WriteDelayedMessage method
      /// </summary>
      public List<string> StatusUpdateList;
      /// <summary>
      /// This is the status handler we update with contents of StatusUpdateList when elements of it are encountered via the WriteDelayedMessage method
      /// </summary>
      public IStatusHandler StatusHandler;

      public PanelHandler(RichTextBox theTextBox) {
         if (theTextBox == null) throw new ArgumentNullException("Cannot initialize class " + this.GetType() + " with a null argument!");
         lock (m_LockObject) {
            this.TheTextBox = theTextBox;
            this.BackgroundColor = theTextBox.BackColor;
         }
      }
      /// <summary>
      /// Writes a message to the panel that this PanelHandler keeps a reference to
      /// </summary>
      /// <param name="message">The message string we want to write.</param>
      public void WriteDelayedMessage(string message) {
         if (!String.IsNullOrEmpty(message)) {
            //If the PanelHandler is printing progress text, then remove the last line of this progress text so we can update it with new progress count text:
            if (new Regex(@"[a-z]{8}\:\s{1}\d{1,3}\%", RegexOptions.IgnoreCase).Match(message).Success) {
               RemoveLastLine(@"[a-z]{8}\:\s{1}\d{1,3}\%");
            }
            //If the PanelHandler is printing progress text, then remove the last line of this progress text so we can update it with new progress count text:
            if (new Regex(@"\d{1,3}\s{1}\%\s{1}[a-z]{8}", RegexOptions.IgnoreCase).Match(message).Success) {
               RemoveLastLine(@"\d{1,3}\s{1}\%\s{1}[a-z]{8}");
            }
            if (StatusUpdateList != null) {
               string statusMessage = StatusUpdateList.Where(m => message.Trim().ToLower().Contains(m.Trim().ToLower())).FirstOrDefault();
               if (!String.IsNullOrEmpty(statusMessage) && StatusHandler != null) {
                  string successComparer = !String.IsNullOrEmpty(StatusSuccessString) ? StatusSuccessString.ToLower() : String.Empty;
                  StatusHandler.PrintStatus(statusMessage, statusMessage.ToLower() == successComparer ? Status.Success : Status.InProgress);
                  StatusStepEvent(StatusUpdateList.FindIndex(m => m.ToLower().Trim() == statusMessage.ToLower()));
               }
            }

            int msgLength = message.TrimEnd().Length;
            string outText;
            if (message == "PS>" || message == "\nPS>" || msgLength == 0) {
               outText = message;
            }
            else {
               outText = message.Substring(0, msgLength);
               if (outText[outText.Length - 1] != '\n') {
                  outText += "\n";
               }
            }
            foreach (char c in outText) {
               if (TheTextBox.InvokeRequired) {
                  // It's on a different thread, so use Invoke.
                  lock (m_LockObject) {
                     PrintTextCallback ourDelegate = new PrintTextCallback(printText);
                     TheTextBox.Invoke(ourDelegate, new object[] { c, TheTextBox, TextColor });
                  }
               }
               else {
                  lock (m_LockObject) {
                     // It's on the same thread, no need for Invoke 
                     TheTextBox.AppendText(c.ToString(), TextColor);
                  }
               }
               Thread.Sleep(PrintDelay);
            }
         }
      }
      /// <summary>
      /// Set the status and message of our panel if a StatusHandler is attached to the instance.
      /// A Statushandler is a IStatusHandler that shows status messages on a display handled by the StatusHandler.
      /// </summary>
      /// <param name="statusMessage">Message for the StatusHandler to display.</param>
      /// <param name="status">The status we are going to convey.</param>
      public void SetPanelStatus(string statusMessage, Status status) {
         if (StatusHandler != null) {
            StatusHandler.PrintStatus(statusMessage, status);
         }
      }
      /// <summary>
      /// Sets the cursor on the last line, but does not scroll.
      /// </summary>
      public void SetCursorAtLastLine() {
         if (TheTextBox.InvokeRequired) {
            lock (m_LockObject) {
               SetCursorAtLastLineCallback ourDelegate = new SetCursorAtLastLineCallback(setCursorAtLastLine);
               TheTextBox.Invoke(ourDelegate, new object[] { TheTextBox });
            }
         } else {
            lock (m_LockObject) {
               TheTextBox.Select(TheTextBox.TextLength, 1);
            }
         }
      }
      /// <summary>
      /// Remove last line if it is identified by regex 
      /// </summary>
      /// <param name="regex">regex to match in last line</param>
      public void RemoveLastLine(string regex) {
         string OldText = String.Empty;
         StringBuilder NewText = new StringBuilder();
         int stepBack = 2;

         string lastLine = String.Empty;
         TheTextBox.HandleInvokeRequired(__TheTextBox => {
            lastLine = __TheTextBox.Lines[__TheTextBox.Lines.Length - 1];
            if (String.IsNullOrEmpty(lastLine)) {
               lastLine = __TheTextBox.Lines[__TheTextBox.Lines.Length - 2];
               stepBack = 4;
            }
         });
         if (new Regex(regex, RegexOptions.IgnoreCase).Match(lastLine).Success) {
            //1. Get the Rtf from the Textbox
            if (TheTextBox.InvokeRequired) {
               // It's on a different thread, so use Invoke:
               lock (m_LockObject) {
                  GetRtfCallback ourDelegate = new GetRtfCallback(getRtf);
                  OldText = (String)TheTextBox.Invoke(ourDelegate, new object[] { TheTextBox });
               }
            }
            else {
               lock (m_LockObject) {
                  OldText = TheTextBox.Rtf;
               }
            }

            //2. Perform magic on the text retrieved from the Textbox:
            int size = OldText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
            for (int count = 0; count < size - stepBack; count++ ) {
               NewText.Append(OldText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[count]);
               if (NewText[NewText.Length - 1] != '\n' || NewText[NewText.Length - 1].ToString() != System.Environment.NewLine) {
                  NewText.Append(System.Environment.NewLine);
               }
            };


            //3. Put the Rtf back to the text box after the magic is done:
            if (TheTextBox.InvokeRequired) {
               // It's on a different thread, so use Invoke.
               lock (m_LockObject) {
                  SetRtfCallback ourDelegate = new SetRtfCallback(setRtf);
                  TheTextBox.Invoke(ourDelegate, new object[] { NewText.ToString(), TheTextBox });
               }
            }
            else {
               lock (m_LockObject) {
                  TheTextBox.Rtf = NewText.ToString();
                  TheTextBox.Refresh();
               }
            }
         }         
      }
      /// <summary>
      /// Remove specified words from our RichTextBox control identified by they keywords argument.
      /// </summary>
      /// <param name="Keywords">List of strings that identify words we want to remove from TheTextBox</param>
      public void RemoveLines(List<string> Keywords) {
         string regex = String.Empty;
         string NewText = String.Empty;

         //1. Get the Rtf from the Textbox
         if (TheTextBox.InvokeRequired) {
            // It's on a different thread, so use Invoke:
            lock (m_LockObject) {
               GetRtfCallback ourDelegate = new GetRtfCallback(getRtf);
               NewText = (String)TheTextBox.Invoke(ourDelegate, new object[] { TheTextBox });
            }
         }
         else {
            lock (m_LockObject) {
               NewText = TheTextBox.Rtf;
            }
         }

         //2. Perform regex magic on the text retrieved from the Textbox:
         Regex MyRegex = null;
         foreach (string keyword in Keywords) {
            regex = String.Format(@"{0}", keyword);
            MyRegex = new Regex(regex, RegexOptions.Multiline);
            NewText = MyRegex.Replace(NewText, "");
            //This one would remove blank lines, but is commented away:
            //NewText = Regex.Replace(NewText, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
         }

         //3. Put the Rtf back to the text box after the magic is done:
         if (TheTextBox.InvokeRequired) {
            // It's on a different thread, so use Invoke.
            lock (m_LockObject) {
               SetRtfCallback ourDelegate = new SetRtfCallback(setRtf);
               TheTextBox.Invoke(ourDelegate, new object[] { NewText, TheTextBox });
            }
         }
         else {
            lock (m_LockObject) {               
               TheTextBox.Rtf = NewText;
               TheTextBox.Refresh();
            }
         }
                
      }
      /// <summary>
      /// Execute this code on Ctrl-C, or to perform the handler for Ctrl-C.
      /// </summary>
      /// <param name="sender">Object</param>
      /// <param name="e">KeyEventArgs</param>
      public void OnCtrlC(object sender, KeyEventArgs e) {
         CtrlCHandler(sender, e);
      }
      /// <summary>
      /// Execute this code on Enter Key, or to perform the handler for Ctrl-C.
      /// </summary>
      /// <param name="sender">Object</param>
      /// <param name="e">KeyEventArgs</param>
      public void OnEnter(object sender, KeyEventArgs e) {
         EnterKeyHandler(sender, e);
      }
      /// <summary>
      /// Reads line after keyword and until newline character.
      /// </summary>
      /// <param name="keyword">Where reading should start</param>
      /// <returns>Returns line that has been read.</returns>
      public String ReadLine(string keyword) {
         String text = String.Empty;
         if (TheTextBox.InvokeRequired) {
            // It's on a different thread, so use Invoke:
            lock (m_LockObject) {
               GetTextCallback ourDelegate = new GetTextCallback(getText);
               text = (String)TheTextBox.Invoke(ourDelegate, new object[] { TheTextBox });
            }
         }
         else {
            lock (m_LockObject) {
               text = TheTextBox.Text;
            }
         }
         if((text.LastIndexOf("\n") - text.LastIndexOf(keyword) - keyword.Length > 0) && text.LastIndexOf(keyword) > -1) {
            return text.Substring(text.LastIndexOf(keyword) + keyword.Length, text.LastIndexOf("\n") - text.LastIndexOf(keyword) - keyword.Length);
         } else {
            return String.Empty;
         }         
      }
      public void PerformKeyPressedHandler(object sender, KeyPressEventArgs e) {
         KeyPressedHandler(sender, e);
      }
      private static void setRtf(string NewText, RichTextBox theTextbox) {
         theTextbox.Rtf = NewText;
         theTextbox.Refresh();
      }
      private static string getRtf(RichTextBox theTextbox) {
         return theTextbox.Rtf;
      }
      private static string getText(RichTextBox theTextbox) {
         return theTextbox.Text;
      }
      private static void setCursorAtLastLine(RichTextBox theTextbox) {
         theTextbox.Select(theTextbox.TextLength, 1);
      }
      private static void printText(Char c, RichTextBox theTextbox, Color printColor) {
         theTextbox.AppendText(c.ToString(), printColor);
         if (c.Equals('\n')) {
            theTextbox.SelectionStart = theTextbox.TextLength;
            theTextbox.ScrollToCaret();
         }
      }
   }
   public static class RichTextBoxExtensions {
     /*private static object m_lock = new object();
      public static void AppendText(this RichTextBox box, string text, Color color) {         
         if(box.InvokeRequired) {
            lock(m_lock) {
               //Nothing here, code still breaks
            }
         }
         else {
            lock (m_lock) {
               box.SelectionStart = box.TextLength;
               box.SelectionLength = 0;

               box.SelectionColor = color;
               box.AppendText(text);
               box.SelectionColor = box.ForeColor;
            }
         }   
      }*/

     public static void AppendText(this RichTextBox box, string text, Color color) {
         box.HandleInvokeRequired(_box => {
            _box.SelectionStart = _box.TextLength;
            _box.SelectionLength = 0;

            _box.SelectionColor = color;
            _box.AppendText(text);
            _box.SelectionColor = _box.ForeColor;
         });
      }      
   }
}
