using System;
using System.Runtime.InteropServices;
using Task1;

namespace Task2
{
    [ComVisible(true)]
    [Guid("35435094-864b-4cc3-a25a-48612e0df293")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class PowerManagment:IPowerManagmentServer
    {
        public void EnterSleepMode()
        {
            PowerManagmentApi.EnterSleepMode();
        }

        public void EnterHybernateMode()
        {
            PowerManagmentApi.EnterHybernateMode();
        }

        public TimeSpan GetLastSleepTime()
        {
            return PowerManagmentApi.GetLastSleepTime();
        }

        public TimeSpan GetLastWakeTime()
        {
            return PowerManagmentApi.GetLastWakeTime();
        }

        public SYSTEM_BATTERY_STATE GetBatteryState()
        {
            return PowerManagmentApi.GetBatteryState();
        }

        public SYSTEM_POWER_INFORMATION GetPowerinformation()
        {
            return PowerManagmentApi.GetPowerinformation();
        }
    }
}
