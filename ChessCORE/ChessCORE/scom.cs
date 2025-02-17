using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Runtime.CompilerServices;

//THIS DOCUMENT IS DEPRECATED AND WILL BE REMOVED IN THE NEXT RELEASE

namespace ChessCORE
{
#pragma warning disable CS8981 // Der Typname enthält nur ASCII-Zeichen in Kleinbuchstaben. Solche Namen können möglicherweise für die Sprache reserviert werden.
    internal class scom
#pragma warning restore CS8981 // Der Typname enthält nur ASCII-Zeichen in Kleinbuchstaben. Solche Namen können möglicherweise für die Sprache reserviert werden.
    {
        static SerialPort? com;
        public static bool auto_ident = true;
        public static bool allow_win32 = true;
        public static bool advanced = false;

        public static int default_baud = 0;
        public static void start()
        {
            Console.Clear();
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new(Read);

            com = new SerialPort();
            // Allow the user to set the appropriate properties.
            com.PortName = SetPortName(com.PortName);

            if (default_baud != 0)
            {
                com.BaudRate = default_baud;
            }

            if (advanced)
            {
                com.BaudRate = SetPortBaudRate(com.BaudRate);
                com.Parity = SetPortParity(com.Parity);
                com.DataBits = SetPortDataBits(com.DataBits);
                com.StopBits = SetPortStopBits(com.StopBits);
                com.Handshake = SetPortHandshake(com.Handshake);
            }


            // Set the read/write timeouts
            //com.ReadTimeout = 500;
            //com.WriteTimeout = 500;

            try
            {
                com.Open();

                readThread.Start();

                //Console.Write("Name: ");
                //name = Console.ReadLine();

                Console.WriteLine("Press ESC to finish Connection");

                do
                {
                    message = Console.ReadLine() ?? "";

                    com.WriteLine(
                        String.Format(message));
                }
                while (ConsoleKey.Escape != Console.ReadKey().Key);




                readThread.Join();
                com.Close();
                Console.Clear();
                Init.MainMenu();
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Ein Fehler ist aufgetreten: " + ex.Message.ToString());
                Console.ResetColor();
                Init.MainMenu();
                return;

            }
        }

        public static void ReadEventHandler(object sender,SerialDataReceivedEventArgs e)
        {
            try
            {
                string? message = com.ReadLine();
                Console.WriteLine(message);
            }
            catch (TimeoutException) { }
        }

        public static void Read()
        {
            com.DataReceived += new SerialDataReceivedEventHandler(ReadEventHandler);
        }

        // Display Port values and prompt user to enter a port.
        public static string SetPortName(string defaultPortName)
        {
            if (OperatingSystem.IsWindows() && allow_win32)
            {
                Console.WriteLine("Retrieving Data...");
                using var searcher = new ManagementObjectSearcher
                    ("SELECT * FROM WIN32_SerialPort");
                string[] portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                var tList = (from n in portnames
                             join p in ports on n equals p["DeviceID"].ToString()
                             select n + " - " + p["Caption"]).ToList();
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen

                if (auto_ident)
                {
                    foreach (ManagementBaseObject p in ports)
                    {
                        string desc = p["Name"].ToString() ?? "";
                        if (desc.Contains("Arduino"))
                        {
                            string[] split = desc.Split('(');
                            string com = split[1][..split[1].IndexOf(')')];
                            Console.WriteLine("Device Found on " + com);
                            return com;
                        }
                    }
                }
                tList.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("Available Ports:");
                foreach (string s in SerialPort.GetPortNames())
                {
                    Console.WriteLine("   {0}",s);
                }
            }

            string portName;
            Console.Write("Enter COM port value (Default: {0}): ",defaultPortName);
            portName = Console.ReadLine() ?? "";
            //LAST UPDATE BEFORE FEATURE REMOVAL!!!
            if (portName == "" || !(portName.ToLower()).StartsWith("com") || !portName.Contains("tty"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
        // Display BaudRate values and prompt user to enter a value.
        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ",defaultPortBaudRate);
            baudRate = Console.ReadLine() ?? "";

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            try
            {
                return int.Parse(baudRate);
            }
            catch { return defaultPortBaudRate; }


        }

        // Display PortParity values and prompt user to enter a value.
        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}",s);
            }

            Console.Write("Enter Parity value (Default: {0}):",defaultPortParity.ToString(),true);
            parity = Console.ReadLine() ?? "";

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity),parity,true);
        }
        // Display DataBits values and prompt user to enter a value.
        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Enter DataBits value (Default: {0}): ",defaultPortDataBits);
            dataBits = Console.ReadLine() ?? "";

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        // Display StopBits values and prompt user to enter a value.
        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}",s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):",defaultPortStopBits.ToString());
            stopBits = Console.ReadLine() ?? "";

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits),stopBits,true);
        }
        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}",s);
            }

            Console.Write("Enter Handshake value (Default: {0}):",defaultPortHandshake.ToString());
            handshake = Console.ReadLine() ?? "";

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake),handshake,true);
        }
    }
}


