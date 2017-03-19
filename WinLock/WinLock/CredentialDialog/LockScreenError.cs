using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinLock
{
	public static class Error
	{
		/// <summary>
		/// Indicates a successful operation.
		/// </summary>
		/// <remarks>
		/// The text for this is a localized string similar to 
		/// "The operation completed successfully."
		/// </remarks>
		public const int Success = 0x00;
		/// <summary>
		/// Indicates an operation cancelled by the user.
		/// </summary>
		/// <remarks>
		/// The text for this is a localized string similar to 
		/// "The operation was cancelled by the user."
		/// </remarks>
		public const int Cancelled = 0x4C7;
	}
}
