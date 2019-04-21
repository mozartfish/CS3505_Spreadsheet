namespace WindowsFormsApp1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.currentStatusList = new System.Windows.Forms.ListBox();
            this.updateList = new System.Windows.Forms.ListBox();
            this.button8 = new System.Windows.Forms.Button();
            this.IP = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1614, 402);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(350, 160);
            this.button1.TabIndex = 0;
            this.button1.Text = "Account Manegment";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AccountManagementButton);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.MenuText;
            this.button3.Location = new System.Drawing.Point(1614, 950);
            this.button3.Margin = new System.Windows.Forms.Padding(6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(350, 150);
            this.button3.TabIndex = 2;
            this.button3.Text = "SHUT\r\nDOWN";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1614, 658);
            this.button2.Margin = new System.Windows.Forms.Padding(6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(350, 160);
            this.button2.TabIndex = 3;
            this.button2.Text = "SS management";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SpreadsheetManagmentButton);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(1662, 969);
            this.button5.Margin = new System.Windows.Forms.Padding(6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(248, 108);
            this.button5.TabIndex = 10;
            this.button5.Text = "SHUTDOWN\r\nSERVER";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.ShutDown);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(24, 40);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(846, 31);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Active Users";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(886, 40);
            this.textBox2.Margin = new System.Windows.Forms.Padding(6);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(646, 31);
            this.textBox2.TabIndex = 12;
            this.textBox2.Text = "Spreadsheet Updates";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // currentStatusList
            // 
            this.currentStatusList.FormattingEnabled = true;
            this.currentStatusList.ItemHeight = 25;
            this.currentStatusList.Location = new System.Drawing.Point(24, 104);
            this.currentStatusList.Margin = new System.Windows.Forms.Padding(6);
            this.currentStatusList.Name = "currentStatusList";
            this.currentStatusList.Size = new System.Drawing.Size(846, 1079);
            this.currentStatusList.TabIndex = 13;
            this.currentStatusList.SelectedIndexChanged += new System.EventHandler(this.currentStatusList_SelectedIndexChanged);
            // 
            // updateList
            // 
            this.updateList.FormattingEnabled = true;
            this.updateList.ItemHeight = 25;
            this.updateList.Location = new System.Drawing.Point(886, 102);
            this.updateList.Margin = new System.Windows.Forms.Padding(6);
            this.updateList.Name = "updateList";
            this.updateList.Size = new System.Drawing.Size(646, 1079);
            this.updateList.TabIndex = 14;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(1608, 196);
            this.button8.Margin = new System.Windows.Forms.Padding(6);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(350, 87);
            this.button8.TabIndex = 18;
            this.button8.Text = "Connect To Server";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.ConnectToServer_buttone);
            // 
            // IP
            // 
            this.IP.Location = new System.Drawing.Point(1608, 71);
            this.IP.Margin = new System.Windows.Forms.Padding(6);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(346, 31);
            this.IP.TabIndex = 19;
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(1608, 146);
            this.Port.Margin = new System.Windows.Forms.Padding(6);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(346, 31);
            this.Port.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1748, 115);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 25);
            this.label1.TabIndex = 21;
            this.label1.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1748, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 25);
            this.label2.TabIndex = 22;
            this.label2.Text = "IP:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1990, 1204);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.IP);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.updateList);
            this.Controls.Add(this.currentStatusList);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Administrator Front End";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ListBox currentStatusList;
        private System.Windows.Forms.ListBox updateList;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox IP;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

