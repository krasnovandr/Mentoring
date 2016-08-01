using System.Runtime.InteropServices;
using Task1;

namespace Task2
{
    [ComVisible(true)]
    [Guid("35435094-864b-4cc3-a25a-48612e0df293")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class PowerManagment : IPowerManagmentServer
    {
        public void EnterSleepMode()
        {
            PowerManagmentApi.EnterSleepMode();
        }

        public void EnterHybernateMode()
        {
            PowerManagmentApi.EnterHybernateMode();
        }

        public double GetLastSleepTime()
        {
            return PowerManagmentApi.GetLastSleepTimeTotalSeconds();
        }

        public double GetLastWakeTime()
        {
            return PowerManagmentApi.GetLastWakeTimeTotalSeconds();
        }

        public uint GetBatteryMaxCapacity()
        {
            return PowerManagmentApi.GetBatteryMaxCapacity();
        }

        public uint GetBatteryRemainingCapacity()
        {
            return PowerManagmentApi.GetBatteryRemainingCapacity();
        }

        public char GetSystemCoolingMode()
        {
            return PowerManagmentApi.GetSystemCoolingMode();
        }

        public void ReserveHybernationFile()
        {
            PowerManagmentApi.ReserveHybernationFile();
        }

        public void DeleteHybernationFile()
        {
            PowerManagmentApi.DeleteHybernationFile();
        }
    }
}
