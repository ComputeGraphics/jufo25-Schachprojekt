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
        public static bool auto_ident = true;
        public static bool advanced = false;
        public static bool ui_mode = false;

        public static bool allow_win32 = true;
        public static bool wait_ready = true;
        public static int default_baud = 0;

        public static bool esp32 = true;
        public static bool await_read = false;
        public static bool format = false; //False: string True: List<>
        public static string? StringResponse = null;
        public static List<string> ListResponse = [];
        public static byte ListCount = 0;

        public static int[] piece_min = new int[12];
        public static int[] piece_max = new int[12];

        public static class ActiveSerial
        {
            public static Thread readThread;
            public static SerialPort com = new();
        }


        public static void set_esp32(bool set, int index = -1)
        {
            if (set)
            {
                default_baud = 19200;

                Database.Physical.piece_min.CopyTo(piece_min, 0);
                Database.Physical.piece_max.CopyTo(piece_max, 0);

                Database.Physical.piece_min = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

                Database.Physical.piece_max = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                //              piece_order = [111, 115, 113, 109, 110, 101, 11, 15, 13, 09, 10, 01];
            }
            else
            {
                scom2.default_baud = 0;
                piece_min.CopyTo(Database.Physical.piece_min, 0);
                piece_max.CopyTo(Database.Physical.piece_max, 0);
            }

            esp32 = set;
            if (index != -1) Init.SettingsMenu(index);
        }


        public static void init()
        {
            Storage.log("Creating Thread");

            Thread readThread = new(Read)
            {
                IsBackground = true
            };

            ActiveSerial.com.ReadTimeout = 1000;       // Adjust as needed
            ActiveSerial.com.WriteTimeout = 1000;      // Adjust as needed
            ActiveSerial.com.NewLine = "\n";           // Match Arduino's line ending
            ActiveSerial.com.Encoding = Encoding.ASCII; // Use ASCII encoding


            ActiveSerial.com.PortName = SetPortName(ActiveSerial.com.PortName);
            //ActiveSerial.com.PortName = "/dev/ttyACM0";
            ActiveSerial.com.DtrEnable = true;

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
                Storage.log("Attempting Serial Startup on " + ActiveSerial.com.PortName);
                //Console.WriteLine("Serial Opened");
                ActiveSerial.com.Open();
                readThread.Start();
                Storage.log("Serial Communication Established");
                //if (OperatingSystem.IsLinux()) sendCommand("X");

                //readThread.Join();
                //com.Close();
                ActiveSerial.readThread = readThread;
            }
            catch (Exception ex)
            {
                Storage.log("Error Establishing Serial Connection: " + ex.Message.ToString());
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
            Storage.log("Starting UI Mode on Serial");
            string message;
            ui_mode = true;
            init();
            //Console.Clear();
            Console.WriteLine("[ESC] - Finish / [RETURN]|[RIGHT] - Activate Write Mode");
            Console.WriteLine(ActiveSerial.com.BaudRate);
            Console.WriteLine(ActiveSerial.com.IsOpen);

            while (ConsoleKey.Escape != Console.ReadKey().Key)
            {
                message = Console.ReadLine() ?? "";

                if (message == "CLS") Console.Clear();
                else ActiveSerial.com?.WriteLine(String.Format(message));
            }

            Dispose();
            Console.Clear();
            Init.MainMenu();
        }

        public static void Dispose()
        {
            Storage.log("Dispose of the Serial Interface");
            ActiveSerial.readThread?.Join();
            ActiveSerial.com?.Close();
        }

        public static List<string> multiCommand(string command, byte count)
        {
            if (ActiveSerial.com != null)
            {
                ListCount = count;
                format = true;
                ListResponse.Clear();
                await_read = true;
                ActiveSerial.com.WriteLine(command);
                //System.Diagnostics.Debug.WriteLine("Await Data...");
                //Console.WriteLine("Await Data...");
                while (await_read) { }
                if (ListResponse.Count < count) return [];
                //Console.WriteLine("Data Recieved!");
                //System.Diagnostics.Debug.WriteLine("Data Recieved!");
                return ListResponse;
            }
            else return [];
        }

        public static string sendCommand(string command)
        {
            if (ActiveSerial.com != null)
            {
                format = false;
                StringResponse = "";
                await_read = true;

                ActiveSerial.com.WriteLine(command);
                //Console.WriteLine("Await Data...");
                //System.Diagnostics.Debug.WriteLine("Await Data...");
                while (await_read) { }
                //System.Diagnostics.Debug.WriteLine("Data Recieved!");
                //Console.WriteLine("Data Recieved!");
                return StringResponse;
            }
            else return "";
        }


        public static void ReadEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Read Line");
            try
            {
                string message = ActiveSerial.com.ReadLine();

                if (ui_mode) Console.WriteLine(message);
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
                            //Console.WriteLine("RESPONSE NO FORMAT");
                            Console.WriteLine(message);
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

            }
            catch (TimeoutException) { Console.WriteLine("Timeout"); }
        }

        public static void ESP32Handler(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine("Read Line");
            try
            {
                List<string> message = ActiveSerial.com.ReadExisting().TrimStart('\n').Split("\n").ToList();
                foreach (string line in message)
                {
                    if (ui_mode) Console.WriteLine(line);

                    if (!line.StartsWith("RB:"))
                    {
                        if (await_read)
                        {
                            if (format)
                            {
                                ListResponse.Add(line);
                                if (ListResponse.Count >= ListCount)
                                {
                                    ActiveSerial.com.WriteLine("X");
                                    await_read = false;
                                }
                            }
                            else
                            {
                                //Console.WriteLine("RESPONSE NO FORMAT");
                                Console.WriteLine(message);
                                ActiveSerial.com.WriteLine("X");

                                StringResponse = line;
                                await_read = false;
                            }
                            //return;
                        }
                    }

                }



            }
            catch (TimeoutException) { Console.WriteLine("Timeout"); }
        }


        public static void Read()
        {
            ActiveSerial.com.DataReceived += esp32 ? new System.IO.Ports.SerialDataReceivedEventHandler(ESP32Handler) : new System.IO.Ports.SerialDataReceivedEventHandler(ReadEventHandler);
        }


        //INTERFACE CONFIG
        public static string SetPortName(string defaultPortName)
        {
            if (OperatingSystem.IsWindows() && allow_win32)
            {
                //Console.WriteLine("(Only Windows) Retrieving Data...");
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
                            //Console.WriteLine("Device Found on " + com);
                            Storage.log("Device Found on " + com);
                            return com;
                        }
                    }
                }
                tList.ForEach(Console.WriteLine);
            }
            else if (OperatingSystem.IsLinux())
            {
                Console.WriteLine("Available Ports:");
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 1)
                {
                    foreach (string s in ports)
                    {
                        Console.WriteLine("   {0}", s);
                    }
                }
                else
                {
                    Storage.log("Device Found on " + ports[0]);
                    Console.WriteLine($"    {ports[0]}");

                    //bash.sendCommand("sudo chmod a+rw "+ports[0]);
                    return ports[0];
                }
                //sudo chmod a+rw /dev/ttyACM0

            }

            string portName;
            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine() ?? "";
            bool container = portName.Contains("tty");
            if (portName == "" || !portName.ToLower().StartsWith("com") || !container)
            {
                portName = defaultPortName;
            }
            if (container && !portName.StartsWith("/dev/"))
            {
                portName = "/dev/" + portName;
            }
            return portName;
        }
        // Display BaudRate values and prompt user to enter a value.
        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
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
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            parity = Console.ReadLine() ?? "";

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
        // Display DataBits values and prompt user to enter a value.
        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
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
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine() ?? "";

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }
        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine() ?? "";

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
    }
}
