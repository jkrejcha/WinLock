using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace WinLock.CredentialDialog
{
	class VistaAndHigherCredentialDialog : ICredentialDialog
	{
		[DllImport("ole32.dll")]
		private static extern void CoTaskMemFree(IntPtr ptr);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct CREDUI_INFO
		{
			public int cbSize;
			public IntPtr hwndParent;
			public string pszMessageText;
			public string pszCaptionText;
			public IntPtr hbmBanner;
		}

		[DllImport("credui.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
																   IntPtr pAuthBuffer,
																   uint cbAuthBuffer,
																   StringBuilder pszUserName,
																   ref int pcchMaxUserName,
																   StringBuilder pszDomainName,
																   ref int pcchMaxDomainame,
																   StringBuilder pszPassword,
																   ref int pcchMaxPassword);

		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
																	 int authError,
																	 ref uint authPackage,
																	 IntPtr InAuthBuffer,
																	 uint InAuthBufferSize,
																	 out IntPtr refOutAuthBuffer,
																	 out uint refOutAuthBufferSize,
																	 ref bool fSave,
																	 WindowsCredentialUIOptions flags);
		
		[DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LogonUser([MarshalAs(UnmanagedType.LPStr)] string pszUserName, [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
		[MarshalAs(UnmanagedType.LPStr)] string pszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern void SetLastError(int errorCode);


		private static LockScreenError GetCredentialsVistaAndUp(string captionText, string displayedMessage, int errorCode = 0)
		{
			int maxUsername = 104; // maximum username in Windows
			int maxPassword = 127; // as of Windows 7
			int maxDomain = 255; // maximum FQDN
			CREDUI_INFO credui = new CREDUI_INFO();
			credui.hwndParent = (new System.Windows.Forms.Form { TopMost = true, StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen }).Handle;
			//credui.hwndParent = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
			credui.pszCaptionText = captionText;
			credui.pszMessageText = displayedMessage;
			credui.cbSize = Marshal.SizeOf(credui);
			uint authPackage = 0;
			IntPtr outCredBuffer = new IntPtr();
			uint outCredSize;
			bool save = false;
			int result = CredUIPromptForWindowsCredentials(ref credui,
			                                               errorCode,
														   ref authPackage,
														   IntPtr.Zero,
														   0,
														   out outCredBuffer,
														   out outCredSize,
														   ref save,
														   WindowsCredentialUIOptions.EnumerateCurrentUser |
			                                               WindowsCredentialUIOptions.EnumerateAdmins);

			StringBuilder usernameBuf = new StringBuilder(maxUsername);
			StringBuilder passwordBuf = new StringBuilder(maxPassword);
			StringBuilder domainBuf = new StringBuilder(maxDomain);

			if (result == 0)
			{
				if (CredUnPackAuthenticationBuffer(0x01, outCredBuffer, outCredSize, usernameBuf, ref maxUsername,
												   domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
				{
					//TODO: ms documentation says we should call this but i can't get it to work
					//SecureZeroMem(outCredBuffer, outCredSize);

					//clear the memory allocated by CredUIPromptForWindowsCredentials 
					CoTaskMemFree(outCredBuffer);
					String domain = usernameBuf.ToString().Substring(0, usernameBuf.ToString().IndexOf("\\"));
					NetworkCredential credential = new NetworkCredential()
					{
						UserName = usernameBuf.ToString().Substring(usernameBuf.ToString().IndexOf("\\")),
						Password = passwordBuf.ToString(),
						Domain = domain
					};
					IntPtr unused = IntPtr.Zero;
					return LogonUser(credential.UserName, credential.Domain, credential.Password, 0x03, 0x00, out unused) ? LockScreenError.None : LockScreenError.AuthenticationError;
				}
				else { return LockScreenError.AuthenticationError; }
			}
			return LockScreenError.UserCancelled;
		}

		public bool VerifyCredentials(String dialogTitle, String dialogText)
		{
			LockScreenError lastError = LockScreenError.AuthenticationError;
			while (lastError == LockScreenError.AuthenticationError)
			{
				lastError = GetCredentialsVistaAndUp(dialogTitle, dialogText, Marshal.GetLastWin32Error());
			}
			if (lastError == LockScreenError.UserCancelled) SetLastError(0x00); // hacky way of resetting the error
			return lastError == LockScreenError.None;
		}

		[Flags]
		public enum WindowsCredentialUIOptions
		{
			/// <summary>
			/// Generic credentials. Cannot be used with <see cref="SecurePrompt"/>.
			/// </summary>
			Generic = 0x1,
			CheckBox = 0x2,
			AuthPackageOnly = 0x10,
			InCredOnly = 0x20,
			EnumerateAdmins = 0x100,
			EnumerateCurrentUser = 0x200,
			SecurePrompt = 0x1000,
			PrePrompting = 0x2000,
			Pack32WoW = 0x10000000,
		}
	}
}
