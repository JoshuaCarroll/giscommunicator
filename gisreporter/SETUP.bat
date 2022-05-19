
echo off
cls
echo.
echo.
set /P callsign=What is the callsign that will receive messages? 
echo echo off > RUN.BAT
echo cls >> RUN.BAT
echo dotnet gisreporter.dll /w "C:\RMS Express\%callsign%\Messages" >> RUN.BAT
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
echo ^<?xml version="1.0" encoding="UTF-8"?^> > REPORTS.KML
echo ^<kml xmlns="http://www.opengis.net/kml/2.2" xmlns:gx="http://www.google.com/kml/ext/2.2" xmlns:kml="http://www.opengis.net/kml/2.2" xmlns:atom="http://www.w3.org/2005/Atom"^> >> REPORTS.KML
echo ^<NetworkLink^> >> REPORTS.KML
echo 	^<name^>%callsign% Winlink messages^</name^> >> REPORTS.KML
echo 	^<description^>Digital messages and forms received over amateur radio^</description^> >> REPORTS.KML
echo 	^<Link^> >> REPORTS.KML
echo 		^<href^>https://giscommunicator.azurewebsites.net/api/gisreceiver?code=xNjuM3fBnIsZ28iTEiUNbFQcmBEdPkp5aspqxVm1aaJ6cGItH4dcyQ==^&amp;recipient=%callsign%^</href^> >> REPORTS.KML
echo 		^<refreshMode^>onInterval^</refreshMode^> >> REPORTS.KML
echo 		^<refreshInterval^>180^</refreshInterval^> >> REPORTS.KML
echo 	^</Link^> >> REPORTS.KML
echo ^</NetworkLink^> >> REPORTS.KML
echo ^</kml^> >> REPORTS.KML
echo.
echo You can now run the program by executing RUN.BAT.
echo.
echo To view the reports, open REPORTS.KML in Google Earth. 
echo.
pause