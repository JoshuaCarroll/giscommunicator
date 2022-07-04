echo off
cls
echo.
echo.
echo "Enter the full path to the folder(s) that contain your Winlink messages."
echo "Multiple folders can be separated by a space. Be sure to wrap your paths"
echo "in quotes."
echo.
echo "Default Winlink messages path:      C:\RMS Express\[[callsign]]\Messages"
echo "Default Pat messages path (Linux)   /home/[[user]]/.local/share/pat/mailbox/[[callsign]]/in"
echo.
set /P folders=::  
echo echo off > RUN.BAT
echo cls >> RUN.BAT
echo dotnet gisreporter_.dll %folders% >> RUN.BAT
echo echo. >> RUN.BAT
echo echo  ************************************************************ >> RUN.BAT
echo echo  *                                                          * >> RUN.BAT
echo echo  *   If the program did not run, you need to install the    * >> RUN.BAT
echo echo  *     .Net Core 3.1 runtime.  You can download it at:      * >> RUN.BAT
echo echo  *                                                          * >> RUN.BAT
echo echo  *            https://aka.ms/dotnet-download                * >> RUN.BAT
echo echo  *                                                          * >> RUN.BAT
echo echo  ************************************************************ >> RUN.BAT
echo pause >> RUN.BAT
echo You can now run the program by executing RUN.BAT.
echo.
echo "For instructions on how you can view the reports, visit https://aa5jc.com/map"
echo.
pause