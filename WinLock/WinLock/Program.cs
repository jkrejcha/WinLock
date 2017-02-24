using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinLock.CredentialDialog;

namespace WinLock
{
	static class Program
	{
		private static LockScreenForm lockScreen;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			lockScreen = new LockScreenForm(Screen.PrimaryScreen.Bounds.Size);
			lockScreen.PreAttemptUnlock += LockScreen_PreAttemptUnlock;
			Taskbar.Hide();
			lockScreen.ShowDialog();
			Taskbar.Show();
        }

		private static void LockScreen_PreAttemptUnlock(object sender, EventArgs e)
		{
			ICredentialDialog dialog;
			OperatingSystem version = Environment.OSVersion;
			int majorOSVersion = version.Version.Major;
			if (majorOSVersion <= 5) // Before Windows Vista
			{
				if (version.Version.Minor >= 1) // Windows XP
				{
					dialog = new XPCredentialDialog();
				}
				else // Before Windows XP
				{
					dialog = new BasicCredentialDialog();
				}
			}
			else // Windows Vista+
			{
				dialog = new VistaAndHigherCredentialDialog();
			}
			NetworkCredential cred = dialog.GetNetworkCredentials(Environment.UserDomainName, Properties.Resources.DialogTitle, Properties.Resources.DialogText);
			if (cred == null) return;
			MessageBox.Show(cred.Domain + "\\" + cred.UserName + "\r\n" + cred.Password);
			//lockScreen.Unlock();
		}
	}
}
