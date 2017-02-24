using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinLock.CredentialDialog
{
	class XPCredentialDialog : ICredentialDialog
	{
		public NetworkCredential GetNetworkCredentials(String server, String dialogTitle, String dialogText)
		{
			throw new NotImplementedException();
		}
	}
}
