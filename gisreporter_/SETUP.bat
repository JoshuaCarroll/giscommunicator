REM ** Editing this file?? Be sure to change the line number references in Program.cs if needed
echo off
cls
echo.
echo.
set /P callsign=What is the callsign that will receive messages? 
echo echo off > RUN.BAT
echo cls >> RUN.BAT
echo dotnet gisreporter_.dll "C:\RMS Express\%callsign%\Messages" >> RUN.BAT
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
echo For instructions on how you can view the reports, visit https://aa5jc.com/map
echo.
pause