using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LukeVo.RunOnSave.GlobalTool
{

    public class Program
    {
        public static void Main(string[] args)
        {
            // Validate input
            if (!ValidateInput(args))
            {
                PrintSyntax();
                return;
            }

            // Parse commands
            var path = args[0];

            IEnumerable<ProcessStartInfo> commands;
            try
            {
                commands = ParseCommands(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Setup Watcher
            var folderPath = path;
            var isFile = File.Exists(path);
            if (isFile)
            {
                folderPath = Path.GetDirectoryName(folderPath);
            }
            else
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine("File or folder not exist.");
                    return;
                }
            }

            using var monitor = new FileSystemWatcher()
            {
                IncludeSubdirectories = !isFile,
                Filter = isFile ? Path.GetFileName(path) : "*.*",
                Path = folderPath,
            };

            Action<object, FileSystemEventArgs> handler1 = (_, e) =>
            {
                monitor.EnableRaisingEvents = false;

                Console.WriteLine($"{e.Name} {e.ChangeType}.");
                OnFileChanged(commands);

                monitor.EnableRaisingEvents = true;
            };

            monitor.Changed += handler1.Invoke;
            monitor.Created += handler1.Invoke;
            monitor.Deleted += handler1.Invoke;
            monitor.Renamed += handler1.Invoke;

            monitor.EnableRaisingEvents = true;

            Console.WriteLine("Monitor Started. Enter Q to stop.");

            while (Console.Read() != 'q') ;
        }

        static bool ValidateInput(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Path not found");
                return false;
            }

            if (args.Length < 2)
            {
                Console.WriteLine("At least one command must be provided");
                return false;
            }

            return true;
        }

        static IEnumerable<ProcessStartInfo> ParseCommands(string[] args)
        {
            var argsPrepend = "";
            var shellName = "/bin/bash";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shellName = "cmd";
                argsPrepend = "/c ";
            }

            return args
                .Skip(1)
                .Select(q => new ProcessStartInfo()
                {
                    FileName = shellName,
                    Arguments = argsPrepend + q,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                }).ToList();
        }

        static void OnFileChanged(IEnumerable<ProcessStartInfo> commands)
        {
            foreach (var command in commands)
            {
                RunCommand(command);
            }

            Console.WriteLine("----------------");
        }

        static void RunCommand(ProcessStartInfo processInfo)
        {
            Console.WriteLine($"RunOnSave > {processInfo.Arguments.Substring(processInfo.FileName == "cmd" ? 3 : 0)}");

            var process = new Process()
            {
                StartInfo = processInfo,
            };
            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                Console.WriteLine(process.StandardOutput.ReadLine());
            }

            while (!process.StandardError.EndOfStream)
            {
                Console.WriteLine(process.StandardError.ReadLine());
            }

            process.WaitForExit();
        }

        static void PrintSyntax()
        {
            Console.WriteLine("runonsave <path> <command1> [<command2> [<command...>]]");
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("Monitor the <path> (can be file or folder) and execute the shell commands when there is changes");
        }
    }

}
