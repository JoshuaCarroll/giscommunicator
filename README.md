[![forthebadge](https://forthebadge.com/images/badges/powered-by-electricity.svg)](https://forthebadge.com)
[![image](https://user-images.githubusercontent.com/2617394/173255052-2d3576b1-889c-42af-98c7-786509a4f5a1.png)](https://forthebadge.com)
[![image](https://user-images.githubusercontent.com/2617394/173255113-80294590-a303-4e39-b765-3ebdfe51fbbc.png)](https://forthebadge.com)

- Trying to *view* data that has been reported? You don't need anything from this page.
  1. Browse to [https://aa5jc.com](https://aa5jc.com) 
  2. Click "SITREP Map" in the menu
  3. Follow the instructions there to create a KML file
  4. Unless your preferences change, you do *not* need to create another KML file in the future; the one provided will automatically update with new data.
  
- Want to include data from your Winlink messages in our data?  You need the *GIS Reporter*.  Scroll down for more infomation and how to set it up.


# GIS Communicator

The GIS Communicator is an end-to-end solution that will faciliate the creation of a situational awareness map, enabling state and agency leaders to have immediate access to actionable information at a single glance.  The solution is comprised of three applications.  The map data can be viewed in any program that can use KML files, but is best viewed in Google Earth.

![image](https://user-images.githubusercontent.com/2617394/173254527-e0188f9a-5614-4291-bc05-954f69bc7209.png)

## GIS Reporter

GIS Reporter is the client program that will monitor for new Winlink or Paclink messages that contain forms.  Messages that do not contain mappable form data (don't have latitude and longitude) are ignored.  When new messages are received, the form data is sent to the GIS Receiver web service.  

This *is* a console application (command line) that was written in C# on the Dotnet Core framework - meaning *it can run on Windows, Mac, or Linux*.  Don't be scared off by the fact that it is a console app; it was made to be extremely user friendly.

### Installation and setup of GIS Reporter
1. Download the zip file containing the latest release at https://github.com/JoshuaCarroll/giscommunicator/releases/latest
2. Extract that zip file into any folder on your computer.
3. Verify the installation of, or install DotNet Core runtime v3.1.
  a. Open a terminal or command prompt window.
  b. Type `dotnet --list-runtimes`.
  c. If you get a list that includes "Microsoft.NETCore.App 3.1.x", skip to step 4. If you get an error or a list that doesn't include that, continue to item d.
  d. Browse to https://aka.ms/dotnet-download to download the runtime, then install it.  Linux users, you may find it easier to run the commands found at  https://github.com/dotnet/core/issues/4360#issuecomment-627792352.
4. Linux users, give the SETUP.sh script execute permission by typing: `sudo chmod +x SETUP.sh`
5. Execute the setup script.  (Linux: SETUP.sh, Windows: SETUP.bat)
6. Follow the instructions, the execute the "RUN" script to start the GIS Reporter.

## GIS Receiver
GIS Receiver is a web service made to receive and store the form data sent from GIS Reporter.  It will parse the data then store it in a database.  
(Note: The database must be an Azure SQL Server since it relies on the `coordinate` datatype.)

## GIS Server
GIS Server is a .Net web application that will faciliate creating the linked, automatically refreshing KML files.  It serves the KML files and their accompanying images.
