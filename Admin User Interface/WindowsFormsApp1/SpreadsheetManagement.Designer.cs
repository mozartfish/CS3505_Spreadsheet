﻿namespace WindowsFormsApp1
{
    partial class SpreadsheetManagement
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
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.CreateUser_User = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.ChangeUser_User = new System.Windows.Forms.TextBox();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(14, 59);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(89, 31);
            this.textBox2.TabIndex = 48;
            this.textBox2.Text = "NAME:";
            // 
            // CreateUser_User
            // 
            this.CreateUser_User.Location = new System.Drawing.Point(109, 59);
            this.CreateUser_User.Name = "CreateUser_User";
            this.CreateUser_User.Size = new System.Drawing.Size(476, 31);
            this.CreateUser_User.TabIndex = 46;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(858, 59);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(89, 31);
            this.textBox4.TabIndex = 44;
            this.textBox4.Text = "NAME:";
            // 
            // ChangeUser_User
            // 
            this.ChangeUser_User.Location = new System.Drawing.Point(953, 59);
            this.ChangeUser_User.Name = "ChangeUser_User";
            this.ChangeUser_User.Size = new System.Drawing.Size(597, 31);
            this.ChangeUser_User.TabIndex = 42;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Location = new System.Drawing.Point(858, 12);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(692, 41);
            this.richTextBox3.TabIndex = 39;
            this.richTextBox3.Text = "Remove SS";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(573, 41);
            this.richTextBox1.TabIndex = 37;
            this.richTextBox1.Text = "Create New Spreadsheet";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1556, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(173, 96);
            this.button2.TabIndex = 33;
            this.button2.Text = "CHANGE";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(591, 13);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(174, 77);
            this.button4.TabIndex = 32;
            this.button4.Text = "CREATE";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(12, 127);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1717, 954);
            this.listBox1.TabIndex = 31;
            // 
            // SpreadsheetManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1761, 1116);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.CreateUser_User);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.ChangeUser_User);
            this.Controls.Add(this.richTextBox3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.listBox1);
            this.Name = "SpreadsheetManagement";
            this.Text = "SpreadsheetManagement";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetManagement_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox CreateUser_User;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox ChangeUser_User;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox listBox1;
    }
}