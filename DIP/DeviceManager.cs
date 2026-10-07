using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace WebCamLib
{
    public class DeviceManager
    {
        [DllImport("avicap32.dll", CharSet = CharSet.Ansi)]
        protected static extern bool capGetDriverDescriptionA(short wDriverIndex,
            StringBuilder lpszName, int cbName, StringBuilder lpszVer, int cbVer);

        static ArrayList devices = new ArrayList();

        public static Device[] GetAllDevices()
        {
            devices.Clear();
            for (short i = 0; i < 10; i++)
            {
                var dName = new StringBuilder(100);
                var dVersion = new StringBuilder(100);
                if (capGetDriverDescriptionA(i, dName, 100, dVersion, 100))
                {
                    Device d = new Device(i);
                    d.Name = dName.ToString().Trim();
                    d.Version = dVersion.ToString().Trim();
                    devices.Add(d);
                }
            }

            return (Device[])devices.ToArray(typeof(Device));
        }

        public static Device? GetDevice(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex >= devices.Count) return null;
            return (Device?)devices[deviceIndex];
        }
    }
}
