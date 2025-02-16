using System.Text;
using System.Xml.Linq;
namespace ChessCORE
{
    internal class storage
    {
        public static string current_log = "na";
        public static void start()
        {
            Console.Clear();
            Directory.CreateDirectory("/home/" + Environment.UserName + "/ChessCORE");
            Directory.SetCurrentDirectory("/home/" + Environment.UserName + "/ChessCORE");
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
            StreamWriter logger = new StreamWriter(current_log, true);
            logger.WriteLine(DateTime.Now.ToString("[HH-mm-ss] ") + Content);
            logger.Close();
        }

        public static string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory + "/");
        }

        public static void cacheVisualBoard(byte[,] board, string name)
        {
            if (File.Exists("cache/" + name + ".core")) File.Delete("cache/" + name + ".core");
            StreamWriter cache_file = new StreamWriter("cache/" + name + ".core", true);
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
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
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
                            temp[current, i] = Byte.Parse(positions[i]);
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
            StreamWriter cache = new StreamWriter(filename, true);
            OpenGame.filename = filename;
            OpenGame.openFile = cache;
            cache.WriteLine(DateTime.Now.ToString("HH:mm dd.MM.yy"));
            //cache.Close();
            return filename;
        }

        public void saveGameSnap(byte[,] board)
        {
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

        public void finishGame(string name) {
            OpenGame.openFile.Close();
            Random rn = new Random();
            if(File.Exists("saves/games/"+name+".core")) File.Copy(OpenGame.filename, "saves/games/"+name+rn.Next()+".core");
            else File.Copy(OpenGame.filename, "saves/games/"+name+".core");
        }

        public int[] snapCount(string name)
        {
            const Int32 BufferSize = 128;
            List<int> lines = new List<int>();
            OpenGame.openFile.Close();
            using (var fileStream = File.OpenRead(name))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
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

            return lines.ToArray();
        }

        public static byte[,] getGameSnap(String name, Int32 number)
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
            Console.WriteLine("Reading File: " + name);
            using (var fileStream = File.OpenRead(name))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String? line;
                byte current = 0;
                Console.WriteLine("Skipping Until " + number);
                for (var i = 0; i <= number; i++)
                {
                    streamReader.ReadLine();
                }

                while ((line = streamReader.ReadLine()) != "pause")
                {
                    if (current < 8)
                    {
                        Console.WriteLine(line);
                        // Process line
                        string[] positions = line.Split(':');
                        for (int i = 0; i < 8; i++)
                        {
                            temp[current, i] = Byte.Parse(positions[i]);
                        }
                        current++;
                    }
                }
                streamReader.Close();
            }


            return temp;
        }


        public void partSnap(byte[,] board, string name)
        {
            if (File.Exists("saves/snaps/" + name + ".core")) File.Delete("saves/snaps/" + name + ".core");
            StreamWriter cache_file = new StreamWriter("saves/snaps/" + name + ".core", true);
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

        public static void absoluteListener(string name)
        {

        }
    }

    public static class OpenGame
    {
        public static StreamWriter openFile;
        public static string filename = "";
        public static int current_snap = 0;
    }

}