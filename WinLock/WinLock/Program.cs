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
		public static bool Debug { get { return System.Diagnostics.Debugger.IsAttached; } }
		internal static LockScreenForm LockScreen { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			LockScreen = new LockScreenForm(Screen.PrimaryScreen.Bounds.Size);
			LockScreen.PreAttemptUnlock += LockScreen_PreAttemptUnlock;
			if (Properties.Settings.Default.ProtectProcess && !Debug)
			{

			}
			Taskbar.Hide();
			LockScreen.ShowDialog();
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
			if (!dialog.VerifyCredentials(Properties.Resources.DialogTitle, Properties.Resources.DialogText)) return;
			LockScreen.Unlock();
		}
	}
}
