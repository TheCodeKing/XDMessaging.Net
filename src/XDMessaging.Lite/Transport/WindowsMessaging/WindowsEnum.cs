using System;
using System.Collections.Generic;
using Conditions;

namespace XDMessaging.Transport.WindowsMessaging
{
    internal sealed class WindowsEnum
    {
        public delegate void WindowFilterHandler(IntPtr hWnd, ref bool include);

        private readonly WindowFilterHandler filterHandler;
        private List<IntPtr> winEnumList;

        public WindowsEnum(WindowFilterHandler filterHandler) : this()
        {
            filterHandler.Requires("filterHandler").IsNotNull();

            this.filterHandler = filterHandler;
        }

        public WindowsEnum()
        {
        }

        public List<IntPtr> Enumerate()
        {
            winEnumList = new List<IntPtr>();
            Native.EnumWindows(OnWindowEnum, IntPtr.Zero);
            return winEnumList;
        }

        private int OnWindowEnum(IntPtr hWnd, IntPtr lParam)
        {
            var include = true;
            filterHandler?.Invoke(hWnd, ref include);

            if (include)
            {
                winEnumList.Add(hWnd);
            }

            return 1;
        }
    }
}