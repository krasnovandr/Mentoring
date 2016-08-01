using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Task1;

namespace Task2
{
    [ComVisible(true)]
    [Guid("db57d41a-4782-4eb6-9e76-c8ec634d60bf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IPowerManagmentServer
    {
        void EnterSleepMode();
        void EnterHybernateMode();
        TimeSpan GetLastSleepTime();
        TimeSpan GetLastWakeTime();
        SYSTEM_BATTERY_STATE GetBatteryState();
        SYSTEM_POWER_INFORMATION GetPowerinformation();
    }
}
