using System.Runtime.InteropServices;

namespace Task2
{
    [ComVisible(true)]
    [Guid("db57d41a-4782-4eb6-9e76-c8ec634d60bf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IPowerManagmentServer
    {
        void EnterSleepMode();
        void EnterHybernateMode();
        double GetLastSleepTime();
        double GetLastWakeTime();
        uint GetBatteryMaxCapacity();
        uint GetBatteryRemainingCapacity();
        char GetSystemCoolingMode();
        void ReserveHybernationFile();
        void DeleteHybernationFile();

    }
}
