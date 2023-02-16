using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DLUTModernWebvpnBrowser.Helpers
{
    internal static class NativeMethods
    {
        internal delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        internal struct RTL_OSVERSIONINFOEX
        {
            public uint dwMajorVersion;

            public uint dwMinorVersion;

            public uint dwBuildNumber;

            public uint dwRevision;

            public uint dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;

            public uint dwOSVersionInfoSize { get; init; }

            public RTL_OSVERSIONINFOEX(uint dwMajorVersion, uint dwMinorVersion, uint dwBuildNumber, uint dwRevision, uint dwPlatformId, string szCSDVersion)
            {
                this = default(RTL_OSVERSIONINFOEX);
                dwOSVersionInfoSize = (uint)Marshal.SizeOf<RTL_OSVERSIONINFOEX>();
                this.dwMajorVersion = dwMajorVersion;
                this.dwMinorVersion = dwMinorVersion;
                this.dwBuildNumber = dwBuildNumber;
                this.dwRevision = dwRevision;
                this.dwPlatformId = dwPlatformId;
                this.szCSDVersion = szCSDVersion;
            }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
        [Conditional("CodeGeneration")]
        public class FriendlyAttribute : Attribute
        {
            public FriendlyFlags Flags { get; }

            public int ArrayLengthParameter { get; set; } = -1;


            public FriendlyAttribute(FriendlyFlags flags)
            {
                Flags = flags;
            }
        }

        [Flags]
        public enum FriendlyFlags
        {
            Array = 0x1,
            In = 0x2
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_NOMOVE = 0x2u
        }

        internal struct DispatcherQueueOptions
        {
            internal int dwSize;

            internal int threadType;

            internal int apartmentType;
        }

        internal enum Monitor_DPI_Type
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = 0
        }

        [Flags]
        internal enum WindowLongIndexFlags
        {
            GWL_WNDPROC = -4
        }

        internal struct MINMAXINFO
        {
            public POINT ptReserved;

            public POINT ptMaxSize;

            public POINT ptMaxPosition;

            public POINT ptMinTrackSize;

            public POINT ptMaxTrackSize;
        }

        public enum WindowMessage
        {
            WM_GETMINMAXINFO = 36
        }

        public struct POINT
        {
            public int x;

            public int y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.x, point.y);
            }

            public static implicit operator POINT(Point point)
            {
                POINT result = default(POINT);
                result.x = point.X;
                result.y = point.Y;
                return result;
            }
        }

        internal static readonly IntPtr HWND_TOP = new IntPtr(0);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);

        [DllImport("NTdll.dll")]
        internal static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

        [DllImport("CoreMessaging.dll")]
        internal static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In][Out][MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        [DllImport("User32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern int GetCurrentPackageFullName(ref uint packageFullNameLength, [Optional] StringBuilder packageFullName);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        internal static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        internal static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, newProc);
            }

            return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
    }
}
