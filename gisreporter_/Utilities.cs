using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Utilties
{
    public static class OperatingSystem
    {
        public enum Platform
        {
            Windows,
            OSX,
            Linux,
            FreeBSD,
            Unknown
        }

        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsFreeBSD() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

        public static Platform GetPlatform()
        {
            if (IsWindows())
            {
                return Platform.Windows;
            }
            else if (IsLinux())
            {
                return Platform.Linux;
            }
            else if (IsMacOS())
            {
                return Platform.OSX;
            }
            else if (IsFreeBSD())
            {
                return Platform.FreeBSD;
            }
            else
            {
                return Platform.Unknown;
            }
        }
    }
    public static class Misc
    {
        public static void ChangeLine(string fileName, string newText, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

    }
    public class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        // omit the parameter to read a line without a timeout
        public static string ReadLine(string defaultValue, int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
            {
                return input;
            }
            else
            {
                //timed out
                Console.WriteLine("N");
                return defaultValue;
            }
        }
    }
}