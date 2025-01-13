using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ChessCORE
{
    internal class scom2
    {
        public static byte default_count = 8;

        public static bool auto_ident = scom.auto_ident;
        public static bool advanced = scom.advanced;
        public static bool ui_mode = false;

        public static int default_baud = scom.default_baud;

        public static bool await_read = false;
        public static bool format = false; //False: string True: List<>
        public static string? StringResponse = null;
        public static List<string> ListResponse = new List<string>();
        public static byte ListCount = 0;

        public static class ActiveSerial
        {
            public static Thread? readThread;
            public static SerialPort com = new SerialPort();
        }


        public static void init()
        {
            Thread readThread = new Thread(Read);

            ActiveSerial.com.PortName = SetPortName(ActiveSerial.com.PortName);

            if (default_baud != 0)
            {
                ActiveSerial.com.BaudRate = default_baud;
            }

            if (advanced)
            {
                ActiveSerial.com.BaudRate = SetPortBaudRate(ActiveSerial.com.BaudRate);
                ActiveSerial.com.Parity = SetPortParity(ActiveSerial.com.Parity);
                ActiveSerial.com.DataBits = SetPortDataBits(ActiveSerial.com.DataBits);
                ActiveSerial.com.StopBits = SetPortStopBits(ActiveSerial.com.StopBits);
                ActiveSerial.com.Handshake = SetPortHandshake(ActiveSerial.com.Handshake);
            }

            try
            {
                ActiveSerial.com.Open();
                readThread.Start();
                ActiveSerial.com.WriteLine("X");
                //readThread.Join();
                //com.Close();
                ActiveSerial.readThread = readThread;
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Ein Fehler ist aufgetreten: " + ex.Message.ToString());
                Console.ResetColor();
                //Init.Main();
                return;
            }
        }

        public static void UI()
        {
            string message = "";
            ui_mode = true;
            init();
            Console.Clear();
            Console.WriteLine("[ESC] - Finish / [RETURN]|[RIGHT] - Activate Write Mode");

            do
            {
                message = Console.ReadLine() ?? "";

                if (message == "CLS") Console.Clear();
                else ActiveSerial.com?.WriteLine(String.Format(message));
            }
            while (ConsoleKey.Escape != Console.ReadKey().Key);

            ActiveSerial.readThread?.Join();
            ActiveSerial.com?.Close();
            Console.Clear();
            Init.Main();
        }
        public static List<string> multiCommand(string command,byte count)
        {
            if (ActiveSerial.com != null)
            {
                ListCount = count;
                format = true;
                ListResponse.Clear();
                await_read = true;
                ActiveSerial.com.WriteLine(command);
                System.Diagnostics.Debug.WriteLine("Await Data...");
                while (await_read) { }
                if (ListResponse.Count < count) return new List<string>();
                System.Diagnostics.Debug.WriteLine("Data Recieved!");
                return ListResponse;
            }
            else return new List<string>();
        }

        public static string sendCommand(string command)
        {
            if (ActiveSerial.com != null)
            {
                format = false;
                StringResponse = "";
                await_read = true;
                ActiveSerial.com.WriteLine(command);

                while (await_read) { }
                return StringResponse;
            }
            else return "";
        }


        public static void ReadEventHandler(object sender,SerialDataReceivedEventArgs e)
        {
            try
            {
                string message = ActiveSerial.com.ReadLine();

                if (!message.StartsWith("RB:"))
                {
                    if (await_read)
                    {
                        if (format)
                        {
                            ListResponse.Add(message);
                            if (ListResponse.Count >= ListCount)
                            {
                                ActiveSerial.com.WriteLine("X");
                                await_read = false;
                            }
                        }
                        else
                        {
                            ActiveSerial.com.WriteLine("X");
                            StringResponse = message;
                            await_read = false;
                        }
                        return;
                    }
                }
                else
                {
                    if (!advanced) return;
                }
                if (ui_mode) Console.WriteLine(message);
            }
            catch (TimeoutException) { Console.WriteLine("Timeout"); }
        }

        public static void Read()
        {
            ActiveSerial.com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(ReadEventHandler);
        }


        //INTERFACE CONFIG
        public static string SetPortName(string defaultPortName)
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WriteLine("(Only Windows) Retrieving Data...");
                using (var searcher = new ManagementObjectSearcher
                    ("SELECT * FROM WIN32_SerialPort"))
                {
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
                                string com = split[1].Substring(0,split[1].IndexOf(')'));
                                Console.WriteLine("Device Found on " + com);
                                return com;
                            }
                        }
                    }
                    tList.ForEach(Console.WriteLine);
                }
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

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
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
