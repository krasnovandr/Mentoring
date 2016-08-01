
set powerManagmentApi = CreateObject("Task2.PowerManagment")
sleepTime =  powerManagmentApi.GetLastSleepTime()
Wscript.Echo "Last Sleep Time Seconds" 
Wscript.Echo CDbl(sleepTime), vbNewLine

wakeTime =  powerManagmentApi.GetLastWakeTime()
Wscript.Echo "Last wake Time Seconds" 
Wscript.Echo CDbl(wakeTime), vbNewLine


powerManagmentApi.ReserveHybernationFile()
Wscript.Echo "hybernation file reserved" , vbNewLine

powerManagmentApi.DeleteHybernationFile()
Wscript.Echo "hybernation file deleted" , vbNewLine


batteryMaxCapacity = powerManagmentApi.GetBatteryMaxCapacity()
Wscript.Echo "Battery Max capacity" 
Wscript.Echo CDbl(batteryMaxCapacity), vbNewLine

batteryRemainingCapacity = powerManagmentApi.GetBatteryRemainingCapacity()
Wscript.Echo "Battery Remaining capacit" 
Wscript.Echo CDbl(batteryRemainingCapacity), vbNewLine

systemCoolingMode = powerManagmentApi.GetSystemCoolingMode()
Wscript.Echo "System cooling mode " 
Wscript.Echo CDbl(systemCoolingMode), vbNewLine


Wscript.Echo "Press enter :" ,vbNewLine
Wscript.Echo " 1 - to EnterSleepMode"  ,vbNewLine
Wscript.Echo " 2 - to EnterHybernateMode" ,vbNewLine

Dim mode
mode = ""
mode = mode & WScript.StdIn.Read(1)

if mode = 1 then
    powerManagmentApi.EnterSleepMode()
end if 

if mode = 2 then
    powerManagmentApi.EnterHybernateMode()
end if 


