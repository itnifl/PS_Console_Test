namespace PS_Console_Test {
   partial class Form1 {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      // 
      private Controls.PowershellInteractiveControl m_powershellInteractiveControl1;      
     
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.m_powershellInteractiveControl1 = new PS_Console_Test.Controls.PowershellInteractiveControl();
         this.btnStart = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // m_powershellInteractiveControl1
         // 
         this.m_powershellInteractiveControl1.Location = new System.Drawing.Point(11, 27);
         this.m_powershellInteractiveControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.m_powershellInteractiveControl1.Name = "m_powershellInteractiveControl1";
         this.m_powershellInteractiveControl1.Size = new System.Drawing.Size(495, 296);
         this.m_powershellInteractiveControl1.TabIndex = 79;
         // 
         // btnStart
         // 
         this.btnStart.Location = new System.Drawing.Point(184, 330);
         this.btnStart.Name = "btnStart";
         this.btnStart.Size = new System.Drawing.Size(154, 43);
         this.btnStart.TabIndex = 80;
         this.btnStart.Text = "Start";
         this.btnStart.UseVisualStyleBackColor = true;
         this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(523, 392);
         this.Controls.Add(this.btnStart);
         this.Controls.Add(this.m_powershellInteractiveControl1);
         this.Name = "Form1";
         this.Text = "Form1";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button btnStart;
   }
}

