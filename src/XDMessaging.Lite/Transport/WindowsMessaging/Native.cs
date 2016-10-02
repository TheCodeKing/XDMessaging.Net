using System;
using System.Runtime.InteropServices;

namespace XDMessaging.Transport.WindowsMessaging
{
    // ReSharper disable InconsistentNaming
    internal static class Native
    {
        public delegate int EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {

            SMTO_NORMAL = 0x0000,

            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }

        public const uint WM_COPYDATA = 0x4A;

        [DllImport("user32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetProp(IntPtr hwnd, string lpString);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int RemoveProp(IntPtr hwnd, string lpString);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int SendMessageTimeout(
            IntPtr hwnd,
            uint wMsg,
            IntPtr wParam,
            ref COPYDATASTRUCT lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int SetProp(IntPtr hwnd, string lpString, int hData);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
    }
    // ReSharper restore InconsistentNaming
}