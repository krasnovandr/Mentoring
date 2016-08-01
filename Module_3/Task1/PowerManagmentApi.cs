using System;
using System.Runtime.InteropServices;

namespace Task1
{
    public static class PowerManagmentApi
    {
        public static void EnterSleepMode()
        {
            PowrProfLibrary.SetSuspendState(0, 0, 0);
        }

        public static void EnterHybernateMode()
        {
            PowrProfLibrary.SetSuspendState(1, 0, 0);
        }

        public static TimeSpan GetLastSleepTime()
        {
            var statusSleepTime = GetLastSleepWakeTime((Int32)POWER_INFORMATION_LEVEL.LastSleepTime);

            return TimeSpan.FromTicks(statusSleepTime);
        }

      

        public static TimeSpan GetLastWakeTime()
        {
            var statusWakeTime = GetLastSleepWakeTime((Int32)POWER_INFORMATION_LEVEL.LastWakeTime);

            return TimeSpan.FromTicks(statusWakeTime);
        }

        public static SYSTEM_BATTERY_STATE  GetBatteryState()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE)));
           
           var  outputBufferSize = (UInt32)Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE));
            PowrProfLibrary.CallNtPowerInformation(5, (IntPtr)null, 0, status, outputBufferSize);


            SYSTEM_BATTERY_STATE battStatus =
                (SYSTEM_BATTERY_STATE)Marshal.PtrToStructure(status, typeof(SYSTEM_BATTERY_STATE));

            Marshal.FreeCoTaskMem(status);
            return battStatus;
        }

        public static SYSTEM_POWER_INFORMATION GetPowerinformation()
        {
            var status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION)));
           
            var powerInformation =
                 (SYSTEM_POWER_INFORMATION)Marshal.PtrToStructure(status, typeof(SYSTEM_POWER_INFORMATION));
           
    
            Marshal.FreeCoTaskMem(status);

            return powerInformation;
        }


        private static int GetLastSleepWakeTime(int informationLevel)
        {
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(UInt32)));

            var outputBufferSize = (UInt32)Marshal.SizeOf(typeof(UInt32));
            PowrProfLibrary.CallNtPowerInformation(
                informationLevel,
                (IntPtr)null,
                0,
                buffer,
                outputBufferSize);

            var statusSleepTime = Marshal.ReadInt32(buffer);
            Marshal.FreeCoTaskMem(buffer);

            return statusSleepTime;
        }
    }
}
