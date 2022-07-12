using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace gisreporter_updater
{
    class Program
    {
        static string pathToUpdateFiles;
        static string pathToCurrentFiles;
        static string commandToExecuteWhenDone;

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("USAGE: gisreporter_updater [pathToUpdateFiles] [pathToCurrentFiles] [commandToExecuteWhenDone]" + Environment.NewLine);

                Console.Write("Path to the new files: ");
                pathToUpdateFiles = Console.ReadLine();

                Console.Write("Path to the current files: ");
                pathToCurrentFiles = Console.ReadLine();

                Console.Write("Command to execute when done: ");
                commandToExecuteWhenDone = Console.ReadLine();

                Console.Write("Arguments for command (leave blank if none): ");
                commandToExecuteWhenDone = Console.ReadLine();
            }
            else
            {
                pathToUpdateFiles = args[0].ToString();
                pathToCurrentFiles = args[1].ToString();
                commandToExecuteWhenDone = args[3];
            }

            Console.WriteLine(Environment.NewLine + Environment.NewLine);
            Console.WriteLine("Waiting for previous program to stop...");

            // We need to wait for the calling program to close before we can overwrite the files
            Task.Delay(5000);

            string[] files = Directory.GetFiles(pathToUpdateFiles, "*.*");
            for (int i = 0; i < files.Length; i++)
            {
                string filename = Path.GetFileName(files[i]);
                File.Copy(files[i], pathToCurrentFiles + Path.DirectorySeparatorChar + filename, true);
                Console.WriteLine(filename);
            }

            if (commandToExecuteWhenDone != string.Empty)
            {
                string[] cmdArr = commandToExecuteWhenDone.Split(' ', 2);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = true;
                startInfo.FileName = cmdArr[0];
                startInfo.Arguments = cmdArr[1];
                startInfo.WindowStyle = ProcessWindowStyle.Normal;

                Process.Start(startInfo);

                Console.WriteLine("Done.");
                Console.WriteLine("**************************************");
            }
        }
    }
}

