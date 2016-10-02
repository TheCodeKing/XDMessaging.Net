using System;

namespace XDMessaging.Transport.WindowsMessaging
{
    internal sealed class WindowEnumFilter
    {
        private readonly string property;

        public WindowEnumFilter(string property)
        {
            this.property = property;
        }

        public void WindowFilterHandler(IntPtr hWnd, ref bool include)
        {
            if (Native.GetProp(hWnd, property) == 0)
            {
                include = false;
            }
        }
    }
}