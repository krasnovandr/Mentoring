using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Task1.Host
{
    static class Program
    {
        static void Main()
        {

            //var a =PowerManagmentApi.GetLastSleepTimeTotalSeconds();
            //Thread.Sleep(2000);
            //var b =PowerManagmentApi.GetLastWakeTimeTotalSeconds();
            //Console.WriteLine(a);
            //Console.WriteLine(b);
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));

            var outputBufferSize = (UInt32)Marshal.SizeOf(typeof(ulong));
            PowrProfLibrary.CallNtPowerInformation(15, (IntPtr)null, 0, buffer, outputBufferSize);
            uint statusSleepTime = (uint)Marshal.ReadInt32(buffer);

            var lastSleepTime = TimeSpan.FromTicks(statusSleepTime);
            Console.WriteLine(lastSleepTime);

            PowrProfLibrary.CallNtPowerInformation(14, (IntPtr)null, 0, buffer, outputBufferSize);
            uint statusWakeTime = (uint)Marshal.ReadInt32(buffer);




            var lastWake = TimeSpan.FromTicks(statusWakeTime);
            Console.WriteLine(lastWake);


            //IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE)));
            //outputBufferSize = (UInt32)Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE));
            //PowrProfLibrary.CallNtPowerInformation(5, (IntPtr)null, 0, status, outputBufferSize);


            //SYSTEM_BATTERY_STATE battStatus =
            //    (SYSTEM_BATTERY_STATE)Marshal.PtrToStructure(status, typeof(SYSTEM_BATTERY_STATE));

            //Console.WriteLine(battStatus);

            //status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION)));
            //outputBufferSize = (UInt32)Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION));
            //PowrProfLibrary.CallNtPowerInformation(12, (IntPtr)null, 0, status, outputBufferSize);

            //SYSTEM_POWER_INFORMATION powerInformation =
            //     (SYSTEM_POWER_INFORMATION)Marshal.PtrToStructure(status, typeof(SYSTEM_POWER_INFORMATION));
            //int size = Marshal.SizeOf(typeof(Int32));
            //IntPtr pBool = Marshal.AllocHGlobal(size);
            //Marshal.WriteInt32(pBool, 0, 1);  // last parameter 0 (FALSE), 1 (TRUE)
            //PowrProfLibrary.CallNtPowerInformation(10, pBool, (UInt32)(Marshal.SizeOf(typeof(UInt32))), (IntPtr)null, 0);
            ////, @q, sizeof(BOOLEAN), 0, 0



            //Console.WriteLine(powerInformation);
            //Marshal.FreeCoTaskMem(buffer);

            // Sleeps the machine
            //PowrProfLibrary.SetSuspendState(0, 0,0);
        }


    }
}
