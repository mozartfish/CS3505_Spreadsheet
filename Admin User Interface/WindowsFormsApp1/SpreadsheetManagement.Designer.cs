namespace WindowsFormsApp1
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
            this.CreateSS_Name = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.DeleteSS_Name = new System.Windows.Forms.TextBox();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(14, 60);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(88, 31);
            this.textBox2.TabIndex = 48;
            this.textBox2.Text = "NAME:";
            // 
            // CreateSS_Name
            // 
            this.CreateSS_Name.Location = new System.Drawing.Point(108, 60);
            this.CreateSS_Name.Margin = new System.Windows.Forms.Padding(4);
            this.CreateSS_Name.Name = "CreateSS_Name";
            this.CreateSS_Name.Size = new System.Drawing.Size(476, 31);
            this.CreateSS_Name.TabIndex = 46;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(858, 60);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(88, 31);
            this.textBox4.TabIndex = 44;
            this.textBox4.Text = "NAME:";
            // 
            // DeleteSS_Name
            // 
            this.DeleteSS_Name.Location = new System.Drawing.Point(952, 60);
            this.DeleteSS_Name.Margin = new System.Windows.Forms.Padding(4);
            this.DeleteSS_Name.Name = "DeleteSS_Name";
            this.DeleteSS_Name.Size = new System.Drawing.Size(596, 31);
            this.DeleteSS_Name.TabIndex = 42;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Location = new System.Drawing.Point(858, 12);
            this.richTextBox3.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(692, 41);
            this.richTextBox3.TabIndex = 39;
            this.richTextBox3.Text = "Remove SS";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(572, 41);
            this.richTextBox1.TabIndex = 37;
            this.richTextBox1.Text = "Create New Spreadsheet";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1556, 13);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(172, 96);
            this.button2.TabIndex = 33;
            this.button2.Text = "CHANGE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.DeleteSS_button);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(592, 13);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(174, 77);
            this.button4.TabIndex = 32;
            this.button4.Text = "CREATE";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.CreateSS_button);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(12, 127);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1716, 954);
            this.listBox1.TabIndex = 31;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1381, 239);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 73);
            this.button1.TabIndex = 49;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1381, 380);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(143, 70);
            this.button3.TabIndex = 50;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SpreadsheetManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1684, 1031);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.CreateSS_Name);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.DeleteSS_Name);
            this.Controls.Add(this.richTextBox3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.listBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SpreadsheetManagement";
            this.Text = "SpreadsheetManagement";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetManagement_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox CreateSS_Name;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox DeleteSS_Name;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}