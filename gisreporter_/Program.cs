using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using gisserver.map.gisreporter;
using System.Reflection;
using System.Diagnostics;

namespace gisreporter_
{
    class Program
    {
        private static string gisReceiverUrl = gisserver.PrivateVariables.FunctionAppUrl;
        private static List<MapItem> mapItems = new List<MapItem>();
        private static bool continueRunning = true;
        private static DateTime lastMessageReceived;
        private static int numberOfTimerCycles = 0;
        private static bool updateIsAvailable = false;

        static void Main(string[] args)
        {
            lastMessageReceived = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));

            Console.Write(@"
/-------------------------------------------------------------------------\
|                               GIS Reporter                              |
|                       by Joshua Carroll, AA5JC                          |
\_________________________________________________________________________/

This program will monitor the folders specified for Winlink mapping/GIS reports. If a new
message is received that contains mapping data, it will send the details of the report
to a web service for consolidation.

(To view the data, visit https://aa5jc.com and click the link for the SITREP MAP.)

Usage: dotnet gisreporter.dll [file path to your messages folder]
  - Example: /W ''C:\RMS Express\AA5JC\Messages''

Note: This can be added multiple times in case you have multiple callsigns, or want to monitor
    a tactical address controlled from Paclink.
  - Example: /W ''C:\RMS Express\AA5JC\Messages'' ''C:\Paclink\Accounts\DELTA_Account''

");
            
            CheckForUpdates(true, args);
            CheckForNewData(args);

            Task t = Task.Run(async () => {
                do
                {
                    Console.Write(string.Format("\r{0}                                      ", DateTime.Now.ToUniversalTime()));

                    int i = 60;
                    do
                    {
                        await Task.Delay(1000);
                        Console.Write(string.Format("\rChecking for new data in {0} seconds...  ", i--));
                    }
                    while (i > 0);
                    Console.Write("\r                                                                 ");
                    
                    CheckForNewData(args);

                    numberOfTimerCycles = numberOfTimerCycles++;
                    if (numberOfTimerCycles % 10 == 0)
                    {
                        CheckForUpdates(false, args);
                    }

                } while (continueRunning);
            });
            t.Wait();
        }

        private static void CheckForNewData(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                CheckWinlinkMessages(paths[i]);
            }
        }

        // This is not currently used.
        private static void CheckWinlinkKml(string winlinkPath)
        {
            if (winlinkPath != string.Empty)
            {
                // Validate file path
                if (Directory.Exists(winlinkPath))
                {
                    if (File.Exists(String.Format("{0}/Winlink_Messages.kml", winlinkPath)))
                    {

                    }

                    if (mapItems.Count > 0)
                    {
                        // Send to web service
                        string json = JsonSerializer.Serialize(mapItems);
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(gisReceiverUrl);
                        req.Method = "POST";
                        req.ContentType = "application/json";
                        Stream stream = req.GetRequestStream();
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        stream.Write(buffer, 0, buffer.Length);
                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                        Console.WriteLine("Server response: " + res.StatusDescription);

                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            mapItems.Clear();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("** The Winlink folder you specified does not exist.");
                    continueRunning = false;
                }
            }
        }

        private static void CheckWinlinkMessages(string path)
        {
            if (path != string.Empty)
            {
                // Validate file path
                if (Directory.Exists(path))
                {
                    DateTime newestNewMessageReceived = lastMessageReceived;

                    string[] filesMime = Directory.GetFiles(path, "*.mime");
                    newestNewMessageReceived = ProcessFiles(filesMime, newestNewMessageReceived);

                    string[] filesBsf = Directory.GetFiles(path, "*.bsf");
                    newestNewMessageReceived = ProcessFiles(filesBsf, newestNewMessageReceived);

                    lastMessageReceived = newestNewMessageReceived;

                    if (mapItems.Count > 0)
                    {
                        // Send to web service

                        string json = JsonSerializer.Serialize(mapItems);
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(gisReceiverUrl);
                        req.Method = "POST";
                        req.ContentType = "application/json";
                        Stream stream = req.GetRequestStream();
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        stream.Write(buffer, 0, buffer.Length);
                        HttpWebResponse res = new HttpWebResponse();

                        try
                        {
                            res = (HttpWebResponse)req.GetResponse();
                            Console.WriteLine("Server response: " + res.StatusDescription);

                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                mapItems.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("SERVER ERROR: " + ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("** The mail folder you specified does not exist.");
                    continueRunning = false;

                }
            }
        }

        /// <summary>
        /// Processes Winlink emails that contain attachments and parses them into an array for uploading
        /// </summary>
        /// <param name="files">String array containing full path filenames to the emails to process</param>
        /// <param name="newestNewMessageReceived">Datetime representing the date and time of the most recently processed email</param>
        /// <returns>Datetime representing the date and time of the most recently processed email</returns>
        private static DateTime ProcessFiles(string[] files, DateTime newestNewMessageReceived)
        {
            DateTime outputNewestNewMessageReceived = newestNewMessageReceived;
            for (int i = 0; i < files.Length; i++)
            {
                if (File.GetLastWriteTime(files[i]) > lastMessageReceived)
                {
                    if (newestNewMessageReceived < File.GetLastWriteTime(files[i]))
                    {
                        newestNewMessageReceived = File.GetLastWriteTime(files[i]);
                    }

                    WinlinkMessage msg = new WinlinkMessage(File.ReadAllText(files[i]), Path.GetFileNameWithoutExtension(files[i]));

                    if (msg.MessageXML != null && msg.MessageXML != string.Empty)
                    {
                        Console.WriteLine(String.Format("- {0} received from {1} at {2}", msg.MessageSubject, msg.SendersCallsign, msg.DateTimeSent));
                        MapItem mapItem = msg.ToMapItem();
                        mapItems.Add(mapItem);
                    }
                }
            }
            return outputNewestNewMessageReceived;
        }

        private static void CheckForUpdates(bool promptToInstall, string[] programArgs)
        {
            gisReporterPackage package;
            gisReporterPackage oldPackage;

            string baseUrl = "https://aa5jc.com/map/gisreporter/";
            string currentDirectory = Directory.GetCurrentDirectory();

            string oldPackageJson = currentDirectory + Path.DirectorySeparatorChar + "package.json";
            if (File.Exists(oldPackageJson))
            {
                string oldJson = File.ReadAllText(oldPackageJson);
                oldPackage = JsonSerializer.Deserialize<gisReporterPackage>(oldJson);
            }
            else
            {
                oldPackage = new gisReporterPackage();
                oldPackage.PublishDateUTC = new DateTime(1970, 1, 1);
            }

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(baseUrl + "/gisreporterversion.ashx");
                package = JsonSerializer.Deserialize<gisReporterPackage>(json);
            }

            if ((package.PublishDateUTC > oldPackage.PublishDateUTC) || updateIsAvailable)
            {
                updateIsAvailable = true;
                if (!promptToInstall)
                {
                    Console.Write(@"


                *****************************

                An updated version is available. Is is strongly recommended to update now.

                Restart the console to update the GIS Reporter.
                
                *****************************


                ?: ");
                }
                else
                {
                    // Download the updated files
                    Console.Write(@"
                *****************************

                An updated version is available. Is is strongly recommended to update now.

                Your version date: " + oldPackage.PublishDateUTC + @"
                New version date: " + package.PublishDateUTC + @"

                Proceed with the update? (Y/N)
                
                *****************************
                ?: ");

                    string input = Console.ReadLine();
                    if (input.Trim().ToUpper() == "Y")
                    {
                        string srcDir = baseUrl + Path.DirectorySeparatorChar + "release" + Path.DirectorySeparatorChar;
                        string tgtDir = currentDirectory + Path.DirectorySeparatorChar + "release";

                        Directory.CreateDirectory(tgtDir);

                        // Download the update
                        using (var wc = new WebClient())
                        {
                            for (int i = 0; i < package.Files.Length; i++)
                            {
                                if (package.Files[i] != "")
                                {
                                    Console.WriteLine(baseUrl + package.Files[i]);
                                    wc.DownloadFile(srcDir + package.Files[i], currentDirectory + Path.DirectorySeparatorChar + "release" + Path.DirectorySeparatorChar + package.Files[i]);
                                }
                            }
                        }

                        string allProgramArgs = "";
                        for (int i = 0; i < programArgs.Length; i++)
                        {
                            allProgramArgs += programArgs[i] + " ";
                        }

                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = true;
                        startInfo.FileName = "dotnet";
                        startInfo.Arguments = String.Format("\"{0}{1}gisreporter_updater.dll\" \"{2}\" \"{0}\" \"{3}\" \"dotnet {4}{1}{5}.dll {6}\"", currentDirectory, Path.DirectorySeparatorChar, tgtDir, Process.GetCurrentProcess().Id, System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.FriendlyName, allProgramArgs);
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        Process.Start(startInfo);

                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Console.WriteLine("You have the most current version.");
            }
        }
    }
}

