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

        public static string[] GetFiles(string directory) {
            return Directory.GetFiles(directory+"/");
        }

        public static void cacheVisualBoard(byte[,] board, string name)
        {
            if(File.Exists("cache/" + name + ".core")) File.Delete("cache/" + name + ".core");
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

        public List<string> Snaps = new List<string>();
        public void createSnap()
        {
            if (File.Exists("saves/snaps" + name + ".core")) File.Delete("saves/snaps" + name + ".core");
        }

        public void partSnap(byte[,] board,string name)
        {
            if (File.Exists("saves/snaps" + name + ".core")) File.Delete("saves/snaps" + name + ".core");
            StreamWriter cache_file = new StreamWriter("saves/snaps" + name + ".core",true);
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

        public byte[,] getSnap(string name)
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

        public static void absoluteListener(string name)
        {

        }
    }
}