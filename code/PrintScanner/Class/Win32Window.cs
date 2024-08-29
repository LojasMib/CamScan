using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamScan.Class
{
    class Win32Window : IWin32Window
    {
        private readonly IntPtr _handle;

        public Win32Window(IntPtr handle)
        {
            _handle = handle;
        }
        public IntPtr Handle => _handle;
    }
}
