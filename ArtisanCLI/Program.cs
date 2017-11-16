using ConsoleHints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArtisanCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHints.ConsoleHintedInput input = new ConsoleHints.ConsoleHintedInput(
                loadCommands()
            );
            string line = "", command, parameters;
            while (line != "quit")
            {
                ConsoleUtils.WritePrompt();
                line = input.ReadHintedLine();
                if (line != "quit")
                {
                    command = line.Split(' ')[0];
                    parameters = string.Join(" ", line.Split(' ').Skip(1));
                    if (command != "quit")
                    {
                        Console.WriteLine();
                        Console.WriteLine(runArtisanCmd(command, parameters));
                    }
                }
            }
        }

        //Run Artisan commands
        static string runArtisanCmd(string cmd, string args="")
        {
            return runCommand("php", "artisan " + cmd + (" " + args).Trim().Insert(0, " "));
        }

        //Load Laravel's commands
        static string[] loadCommands()
        {
            List<string> cmds = new List<string>();
            cmds.Add("quit");
            string all = runArtisanCmd("list");
            Match match = Regex.Match(all, "Available commands:");
            if (match.Success)
            {
                string list = all.Substring(match.Index + match.Length + 1);
                string[] lines = list.Split('\n');
                string[] lineParts;
                foreach(string line in lines)
                {
                    lineParts = line.Trim().Split(' ');
                    if (lineParts.Length > 1)
                    {
                        cmds.Add(clean(Regex.Replace(lineParts[0],"\\s","")));
                    }
                }
            }
            return cmds.ToArray();
        }

        //Run command
        static string runCommand(string file, string args=null)
        {
            string result = string.Empty;
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = file,
                    Arguments = args,
                    CreateNoWindow = true
                }
            };
            try
            {
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    result += clean(proc.StandardOutput.ReadLine(), false)
                         + "\n";
                }
            }
            catch {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Comando Desconocido.");
                Console.ResetColor();
            }
            
            return result;
        }

        //Remove strange characters
        static string clean(string text, bool justEmpty = true)
        {
            var replacement = justEmpty ? string.Empty : " ";
            return text.Replace("[32m", replacement).Replace("[39m", replacement).Replace("[33m", replacement)
                        .Replace("[37;41m", replacement).Replace("[39;49m", replacement);
        }
    }
}
