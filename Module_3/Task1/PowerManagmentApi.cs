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

        public static double GetLastSleepTimeTotalSeconds()
        {
            var statusSleepTime = GetLastSleepWakeTime((Int32)POWER_INFORMATION_LEVEL.LastSleepTime);

            return TimeSpan.FromTicks(statusSleepTime).TotalSeconds;
        }

        public static double GetLastWakeTimeTotalSeconds()
        {
            var statusWakeTime = GetLastSleepWakeTime((Int32)POWER_INFORMATION_LEVEL.LastWakeTime);

            return TimeSpan.FromTicks(statusWakeTime).TotalSeconds;
        }

        public static uint GetBatteryMaxCapacity()
        {
            return GetSystemBatteryState().MaxCapacity;
        }

        public static uint GetBatteryRemainingCapacity()
        {
            return GetSystemBatteryState().RemainingCapacity;
        }

        public static char GetSystemCoolingMode()
        {
            return GetSystemPowerInformation().CoolingMode;
        }

        public static void ReserveHybernationFile()
        {
            HybernationFile(1);
        }

        public static void DeleteHybernationFile()
        {
            HybernationFile(0);
        }

        private static void HybernationFile(int reservedOrDelete)
        {
            int size = Marshal.SizeOf(typeof(Int32));
            IntPtr pBool = Marshal.AllocHGlobal(size);
            Marshal.WriteInt32(pBool, 0, reservedOrDelete);
          
            PowrProfLibrary.CallNtPowerInformation(
                (Int32)POWER_INFORMATION_LEVEL.SystemReserveHiberFile, 
                pBool, 
                (UInt32)(Marshal.SizeOf(typeof(UInt32))), 
                (IntPtr)null, 
                0);

        }

    

        private static SYSTEM_BATTERY_STATE GetSystemBatteryState()
        {
            IntPtr status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (SYSTEM_BATTERY_STATE)));

            var outputBufferSize = (UInt32) Marshal.SizeOf(typeof (SYSTEM_BATTERY_STATE));
            PowrProfLibrary.CallNtPowerInformation(5, (IntPtr) null, 0, status, outputBufferSize);


            SYSTEM_BATTERY_STATE battStatus =
                (SYSTEM_BATTERY_STATE) Marshal.PtrToStructure(status, typeof (SYSTEM_BATTERY_STATE));

            Marshal.FreeCoTaskMem(status);
            return battStatus;
        }

      

        private static SYSTEM_POWER_INFORMATION GetSystemPowerInformation()
        {
            var status = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (SYSTEM_POWER_INFORMATION)));

            var powerInformation =
                (SYSTEM_POWER_INFORMATION) Marshal.PtrToStructure(status, typeof (SYSTEM_POWER_INFORMATION));


            Marshal.FreeCoTaskMem(status);

            return powerInformation;
        }


        private static uint GetLastSleepWakeTime(int informationLevel)
        {
            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ulong)));

            var outputBufferSize = (UInt32)Marshal.SizeOf(typeof(ulong));
            PowrProfLibrary.CallNtPowerInformation(
                informationLevel,
                (IntPtr)null,
                0,
                buffer,
                outputBufferSize);

            var statusSleepTime = (uint)Marshal.ReadInt32(buffer);
            Marshal.FreeCoTaskMem(buffer);

            return statusSleepTime;
        }
    }
}
