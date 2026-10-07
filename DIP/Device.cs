using System;
using System.Runtime.InteropServices;

namespace WebCamLib
{
    public class Device
    {
        private const short WM_CAP = 0x400;
        private const int WM_CAP_DRIVER_CONNECT = 0x40a;
        private const int WM_CAP_DRIVER_DISCONNECT = 0x40b;
        private const int WM_CAP_EDIT_COPY = WM_CAP + 30;
        private const int WM_CAP_SET_PREVIEW = 0x432;
        private const int WM_CAP_SET_OVERLAY = 0x433;
        private const int WM_CAP_SET_PREVIEWRATE = 0x434;
        private const int WM_CAP_SET_SCALE = 0x435;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CAP_SEQUENCE = WM_CAP + 62;
        private const int WM_CAP_FILE_SAVEAS = WM_CAP + 23;
        private const int SWP_NOMOVE = 0x20;
        private const int SWP_NOSIZE = 1;
        private const int SWP_NOZORDER = 0x40;
        private const int HWND_BOTTOM = 1;

        [DllImport("avicap32.dll", CharSet = CharSet.Ansi)]
        protected static extern IntPtr capCreateCaptureWindowA(string lpszWindowName,
            int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);

        [DllImport("user32", EntryPoint = "SendMessageA")]
        protected static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

        [DllImport("user32")]
        protected static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32")]
        protected static extern bool DestroyWindow(IntPtr hwnd);

        int index;
        IntPtr deviceHandle;

        public Device()
        {
        }

        public Device(int index)
        {
            this.index = index;
        }

        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _version = string.Empty;

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Init(int windowHeight, int windowWidth, IntPtr handle)
        {
            string deviceIndex = Convert.ToString(this.index);
            deviceHandle = capCreateCaptureWindowA(deviceIndex, WS_VISIBLE | WS_CHILD, 0, 0, windowWidth, windowHeight, handle, 0);

            if (SendMessage(deviceHandle, WM_CAP_DRIVER_CONNECT, this.index, IntPtr.Zero) > 0)
            {
                SendMessage(deviceHandle, WM_CAP_SET_SCALE, -1, IntPtr.Zero);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEWRATE, 0x42, IntPtr.Zero);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEW, -1, IntPtr.Zero);

                SetWindowPos(deviceHandle, 1, 0, 0, windowWidth, windowHeight, 6);
            }
        }

        public void ShowWindow(System.Windows.Forms.Control windowsControl)
        {
            Init(windowsControl.Height, windowsControl.Width, windowsControl.Handle);
        }

        public void Stop()
        {
            SendMessage(deviceHandle, WM_CAP_DRIVER_DISCONNECT, this.index, IntPtr.Zero);
            DestroyWindow(deviceHandle);
        }

        public void Sendmessage()
        {
            SendMessage(deviceHandle, WM_CAP_EDIT_COPY, 0, IntPtr.Zero);
        }
    }
}
