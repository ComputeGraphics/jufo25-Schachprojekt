using System.Diagnostics;
namespace ChessCORE
{

    internal class Bash
    {
        public static void sendCommand(string command)
        {
            ProcessStartInfo startInfo = new() { FileName = "/bin/bash",Arguments = command,};
            Process proc = new() { StartInfo = startInfo,};
            proc.Start();
        }
    }
}