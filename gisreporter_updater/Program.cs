using System;
using System.Diagnostics;
using System.IO;

namespace gisreporter_updater
{
    class Program
    {
        static string pathToUpdateFiles;
        static string pathToCurrentFiles;
        static int processIdOfProgram;
        static string commandToExecuteWhenDone;

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("USAGE: gisreporter_updater [pathToUpdateFiles] [pathToCurrentFiles] [processNameOfProgramToUpdate] [commandToExecuteWhenDone]" + Environment.NewLine);

                Console.Write("Path to the new files: ");
                pathToUpdateFiles = Console.ReadLine();

                Console.Write("Path to the current files: ");
                pathToCurrentFiles = Console.ReadLine();

                Console.Write("ID of current program process: ");
                processIdOfProgram = int.Parse(Console.ReadLine());

                Console.Write("Command to execute when done: ");
                commandToExecuteWhenDone = Console.ReadLine();

                Console.Write("Arguments for command (leave blank if none): ");
                commandToExecuteWhenDone = Console.ReadLine();
            }
            else
            {
                pathToUpdateFiles = args[0].ToString();
                pathToCurrentFiles = args[1].ToString();
                processIdOfProgram = int.Parse(args[2].ToString());
                commandToExecuteWhenDone = args[3];
            }

            Console.WriteLine(String.Format("Waiting for process {0} to exit...", processIdOfProgram));

            if (Process.GetProcessById(processIdOfProgram).WaitForExit(5000))
            {
                Console.WriteLine("  Process exited.");
            }
            {
                Process.GetProcessById(processIdOfProgram).Kill();
                Console.WriteLine("  Process killed after timeout.");
            }

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

