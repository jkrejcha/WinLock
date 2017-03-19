using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using static WinLock.NativeMethods;

namespace WinLock.CredentialDialog
{
	class VistaAndHigherCredentialDialog : ICredentialDialog
	{
		private static int ShowCredentialDialog(string captionText, string displayedMessage, int errorCode = 0)
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
					return Program.TryLogon(credential.UserName, credential.Password, credential.Domain);
				}
				else { return result; }
			}
			return Error.Cancelled;
		}

		public bool VerifyCredentials(String dialogTitle, String dialogText)
		{
			int lastError = 0x01;
			while (lastError != Error.Success || lastError != Error.Cancelled)
			{
				lastError = ShowCredentialDialog(dialogTitle, dialogText, Marshal.GetLastWin32Error());
			}
			if (lastError == Error.Cancelled) SetLastError(Error.Success); // hacky way of resetting the error
			return lastError == Error.Success;
		}
	}
}
