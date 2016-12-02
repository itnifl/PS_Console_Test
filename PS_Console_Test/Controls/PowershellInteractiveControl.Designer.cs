namespace PS_Console_Test.Controls {
   partial class PowershellInteractiveControl {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.txtPowerShellOutput = new System.Windows.Forms.RichTextBox();
         this.SuspendLayout();
         // 
         // txtPowerShellOutput
         // 
         this.txtPowerShellOutput.BackColor = System.Drawing.Color.DarkGray;
         this.txtPowerShellOutput.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtPowerShellOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txtPowerShellOutput.ForeColor = System.Drawing.Color.White;
         this.txtPowerShellOutput.Location = new System.Drawing.Point(0, 0);
         this.txtPowerShellOutput.Name = "txtPowerShellOutput";
         this.txtPowerShellOutput.Size = new System.Drawing.Size(495, 296);
         this.txtPowerShellOutput.TabIndex = 60;
         this.txtPowerShellOutput.Text = "";
         this.txtPowerShellOutput.Click += new System.EventHandler(this.txtPowerShellOutput_Clicked);
         this.txtPowerShellOutput.DoubleClick += new System.EventHandler(this.txtPowerShellOutput_DoubleClicked);
         this.txtPowerShellOutput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPowerShellOutput_KeyUp);
         // 
         // PowershellInteractiveControl
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.txtPowerShellOutput);
         this.Name = "PowershellInteractiveControl";
         this.Size = new System.Drawing.Size(495, 296);
         this.ResumeLayout(false);

      }

      #endregion

      public System.Windows.Forms.RichTextBox txtPowerShellOutput;
   }
}
