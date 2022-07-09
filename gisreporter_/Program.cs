﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using gisserver.map.gisreporter;
using System.Reflection;
using System.Diagnostics;
using System.IO.Compression;

namespace gisreporter_
{
    class Program
    {
        private static string gisReceiverUrl = gisserver.PrivateVariables.FunctionAppUrl;
        private static List<MapItem> mapItems = new List<MapItem>();
        private static bool continueRunning = true;
        private static DateTime lastMessageCheck;
        private static int numberOfTimerCycles;
        private static bool updateIsAvailable = false;
        public static double Version;

        static void Main(string[] args)
        {
            Version = 1.26;


            // Initialize global variables
            lastMessageCheck = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
            numberOfTimerCycles = 1;

            Console.Write(@"
/-------------------------------------------------------------------------\
|                               GIS Reporter                              |
|                       by Joshua Carroll, AA5JC                          |
\_________________________________________________________________________/

Version " + Version + @"

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
                    int i = 60; // Seconds of delay between message checks.
                    do
                    {
                        await Task.Delay(1000);
                        Console.Write(string.Format("\rChecking for new data in {0} seconds...  ", i--));
                    }
                    while (i > 0);
                    Console.Write("\r                                                                 ");
                    
                    CheckForNewData(args);

                    numberOfTimerCycles = numberOfTimerCycles + 1;
                    if (numberOfTimerCycles % 40 == 0)
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

        private static void CheckWinlinkMessages(string path)
        {
            if (path != string.Empty)
            {
                // Validate file path
                if (Directory.Exists(path))
                {
                    string[] filesMime = Directory.GetFiles(path, "*.mime");
                    string[] filesBsf = Directory.GetFiles(path, "*.b2f");
                    string[] filesToProcess = filesMime.Concatenate<string>(filesBsf);

                    ProcessFiles(filesToProcess);

                    if (mapItems.Count > 0)
                    {
                        // Send to web service
                        string suffix = "s";
                        if (mapItems.Count < 2) { suffix = ""; }

                        Console.WriteLine(String.Format("Sending {0} new message{1} to the server...", mapItems.Count, suffix));

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
                            Console.WriteLine("  Server response: " + res.StatusDescription);

                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                mapItems.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("  ** SERVER ERROR: " + ex.Message);
                        }
                        Console.WriteLine("");
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
        private static void ProcessFiles(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (File.GetLastWriteTime(files[i]) > lastMessageCheck)
                {
                    WinlinkMessage msg = new WinlinkMessage(File.ReadAllText(files[i]), files[i]);

                    if (msg.MessageXML != null && msg.MessageXML.Trim() != string.Empty)
                    {
                        if (mapItems.Count == 0)
                        {
                            Console.Write(Environment.NewLine);
                        }
                        Console.WriteLine(String.Format(" - {0} received from {1} at {2}", msg.MessageSubject, msg.SendersCallsign, msg.DateTimeSent));
                        MapItem mapItem = msg.ToMapItem();
                        mapItems.Add(mapItem);
                    }
                }
            }

            lastMessageCheck = DateTime.Now;
        }

        private static void CheckForUpdates(bool promptToInstall, string[] programArgs)
        {
            if (updateIsAvailable)
            {
                updateAvailablePrompt();
            }
            else
            {
                Console.WriteLine("Checking for updates...");
                // Get the json releases from Github
                Github.Release latestRelease;
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "GIS Communicator (GIS receiver)");
                    string json = wc.DownloadString("https://api.github.com/repos/JoshuaCarroll/giscommunicator/releases/latest");
                    latestRelease = JsonSerializer.Deserialize<Github.Release>(json);
                }

                string tag_name = latestRelease.tag_name;
                if (tag_name.StartsWith("v", StringComparison.InvariantCultureIgnoreCase))
                {
                    tag_name = tag_name.Substring(1);
                }

                double latestVersion = double.Parse(tag_name);
                if (latestVersion > Version)
                {
                    // Do the update
                    string currentDirectory = Directory.GetCurrentDirectory();

                    updateIsAvailable = true;
                    if (!promptToInstall)
                    {
                        updateAvailablePrompt();
                    }
                    else
                    {
                        // Download the updated files
                        Console.Write(@"


*************************************************************

An updated version is available. Is is strongly recommended to update now.

Your version: " + Version + @"
New version: " + latestVersion + @"

Proceed with the update? (Y/N)
                
*************************************************************

?: ");

                        string input = Console.ReadLine();
                        if (input.Trim().ToUpper() == "Y")
                        {
                            // Find the zip file
                            int zipfileAssetIndex = -1;
                            for (int i = 0; i < latestRelease.assets.Count; i++)
                            {
                                if (latestRelease.assets[i].name.ToLower().EndsWith(".zip"))
                                {
                                    zipfileAssetIndex = i;
                                    break;
                                }
                            }

                            string tgtDir = currentDirectory + Path.DirectorySeparatorChar + "release";
                            Directory.CreateDirectory(tgtDir);

                            string[] oldUpdateFiles = Directory.GetFiles(tgtDir);
                            for (int i = 0; i < oldUpdateFiles.Length; i++)
                            {
                                File.Delete(oldUpdateFiles[i]);
                            }

                            // Download the new package
                            using (var wc = new WebClient())
                            {
                                wc.DownloadFile(latestRelease.assets[zipfileAssetIndex].browser_download_url, tgtDir + Path.DirectorySeparatorChar + latestRelease.assets[zipfileAssetIndex].name);
                            }

                            // unzip file +++++
                            ZipFile.ExtractToDirectory(tgtDir + Path.DirectorySeparatorChar + latestRelease.assets[zipfileAssetIndex].name, tgtDir, true);

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
                    Console.WriteLine("  No updates available.");
                }
            }
        }

        private static void updateAvailablePrompt()
        {
            Console.Write(Environment.NewLine + Environment.NewLine + "** An updated version is available. Restart the program to update the GIS Reporter. **" + Environment.NewLine + Environment.NewLine);
        }
    }
}

