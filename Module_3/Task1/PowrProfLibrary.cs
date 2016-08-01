using System;
using System.Runtime.InteropServices;

namespace Task1
{
    public struct SYSTEM_BATTERY_STATE
    {
        public byte AcOnLine;
        public byte BatteryPresent;
        public byte Charging;
        public byte Discharging;
        public byte spare1;
        public byte spare2;
        public byte spare3;
        public byte spare4;
        public UInt32 MaxCapacity;
        public UInt32 RemainingCapacity;
        public Int32 Rate;
        public UInt32 EstimatedTime;
        public UInt32 DefaultAlert1;
        public UInt32 DefaultAlert2;
    }

    public struct SYSTEM_POWER_INFORMATION
    {
        public UInt32 MaxIdlenessAllowed;
        public UInt32 Idleness;
        public UInt32 TimeRemaining;
        public Char CoolingMode;
    }


    public enum POWER_INFORMATION_LEVEL
    {
        AdministratorPowerPolicy = 9,
        LastSleepTime = 15,
        LastWakeTime = 14,
        ProcessorInformation = 11,
        ProcessorPowerPolicyAc = 18,
        ProcessorPowerPolicyCurrent = 22,
        ProcessorPowerPolicyDc = 19,
        SystemBatteryState = 5,
        SystemExecutionState = 16,
        SystemPowerCapabilities = 4,
        SystemPowerInformation = 12,
        SystemPowerPolicyAc = 0,
        SystemPowerPolicyCurrent = 8,
        SystemPowerPolicyDc = 1,
        SystemReserveHiberFile = 10,
        VerifyProcessorPowerPolicyAc = 20,
        VerifyProcessorPowerPolicyDc = 21,
        VerifySystemPolicyAc = 2,
        VerifySystemPolicyDc = 3
    }

    public static class PowrProfLibrary
    {
        [DllImport("PowrProf.dll")]
        public static extern UInt32 CallNtPowerInformation(
            Int32 InformationLevel, 
            IntPtr lpInputBuffer,
            UInt32 nInputBufferSize,
            IntPtr lpOutputBuffer,
            UInt32 nOutputBufferSize
         );

        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(
            Int32 hibernate,
            Int32 forceCritical,
            Int32 disableWakeEvent);

      

    }
}
