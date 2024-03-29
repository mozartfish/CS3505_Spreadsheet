﻿namespace Display
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
            this.ConnectButton = new System.Windows.Forms.Button();
            this.Username = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.ServerAddress = new System.Windows.Forms.TextBox();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.spreadsheetList = new System.Windows.Forms.ListBox();
            this.NewSpreadsheet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(462, 21);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(100, 34);
            this.ConnectButton.TabIndex = 0;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(103, 21);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(319, 22);
            this.Username.TabIndex = 1;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(103, 55);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(319, 22);
            this.Password.TabIndex = 2;
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(20, 21);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(77, 17);
            this.UsernameLabel.TabIndex = 3;
            this.UsernameLabel.Text = "Username:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(23, 60);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(73, 17);
            this.PasswordLabel.TabIndex = 4;
            this.PasswordLabel.Text = "Password:";
            // 
            // ServerAddress
            // 
            this.ServerAddress.Location = new System.Drawing.Point(103, 89);
            this.ServerAddress.Name = "ServerAddress";
            this.ServerAddress.Size = new System.Drawing.Size(319, 22);
            this.ServerAddress.TabIndex = 5;
            this.ServerAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ServerAddress_KeyDown);
            // 
            // ServerLabel
            // 
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Location = new System.Drawing.Point(23, 93);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(54, 17);
            this.ServerLabel.TabIndex = 6;
            this.ServerLabel.Text = "Server:";
            // 
            // spreadsheetList
            // 
            this.spreadsheetList.FormattingEnabled = true;
            this.spreadsheetList.ItemHeight = 16;
            this.spreadsheetList.Location = new System.Drawing.Point(103, 145);
            this.spreadsheetList.Name = "spreadsheetList";
            this.spreadsheetList.Size = new System.Drawing.Size(612, 292);
            this.spreadsheetList.TabIndex = 7;
            this.spreadsheetList.DoubleClick += new System.EventHandler(this.spreadsheetList_SelectedIndexChanged);
            // 
            // NewSpreadsheet
            // 
            this.NewSpreadsheet.Location = new System.Drawing.Point(558, 111);
            this.NewSpreadsheet.Name = "NewSpreadsheet";
            this.NewSpreadsheet.Size = new System.Drawing.Size(157, 28);
            this.NewSpreadsheet.TabIndex = 8;
            this.NewSpreadsheet.Text = "New Spreadsheet";
            this.NewSpreadsheet.UseVisualStyleBackColor = true;
            this.NewSpreadsheet.Click += new System.EventHandler(this.NewSpreadsheet_Click);
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.NewSpreadsheet);
            this.Controls.Add(this.spreadsheetList);
            this.Controls.Add(this.ServerLabel);
            this.Controls.Add(this.ServerAddress);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.ConnectButton);
            this.Name = "WelcomePage";
            this.Text = "Welcome Page";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox ServerAddress;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.ListBox spreadsheetList;
        private System.Windows.Forms.Button NewSpreadsheet;
    }
}

