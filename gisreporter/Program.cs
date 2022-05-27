using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Text.Json;
using System.Text;

namespace gisreporter
{
    class Program
    {
        private static string gisReceiverUrl = "https://giscommunicator.azurewebsites.net/api/gisreceiver?code=xNjuM3fBnIsZ28iTEiUNbFQcmBEdPkp5aspqxVm1aaJ6cGItH4dcyQ==";

        private static List<MapItem> mapItems = new List<MapItem>();
        private static Timer timer = new Timer();
        private static bool continueRunning = true;
        private static DateTime lastMessageReceived;

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

(To view the data, visit https://aa5jc.com/map)

To run you need to specify at least one folder as described below:                                           

Winlink
  - This will report forms that contain location data that are received
    on your computer.
  - Usage: /W [file path to your messages folder]
  - Example: /W ''C:\RMS Express\AA5JC\Messages''
  - NOTE: This can be added multiple times in case you have multiple callsigns, or want to monitor
    a tactical address controlled from Paclink.

");
            

            CheckForNewData();

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
        private static void CheckWinlinkKml()
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
                    timer.Stop();
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
                    // Get all files 
                    string[] files = Directory.GetFiles(path, "*.mime");

                    // Check each of these files to see if they are plotable and new
                    DateTime newestNewMessageReceived = lastMessageReceived;

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
                    Console.WriteLine("** The Winlink folder you specified does not exist.");
                    timer.Stop();
                }
            }
        }
    }
}
