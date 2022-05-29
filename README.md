# GIS Communicator

The GIS Communicator is a solution that will faciliate the creation of a situational awareness map, enabling state and agency leaders to have immediate access to actionable information at a single glance.  The solution is comprised of three applications.  The map data is best viewed in Google Earth.

## GIS Reporter

GIS Reporter will monitor for new Winlink or Paclink messages that contain forms.  Messages that do not contain mappable form data (don't have latitude and longitude) are ignored.  When new messages are received, the form data is sent to the GIS Receiver web service.  This is a console application that was written in C# on the Dotnet Core framework - meaning it can run on Windows, Mac, or Linux.

## GIS Receiver
GIS Receiver is a web service made to receive and store the form data sent from GIS Reporter.  It will parse the data then store it in a database.  
(Note: The database must be an Azure SQL Server since it relies on the `coordinate` datatype.)

##GIS Server
GIS Server is a .Net web application that will faciliate creating the linked, automatically refreshing KML files.  It serves the KML files and their accompanying images.
