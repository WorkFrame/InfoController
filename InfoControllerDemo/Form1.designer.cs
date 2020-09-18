namespace NetEti.DemoApplications
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.lblMessage = new System.Windows.Forms.Label();
      this.tbxMessages = new System.Windows.Forms.TextBox();
      this.btnGo = new System.Windows.Forms.Button();
      this.lblException = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lblMessage
      // 
      this.lblMessage.AutoSize = true;
      this.lblMessage.Location = new System.Drawing.Point(16, 27);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 17);
      this.lblMessage.TabIndex = 0;
      // 
      // tbxMessages
      // 
      this.tbxMessages.Location = new System.Drawing.Point(16, 60);
      this.tbxMessages.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.tbxMessages.Multiline = true;
      this.tbxMessages.Name = "tbxMessages";
      this.tbxMessages.Size = new System.Drawing.Size(345, 74);
      this.tbxMessages.TabIndex = 1;
      // 
      // btnGo
      // 
      this.btnGo.Location = new System.Drawing.Point(263, 282);
      this.btnGo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.btnGo.Name = "btnGo";
      this.btnGo.Size = new System.Drawing.Size(100, 28);
      this.btnGo.TabIndex = 2;
      this.btnGo.Text = "go!";
      this.btnGo.UseVisualStyleBackColor = true;
      this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
      // 
      // lblException
      // 
      this.lblException.AutoSize = true;
      this.lblException.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblException.Location = new System.Drawing.Point(16, 185);
      this.lblException.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblException.Name = "lblException";
      this.lblException.Size = new System.Drawing.Size(0, 20);
      this.lblException.TabIndex = 3;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(20, 282);
      this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(100, 28);
      this.button1.TabIndex = 4;
      this.button1.Text = "Cut Log";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(401, 339);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.lblException);
      this.Controls.Add(this.btnGo);
      this.Controls.Add(this.tbxMessages);
      this.Controls.Add(this.lblMessage);
      this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "InfoControllerDemo";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.TextBox tbxMessages;
    private System.Windows.Forms.Button btnGo;
    private System.Windows.Forms.Label lblException;
    private System.Windows.Forms.Button button1;
  }
}

