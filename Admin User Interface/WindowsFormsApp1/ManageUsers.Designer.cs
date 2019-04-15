namespace WindowsFormsApp1
{
    partial class ManageUsers
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.DeleteUser_Pass = new System.Windows.Forms.TextBox();
            this.DeleteUser_User = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.ChangeUser_OldPass = new System.Windows.Forms.TextBox();
            this.ChangeUser_User = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.CreateUser_pass = new System.Windows.Forms.TextBox();
            this.CreateUser_User = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.ChangeUser_newPass = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 6);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(432, 433);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(689, 45);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(105, 40);
            this.button4.TabIndex = 4;
            this.button4.Text = "CREATE";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(689, 149);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(105, 40);
            this.button2.TabIndex = 9;
            this.button2.Text = "CHANGE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DeleteUser_Pass
            // 
            this.DeleteUser_Pass.Location = new System.Drawing.Point(496, 285);
            this.DeleteUser_Pass.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DeleteUser_Pass.Name = "DeleteUser_Pass";
            this.DeleteUser_Pass.Size = new System.Drawing.Size(192, 20);
            this.DeleteUser_Pass.TabIndex = 14;
            // 
            // DeleteUser_User
            // 
            this.DeleteUser_User.Location = new System.Drawing.Point(496, 266);
            this.DeleteUser_User.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DeleteUser_User.Name = "DeleteUser_User";
            this.DeleteUser_User.Size = new System.Drawing.Size(192, 20);
            this.DeleteUser_User.TabIndex = 13;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(689, 266);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(105, 40);
            this.button3.TabIndex = 12;
            this.button3.Text = "DELETE";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(448, 8);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(348, 33);
            this.richTextBox1.TabIndex = 16;
            this.richTextBox1.Text = "Create User";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(448, 231);
            this.richTextBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(348, 33);
            this.richTextBox2.TabIndex = 17;
            this.richTextBox2.Text = "Delete User";
            // 
            // richTextBox3
            // 
            this.richTextBox3.Location = new System.Drawing.Point(448, 108);
            this.richTextBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(348, 33);
            this.richTextBox3.TabIndex = 18;
            this.richTextBox3.Text = "Change Password";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(448, 266);
            this.textBox7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(46, 20);
            this.textBox7.TabIndex = 19;
            this.textBox7.Text = "USER:";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(448, 285);
            this.textBox8.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(46, 20);
            this.textBox8.TabIndex = 20;
            this.textBox8.Text = "PASS:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(448, 161);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(66, 20);
            this.textBox3.TabIndex = 24;
            this.textBox3.Text = "OLD PASS:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(448, 142);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(46, 20);
            this.textBox4.TabIndex = 23;
            this.textBox4.Text = "USER:";
            // 
            // ChangeUser_OldPass
            // 
            this.ChangeUser_OldPass.Location = new System.Drawing.Point(514, 161);
            this.ChangeUser_OldPass.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChangeUser_OldPass.Name = "ChangeUser_OldPass";
            this.ChangeUser_OldPass.Size = new System.Drawing.Size(174, 20);
            this.ChangeUser_OldPass.TabIndex = 22;
            // 
            // ChangeUser_User
            // 
            this.ChangeUser_User.Location = new System.Drawing.Point(496, 142);
            this.ChangeUser_User.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChangeUser_User.Name = "ChangeUser_User";
            this.ChangeUser_User.Size = new System.Drawing.Size(192, 20);
            this.ChangeUser_User.TabIndex = 21;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(448, 64);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(46, 20);
            this.textBox1.TabIndex = 28;
            this.textBox1.Text = "PASS:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(448, 45);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(46, 20);
            this.textBox2.TabIndex = 27;
            this.textBox2.Text = "USER:";
            // 
            // CreateUser_pass
            // 
            this.CreateUser_pass.Location = new System.Drawing.Point(496, 64);
            this.CreateUser_pass.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CreateUser_pass.Name = "CreateUser_pass";
            this.CreateUser_pass.Size = new System.Drawing.Size(192, 20);
            this.CreateUser_pass.TabIndex = 26;
            // 
            // CreateUser_User
            // 
            this.CreateUser_User.Location = new System.Drawing.Point(496, 45);
            this.CreateUser_User.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CreateUser_User.Name = "CreateUser_User";
            this.CreateUser_User.Size = new System.Drawing.Size(192, 20);
            this.CreateUser_User.TabIndex = 25;
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(448, 180);
            this.textBox13.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(66, 20);
            this.textBox13.TabIndex = 30;
            this.textBox13.Text = "NEW PASS:";
            // 
            // ChangeUser_newPass
            // 
            this.ChangeUser_newPass.Location = new System.Drawing.Point(514, 180);
            this.ChangeUser_newPass.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChangeUser_newPass.Name = "ChangeUser_newPass";
            this.ChangeUser_newPass.Size = new System.Drawing.Size(174, 20);
            this.ChangeUser_newPass.TabIndex = 29;
            // 
            // ManageUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox13);
            this.Controls.Add(this.ChangeUser_newPass);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.CreateUser_pass);
            this.Controls.Add(this.CreateUser_User);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.ChangeUser_OldPass);
            this.Controls.Add(this.ChangeUser_User);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.richTextBox3);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.DeleteUser_Pass);
            this.Controls.Add(this.DeleteUser_User);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.listBox1);
            this.Name = "ManageUsers";
            this.Text = "ManageUsers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManageUsers_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox DeleteUser_Pass;
        private System.Windows.Forms.TextBox DeleteUser_User;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox ChangeUser_OldPass;
        private System.Windows.Forms.TextBox ChangeUser_User;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox CreateUser_pass;
        private System.Windows.Forms.TextBox CreateUser_User;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox ChangeUser_newPass;
    }
}