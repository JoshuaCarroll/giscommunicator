[![forthebadge](https://forthebadge.com/images/badges/powered-by-electricity.svg)](https://forthebadge.com)
[![image](https://user-images.githubusercontent.com/2617394/173255052-2d3576b1-889c-42af-98c7-786509a4f5a1.png)](https://forthebadge.com)
[![image](https://user-images.githubusercontent.com/2617394/173255113-80294590-a303-4e39-b765-3ebdfe51fbbc.png)](https://forthebadge.com)

Note: If you are just trying to view data that has been reported, you do not need anything from this page.  Just browse to [https://aa5jc.com](https://aa5jc.com) and click "SITREP Map" in the menu.

# GIS Communicator

The GIS Communicator is an end-to-end solution that will faciliate the creation of a situational awareness map, enabling state and agency leaders to have immediate access to actionable information at a single glance.  The solution is comprised of three applications.  The map data is best viewed in Google Earth.

![image](https://user-images.githubusercontent.com/2617394/173254527-e0188f9a-5614-4291-bc05-954f69bc7209.png)

## GIS Reporter

GIS Reporter is the client program that will monitor for new Winlink or Paclink messages that contain forms.  Messages that do not contain mappable form data (don't have latitude and longitude) are ignored.  When new messages are received, the form data is sent to the GIS Receiver web service.  This is a console application that was written in C# on the Dotnet Core framework - meaning it can run on Windows, Mac, or Linux.

## GIS Receiver
GIS Receiver is a web service made to receive and store the form data sent from GIS Reporter.  It will parse the data then store it in a database.  
(Note: The database must be an Azure SQL Server since it relies on the `coordinate` datatype.)

## GIS Server
GIS Server is a .Net web application that will faciliate creating the linked, automatically refreshing KML files.  It serves the KML files and their accompanying images.
