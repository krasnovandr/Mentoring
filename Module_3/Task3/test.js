powerManagmentApi = new ActiveXObject("Task2.PowerManagment");

var sleepTime = powerManagmentApi.GetLastSleepTime();
WScript.Echo("Last Sleep Time Seconds");
WScript.Echo(sleepTime);
WScript.Echo("\n");

var wakeTime = powerManagmentApi.GetLastWakeTime();
WScript.Echo("Last wake Time Seconds \n");
WScript.Echo(wakeTime);
WScript.Echo("\n");

powerManagmentApi.ReserveHybernationFile();
WScript.Echo("hybernation file reserved");
WScript.Echo("\n");

powerManagmentApi.DeleteHybernationFile();
WScript.Echo("hybernation file deleted");
WScript.Echo("\n");

var batteryMaxCapacity = powerManagmentApi.GetBatteryMaxCapacity();
WScript.Echo("Battery Max capacity");
WScript.Echo(batteryMaxCapacity),
WScript.Echo("\n");

var batteryRemainingCapacity = powerManagmentApi.GetBatteryRemainingCapacity();
WScript.Echo("Battery Remaining capacit");
WScript.Echo(batteryRemainingCapacity),
WScript.Echo("\n");

var systemCoolingMode = powerManagmentApi.GetSystemCoolingMode();
WScript.Echo("System cooling mode ");
WScript.Echo(systemCoolingMode);
WScript.Echo("\n");

WScript.Echo("Press enter :\n");
WScript.Echo(" 1 - to EnterSleepMode \n");
WScript.Echo(" 2 - to EnterHybernateMode \n");

var stdin = WScript.StdIn;
var str = stdin.ReadLine();

if (str === "1")
  powerManagmentApi.EnterSleepMode();

if (str === "2")
  powerManagmentApi.EnterHybernateMode();

