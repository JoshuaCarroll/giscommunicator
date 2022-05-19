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

        private static string winlinkPath = "";
        private static string netloggerCheckinsURL = "";
        private static List<MapItem> mapItems = new List<MapItem>();
        private static Timer timer = new Timer();
        private static bool continueRunning = true;
        private static DateTime lastMessageReceived;

        static void Main(string[] args)
        {
            lastMessageReceived = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));

            if (args.Length == 0)
            {
                Console.Write(@"
/-------------------------------------------------------------------------\
|                               GIS Reporter                              |
|                       by Joshua Carroll, AA5JC                          |
\_________________________________________________________________________/                                                                         

To run you need to specify at least one data source through the command line switches detailed below:                                           

Winlink
  - This will report forms that contain location data that are received
    on your computer.
  - Usage: /W [file path to your messages folder]
  - Example: /W ''C:\RMS Express\AA5JC\Messages''

Netlogger
  - This will report the list of check-ins into your net as recorded in
    Netlogger.
  - Usage: /N [URL]
  - Example: /N https://www.netlogger.org/api/GetPastNetCheckins.php?ServerName=server1&NetName=net1&NetID=1111
  - NOTE: Please see the NetLogger API Specification for details. It can be found at:
    http://www.netlogger.org/api/The%20NetLogger%20XML%20Data%20Service%20Interface%20Specification.pdf

");
            }

            // Read command line switches into variables
            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i].ToUpper())
                {
                    case "/W":
                        winlinkPath = args[i + 1];
                        break;
                    case "/N":
                        netloggerCheckinsURL = args[i + 1];
                        break;
                    default:
                        // do other stuff...
                        break;
                }
            }

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

                    CheckForNewData();
                } while (continueRunning);
            });
            t.Wait();
        }

        private static void CheckForNewData()
        {
            CheckWinlinkMessages();
        }

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

        private static void CheckWinlinkMessages()
        {
            if (winlinkPath != string.Empty)
            {
                // Validate file path
                if (Directory.Exists(winlinkPath))
                {
                    // Get all files 
                    string[] files = Directory.GetFiles(winlinkPath, "*.mime");

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
