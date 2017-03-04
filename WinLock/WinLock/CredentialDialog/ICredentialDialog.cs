using System;

namespace WinLock.CredentialDialog
{
	public interface ICredentialDialog
	{
		bool VerifyCredentials(String dialogTitle, String dialogText);
	}
}
