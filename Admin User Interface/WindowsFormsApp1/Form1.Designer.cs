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
            this.button4 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(625, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Account Manegment";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AccountManagementButton);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.MenuText;
            this.button3.Location = new System.Drawing.Point(643, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(325, 78);
            this.button3.TabIndex = 2;
            this.button3.Text = "SHUT\r\nDOWN";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(625, 36);
            this.button2.TabIndex = 3;
            this.button2.Text = "SS management";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SpreadsheetManagmentButton);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(681, 25);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(254, 52);
            this.button5.TabIndex = 10;
            this.button5.Text = "SHUT\r\nDOWN";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.ShutDown);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 96);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(625, 20);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Active Users";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(643, 96);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(325, 20);
            this.textBox2.TabIndex = 12;
            this.textBox2.Text = "Spreadsheet Updates";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // currentStatusList
            // 
            this.currentStatusList.FormattingEnabled = true;
            this.currentStatusList.Location = new System.Drawing.Point(12, 132);
            this.currentStatusList.Name = "currentStatusList";
            this.currentStatusList.Size = new System.Drawing.Size(625, 472);
            this.currentStatusList.TabIndex = 13;
            // 
            // updateList
            // 
            this.updateList.FormattingEnabled = true;
            this.updateList.Location = new System.Drawing.Point(643, 132);
            this.updateList.Name = "updateList";
            this.updateList.Size = new System.Drawing.Size(325, 472);
            this.updateList.TabIndex = 14;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(598, 254);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(66, 38);
            this.button4.TabIndex = 15;
            this.button4.Text = "redraw";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(598, 194);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(66, 36);
            this.button6.TabIndex = 16;
            this.button6.Text = "populate model";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(598, 312);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(66, 63);
            this.button7.TabIndex = 17;
            this.button7.Text = "test Inserting at top";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.TESTinsertingTopOfList);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 620);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.updateList);
            this.Controls.Add(this.currentStatusList);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Administrator Front End";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
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
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
    }
}

