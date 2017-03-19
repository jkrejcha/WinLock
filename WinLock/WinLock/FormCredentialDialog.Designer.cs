namespace WinLock
{
	partial class FormCredentialDialog
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.CBUsername = new System.Windows.Forms.ComboBox();
			this.TBPassword = new System.Windows.Forms.TextBox();
			this.lblLoggingOnTo = new System.Windows.Forms.Label();
			this.lblUsername = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(317, 144);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(236, 144);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// CBUsername
			// 
			this.CBUsername.FormattingEnabled = true;
			this.CBUsername.Location = new System.Drawing.Point(114, 44);
			this.CBUsername.Name = "CBUsername";
			this.CBUsername.Size = new System.Drawing.Size(278, 21);
			this.CBUsername.TabIndex = 2;
			this.CBUsername.TextChanged += new System.EventHandler(this.CBUsername_TextChanged);
			// 
			// TBPassword
			// 
			this.TBPassword.Location = new System.Drawing.Point(114, 82);
			this.TBPassword.Name = "TBPassword";
			this.TBPassword.Size = new System.Drawing.Size(278, 20);
			this.TBPassword.TabIndex = 3;
			this.TBPassword.UseSystemPasswordChar = true;
			// 
			// lblLoggingOnTo
			// 
			this.lblLoggingOnTo.AutoSize = true;
			this.lblLoggingOnTo.Location = new System.Drawing.Point(111, 119);
			this.lblLoggingOnTo.Name = "lblLoggingOnTo";
			this.lblLoggingOnTo.Size = new System.Drawing.Size(134, 13);
			this.lblLoggingOnTo.TabIndex = 4;
			this.lblLoggingOnTo.Text = "Logging on to: <unknown>";
			// 
			// lblUsername
			// 
			this.lblUsername.AutoSize = true;
			this.lblUsername.Location = new System.Drawing.Point(12, 47);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(61, 13);
			this.lblUsername.TabIndex = 5;
			this.lblUsername.Text = "&User name:";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(12, 89);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 6;
			this.lblPassword.Text = "&Password:";
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Location = new System.Drawing.Point(12, 9);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(21, 13);
			this.lblMessage.TabIndex = 7;
			this.lblMessage.Text = "{1}";
			// 
			// FormCredentialDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(404, 179);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.lblUsername);
			this.Controls.Add(this.lblLoggingOnTo);
			this.Controls.Add(this.TBPassword);
			this.Controls.Add(this.CBUsername);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCredentialDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "{0}";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ComboBox CBUsername;
		private System.Windows.Forms.TextBox TBPassword;
		private System.Windows.Forms.Label lblLoggingOnTo;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblMessage;
	}
}

