using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinLock.CredentialDialog
{
	class VistaAndHigherCredentialDialog : ICredentialDialog
	{
		[DllImport("ole32.dll")]
		public static extern void CoTaskMemFree(IntPtr ptr);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct CREDUI_INFO
		{
			public int cbSize;
			public IntPtr hwndParent;
			public string pszMessageText;
			public string pszCaptionText;
			public IntPtr hbmBanner;
		}


		[DllImport("credui.dll", CharSet = CharSet.Auto)]
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
																	 int flags);

		

		private static void GetCredentialsVistaAndUp(string serverName, string CaptionText, string DisplayedMessage, out NetworkCredential networkCredential)
		{
			int maxUsername = 100;
			int maxPassword = 100;
			int maxDomain = 100;
			CREDUI_INFO credui = new CREDUI_INFO();
			/*credui.pszCaptionText = "Please enter the credentails for " + serverName;
			credui.pszMessageText = "DisplayedMessage";*/
			credui.pszCaptionText = CaptionText;
			credui.pszMessageText = DisplayedMessage;
			credui.cbSize = Marshal.SizeOf(credui);
			uint authPackage = 0;
			IntPtr outCredBuffer = new IntPtr();
			uint outCredSize;
			bool save = false;
			int result = CredUIPromptForWindowsCredentials(ref credui,
														   0,
														   ref authPackage,
														   IntPtr.Zero,
														   0,
														   out outCredBuffer,
														   out outCredSize,
														   ref save,
														   0x200/* CurrentUser */);

			var usernameBuf = new StringBuilder(maxUsername);
			var passwordBuf = new StringBuilder(maxPassword);
			var domainBuf = new StringBuilder(maxDomain);

			if (result == 0)
			{
				if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUsername,
												   domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
				{
					//TODO: ms documentation says we should call this but i can't get it to work
					//SecureZeroMem(outCredBuffer, outCredSize);

					//clear the memory allocated by CredUIPromptForWindowsCredentials 
					CoTaskMemFree(outCredBuffer);
					networkCredential = new NetworkCredential()
					{
						UserName = usernameBuf.ToString(),
						Password = passwordBuf.ToString(),
						Domain = domainBuf.ToString()
					};
					return;
				}
			}

			networkCredential = null;
		}

		public NetworkCredential GetNetworkCredentials(String server, String dialogTitle, String dialogText)
		{
			NetworkCredential credentials;
			GetCredentialsVistaAndUp(server, dialogTitle, dialogText, out credentials);
			return credentials;
		}
	}
}
