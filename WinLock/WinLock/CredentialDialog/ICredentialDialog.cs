using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinLock.CredentialDialog
{
	public interface ICredentialDialog
	{
		NetworkCredential GetNetworkCredentials(String server, String dialogTitle, String dialogText);
	}
}
