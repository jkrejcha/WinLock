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
			ICredentialDialog dialog = GetCredentialDialog();
			if (!dialog.VerifyCredentials(Properties.Resources.DialogTitle, Properties.Resources.DialogText)) return;
			LockScreen.Unlock();
		}

		/// <summary>
		/// Gets the correct credential dialog to use based on the OS version and the
		/// version of Windows.
		/// </summary>
		/// <returns>
		/// <list type="number">
		/// <item>
		/// <description>
		/// If <see cref="ForceCustomDialog"/> is set, on NT 3, 3.5, or 4, or if not on Windows NT based operating system, <see cref="BasicCredentialDialog"/>
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// If on Windows XP, <see cref="XPCredentialDialog"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// If on Windows Vista or above, <see cref="VistaAndHigherCredentialDialog"/>.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		private static ICredentialDialog GetCredentialDialog()
		{
			OperatingSystem osVersion = Environment.OSVersion;
			if (ForceCustomDialog || osVersion.Platform != PlatformID.Win32NT) return new BasicCredentialDialog();
			Version versionInfo = osVersion.Version;
			const int PreVistaMajor = 5;
			const int XPMinor = 1;
			if (versionInfo.Major <= PreVistaMajor)
			{
				if (versionInfo.Major == PreVistaMajor &&
					versionInfo.Minor >= XPMinor) // Windows XP
				{
					return new XPCredentialDialog();
				}
				else // Windows NT 3, 3.5, 4
				{
					return new BasicCredentialDialog();
				}
			}
			else // Windows Vista+
			{
				return new VistaAndHigherCredentialDialog();
			}
		}

		/// <summary>
		/// Parses a username from an entered string. Acceptable formats are
		/// domain\username, username@domain, or no domain specified (assumes current domain).<br/>
		/// If the domain is <code>.</code>, then the current machine name will be used.
		/// </summary>
		/// <param name="initialStr">The string to parse.</param>
		/// <param name="username">A <see cref="String"/> containing the parsed username.</param>
		/// <param name="domain">A <see cref="String"/> containing the parsed domain name.</param>
		public static void ParseUsername(String initialStr, out string username, out string domain)
		{
			username = initialStr;
			domain = null;
			if (username.Contains("\\")) // domain\username format
			{
				int indexOfSlash = username.IndexOf("\\");
				domain = username.Substring(0, indexOfSlash);
				username = username.Substring(indexOfSlash);
			}
			else if (username.Contains("@")) // username@domain format
			{
				int indexOfAt = username.IndexOf("@");
				username = username.Substring(0, indexOfAt);
				domain = username.Substring(indexOfAt);
			}
			else // no domain specified, use current
			{
				domain = Environment.UserDomainName;
			}
			if (domain == ".") domain = Environment.MachineName; // use machine name if domain is '.'
		}

		/// <summary>
		/// Attempts to logon using the LogonUser API, given a username, password,
		/// and domain.
		/// </summary>
		/// <param name="username">Username to use</param>
		/// <param name="password">Password to use</param>
		/// <param name="domain">Domain to attempt to logon to</param>
		/// <returns>A Win32 error code which determines the success of the logon
		/// request. An error of 0 indicated success.<br/>
		/// See <a href="https://docs.microsoft.com/en-us/windows/desktop/debug/system-error-codes">this listing</a>
		/// for more information.</returns>
		public static int TryLogon(String username, String password, String domain)
		{
			IntPtr unused = IntPtr.Zero;
			NativeMethods.LogonUser(username, domain, password, (Int32)LogonType.Unlock, 0x00, out unused);
			return System.Runtime.InteropServices.Marshal.GetLastWin32Error();
		}
	}
}
