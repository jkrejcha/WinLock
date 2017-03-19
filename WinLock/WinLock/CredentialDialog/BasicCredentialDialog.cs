using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinLock.NativeMethods;

namespace WinLock.CredentialDialog
{
	class BasicCredentialDialog : ICredentialDialog
	{
		private int ShowCredentialDialog()
		{
			FormCredentialDialog dialog = new FormCredentialDialog();
			if (dialog.ShowDialog() == DialogResult.Cancel)
			{
				return Error.Cancelled;
			}
			String username;
			String domain;
			Program.ParseUsername(dialog.Username, out username, out domain);
			return Program.TryLogon(username, dialog.Password, domain);
		}

		public bool VerifyCredentials(string dialogTitle, string dialogText)
		{
			int lastError = 0x01;
			while (lastError != Error.Success || lastError != Error.Cancelled)
			{
				int lastErrorCode = Marshal.GetLastWin32Error();
				if (lastErrorCode != 0x00)
				{
					MessageBox.Show(new Win32Exception(lastErrorCode).Message, "Logon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                lastError = ShowCredentialDialog();
			}
			if (lastError == Error.Cancelled) SetLastError(Error.Success); // hacky way of resetting the error
			return lastError == Error.Success;
		}
	}
}
