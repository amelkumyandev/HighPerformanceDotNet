using System.Runtime.InteropServices;

namespace NativeCode
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            dwMemoryLoad = 0;
            ullTotalPhys = ullAvailPhys = ullTotalPageFile = ullAvailPageFile = 0UL;
            ullTotalVirtual = ullAvailVirtual = ullAvailExtendedVirtual = 0UL;
        }
    }

    public class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }
}
