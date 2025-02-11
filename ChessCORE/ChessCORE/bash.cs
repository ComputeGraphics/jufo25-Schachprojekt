using System.Diagnostics;
namespace ChessCORE {

    internal class bash 
    {
        public static void sendCommand(string command) {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = command, }; 
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
        }
    }
}