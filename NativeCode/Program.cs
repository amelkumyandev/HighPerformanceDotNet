using NativeCode;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        MEMORYSTATUSEX memoryStatus = new MEMORYSTATUSEX();

        // Call the native function
        if (NativeMethods.GlobalMemoryStatusEx(ref memoryStatus))
        {
            Console.WriteLine("Memory Load: {0}%", memoryStatus.dwMemoryLoad);
            Console.WriteLine("Total Physical Memory (MB): {0}", memoryStatus.ullTotalPhys / (1024 * 1024));
            Console.WriteLine("Available Physical Memory (MB): {0}", memoryStatus.ullAvailPhys / (1024 * 1024));
            Console.WriteLine("Total Virtual Memory (MB): {0}", memoryStatus.ullTotalVirtual / (1024 * 1024));
            Console.WriteLine("Available Virtual Memory (MB): {0}", memoryStatus.ullAvailVirtual / (1024 * 1024));
        }
        else
        {
            Console.WriteLine("Failed to retrieve memory status.");
            Console.WriteLine($"Error Code: {Marshal.GetLastWin32Error()}");
        }
    }
}


