using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLock
{
	public partial class FormCredentialDialog : Form
	{
		public String Username { get { return CBUsername.Text; } }

		public String Password { get { return TBPassword.Text; } }

		public void ClearPassword()
		{
			TBPassword.Text = "";
		}

		public FormCredentialDialog()
		{
			InitializeComponent();
			this.Text = Properties.Resources.DialogTitle;
			lblMessage.Text = Properties.Resources.DialogText;
			CBUsername.Items.Add(Environment.UserName);
			CBUsername.Text = Environment.UserName;
		}

		private void CBUsername_TextChanged(object sender, EventArgs e)
		{
			String unused = null;
			String domain = null;
			Program.ParseUsername(CBUsername.Text, out unused, out domain);
			lblLoggingOnTo.Text = String.Format(Properties.Resources.UILoggingOntoText, domain);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
