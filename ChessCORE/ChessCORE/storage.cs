using System.Text;
using System.Xml.Linq;
namespace ChessCORE
{
    internal class Storage
    {
        public static string current_log = "na";

        public static string OSHome()
        {
            if (OperatingSystem.IsLinux()) return "/home/";
            else if (OperatingSystem.IsWindows()) return "/Users/";
            else return "/";
        }
        public static void start()
        {
            String home = OSHome();

            Console.Clear();
            Directory.CreateDirectory(home + Environment.UserName + "/ChessCORE");
            Directory.SetCurrentDirectory(home + Environment.UserName + "/ChessCORE");
            Directory.CreateDirectory("saves");
            Directory.CreateDirectory("saves/snaps");
            Directory.CreateDirectory("saves/games");
            Directory.CreateDirectory("cache");
            Directory.CreateDirectory("logs");
            Directory.CreateDirectory("settings");
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine(Environment.UserName);
            current_log = "logs/" + DateTime.Now.ToString("HH-mm dd-MM-yy") + ".log";
            log("ChessCORE Logger Startup");
            return;
        }

        public static void log(string Content)
        {
            StreamWriter logger = new(current_log,true);
            logger.WriteLine(DateTime.Now.ToString("[HH-mm-ss] ") + Content);
            logger.Close();
        }

        public static string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory + "/");
        }

        public static void cacheVisualBoard(byte[,] board,string name)
        {
            if (File.Exists("cache/" + name + ".core")) File.Delete("cache/" + name + ".core");
            StreamWriter cache_file = new("cache/" + name + ".core",true);
            int i = 1;
            foreach (byte element in board)
            {
                if (i == 8)
                {
                    cache_file.Write(element + "\n");
                    i = 0;
                }
                else
                {
                    cache_file.Write(element + ":");
                }
                i++;
            }
            cache_file.Close();
        }

        public static byte[,] GetCachedBoard(string name)
        {
            byte[,] temp =
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
            };
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(name))
            using (var streamReader = new StreamReader(fileStream,Encoding.UTF8,true,BufferSize))
            {
                String? line;
                byte current = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (current < 8)
                    {
                        // Process line
                        string[] positions = line.Split(':');
                        for (int i = 0; i < 8; i++)
                        {
                            temp[current,i] = Byte.Parse(positions[i]);
                        }
                        current++;
                    }
                }
                streamReader.Close();
            }


            return temp;
        }

        public static string createGame()
        {
            string time = DateTime.Now.ToString("HHmm-ddMMyy");
            string filename = "cache/temp.core";
            if (File.Exists(filename)) File.Delete(filename);
            StreamWriter cache = new(filename,true);
            OpenGame.filename = filename;
            OpenGame.openFile = cache;
            OpenGame.open = true;
            //cache.Close();
            return filename;
        }

        public static void saveGameSnap(byte[,] board)
        {
            System.Diagnostics.Debug.WriteLine("Save Game Snap to File");
            StreamWriter cache_file = OpenGame.openFile;
            int i = 1;
            cache_file.WriteLine("pause");
            foreach (byte element in board)
            {
                if (i == 8)
                {
                    cache_file.Write(element + "\n");
                    i = 0;
                }
                else
                {
                    cache_file.Write(element + ":");
                }
                i++;
            }
        }

        public static void finishGame(string name)
        {
            OpenGame.openFile.Close();
            OpenGame.open = false;
            Random rn = new();
            if (File.Exists("saves/games/" + name + ".core")) File.Move(OpenGame.filename,"saves/games/" + name + rn.Next() + ".core");
            else File.Move(OpenGame.filename,"saves/games/" + name + ".core");
        }

        public static int[] snapCount(string name)
        {
            const Int32 BufferSize = 128;
            List<int> lines = [];
            //OpenGame.openFile.Close();
            using (var fileStream = File.OpenRead(name))
            using (var streamReader = new StreamReader(fileStream,Encoding.UTF8,true,BufferSize))
            {
                String? line;
                Int32 current = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == "pause")
                    {
                        lines.Add(current);
                    }
                    current++;
                }
                streamReader.Close();
            }
            //Console.WriteLine("Snap Count:");
            //foreach(var line in lines) Console.WriteLine(line);
            return [.. lines];
        }

        public static byte[,] getGameSnap(String name,Int32 number)
        {
            OpenGame.filename = name;
            byte[,] temp =
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
            };
            const Int32 BufferSize = 128;
            Console.WriteLine("Reading File: " + name);
            using (var fileStream = File.OpenRead(name))
            using (var streamReader = new StreamReader(fileStream,Encoding.UTF8,true,BufferSize))
            {
                byte current = 0;
                List<int[,]> snaps = [];

                Console.WriteLine("Skipping Until " + number);
                for (var i = 0; i < number + 1; i++)
                {
                    streamReader.ReadLine();
                }

                //String? all = streamReader.ReadLine();
                //Console.WriteLine(all);
                String? line;
                while ((line = streamReader.ReadLine()) != null && line != "pause")
                {
                    if (current < 8)
                    {
                        Console.WriteLine(line);
                        // Process line
                        string[] positions = line.Split(':');
                        for (int i = 0; i < 8; i++)
                        {
                            temp[current,i] = Byte.Parse(positions[i]);
                        }
                        current++;
                    }
                }
                streamReader.Close();
            }


            return temp;
        }


        public static void partSnap(byte[,] board,string name)
        {
            if (File.Exists("saves/snaps/" + name + ".core")) File.Delete("saves/snaps/" + name + ".core");
            StreamWriter cache_file = new("saves/snaps/" + name + ".core",true);
            int i = 1;
            foreach (byte element in board)
            {
                if (i == 8)
                {
                    cache_file.Write(element + "\n");
                    i = 0;
                }
                else
                {
                    cache_file.Write(element + ":");
                }
                i++;
            }
            cache_file.Close();
        }

    }

    public static class OpenGame
    {
        public static StreamWriter openFile;
        public static string filename = "";
        public static int current_snap = 0;
        public static bool open = false;
    }

}