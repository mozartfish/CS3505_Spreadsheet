namespace WindowsFormsApp1
{
    partial class WelcomePage
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
            this.button1 = new System.Windows.Forms.Button();
            this.IP = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(499, 203);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 81);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect To Server";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ConnectButton_click);
            // 
            // IP
            // 
            this.IP.Location = new System.Drawing.Point(121, 203);
            this.IP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.IP.Multiline = true;
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(366, 33);
            this.IP.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(121, 76);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(534, 116);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "Welcome to the Spreadsheet admin front end.\r\nEnter the hostname, and port and pre" +
    "ss continue.";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(121, 248);
            this.Port.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Port.Multiline = true;
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(366, 33);
            this.Port.TabIndex = 3;
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 393);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.IP);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WelcomePage";
            this.Text = "WelcomePage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox IP;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox Port;
    }
}