using System;
using System.Windows.Forms;
using WinLock.CredentialDialog;

namespace WinLock
{
	static class Program
	{
#if DEBUG
		public static bool Debug { get { return System.Diagnostics.Debugger.IsAttached; } }
#else
		public static bool Debug { get { return false; } }
#endif
		public static bool ForceCustomDialog { get; private set; }
		internal static LockScreenForm LockScreen { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(String[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			LockScreen = new LockScreenForm(Screen.PrimaryScreen.Bounds.Size);
			LockScreen.AttemptUnlock += LockScreen_AttemptUnlock;
			System.Threading.Thread secThread = null;
			if (Properties.Settings.Default.ProtectProcess && !Debug)
			{
				secThread = ProcessProtection.ProcessProtect.Start();
			}
			ForceCustomDialog = Debug && args.Length > 0 && args[0] == "-customdialog";
			if (!Debug) Taskbar.Hide();
			LockScreen.ShowDialog();
			Taskbar.Show();
			if (secThread != null) secThread.Abort();
        }

		private static void LockScreen_AttemptUnlock(object sender, EventArgs e)
		{
			ICredentialDialog dialog;
			OperatingSystem version = Environment.OSVersion;
			int majorOSVersion = version.Version.Major;
			if (ForceCustomDialog || majorOSVersion <= 5) // Before Windows Vista
			{
				if (ForceCustomDialog || version.Version.Minor < 1)
				{
					dialog = new BasicCredentialDialog();
				}
				else
				{
					dialog = new XPCredentialDialog();
				}
			}
			else // Windows Vista+
			{
				dialog = new VistaAndHigherCredentialDialog();
			}
			if (!dialog.VerifyCredentials(Properties.Resources.DialogTitle, Properties.Resources.DialogText)) return;
			LockScreen.Unlock();
		}

		public static void ParseUsername(String initialStr, out string username, out string domain)
		{
			username = initialStr;
			domain = null;
			if (username.Contains("\\"))
			{
				int indexOfSlash = username.IndexOf("\\");
				domain = username.Substring(0, indexOfSlash);
				username = username.Substring(indexOfSlash);
			}
			else
			{
				domain = Environment.UserDomainName;
			}
			if (domain == ".") domain = Environment.MachineName;
		}

		public static int TryLogon(String username, String password, String domain)
		{
			IntPtr unused = IntPtr.Zero;
			NativeMethods.LogonUser(username, domain, password, 0x02, 0x00, out unused);
			return System.Runtime.InteropServices.Marshal.GetLastWin32Error();
		}
	}
}
