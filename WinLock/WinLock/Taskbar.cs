using System;
using System.Runtime.InteropServices;

namespace WinLock
{
	public class Taskbar
	{
		[DllImport("user32.dll")]
		private static extern int FindWindow(string className, string windowText);

		[DllImport("user32.dll")]
		private static extern int ShowWindow(int hwnd, int command);

		[DllImport("user32.dll")]
		public static extern int FindWindowEx(int parentHandle, int childAfter, string className, int windowTitle);

		[DllImport("user32.dll")]
		private static extern int GetDesktopWindow();

		private const int SW_HIDE = 0;
		private const int SW_SHOW = 1;

		protected static int Handle
		{
			get
			{
				const String TaskbarWindowName = "Shell_TrayWnd";
				return FindWindow(TaskbarWindowName, String.Empty);
			}
		}

		protected static int StartButtonHandle
		{
			get
			{
				const String buttonClassName = "button";
				int desktopHandle = GetDesktopWindow();
				int startButtonHandle = FindWindowEx(desktopHandle, 0, buttonClassName, 0);
				return startButtonHandle;
			}
		}

		private Taskbar()
		{
			// hide ctor
		}

		public static void Show()
		{
			ShowWindow(Handle, SW_SHOW);
			ShowWindow(StartButtonHandle, SW_SHOW);
		}

		public static void Hide()
		{
			ShowWindow(Handle, SW_HIDE);
			ShowWindow(StartButtonHandle, SW_HIDE);
		}
	}
}