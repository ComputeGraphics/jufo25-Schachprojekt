using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using ToolsCORE;


namespace ChessCORE
{
    internal class board_visual
    {
        public static bool show_icons = true;

        public static int timeout = 8;
        public static int timeout_remain = 0;
        public static bool show_magnetic = false;

        public static bool low_render = false;
        //□■█▓▰
        public int loader_progress = 0;

        public static void redraw_loader(int percentage)
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("LOADING".PadLeft(Console.WindowWidth / 2 + 3) + "".PadRight(Console.WindowWidth / 2 - 3) + "\n\n");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ResetColor();
            //Console.WriteLine("Please wait while the system is loading...");
            Console.WriteLine("This may take a few minutes".PadLeft(Console.WindowWidth / 2 + 13) + "".PadRight(Console.WindowWidth / 2 + 14));
            Console.WriteLine("Please do not open other programs using COM".PadLeft(Console.WindowWidth / 2 + 21) + "".PadRight(Console.WindowWidth / 2 + 22));
            Console.WriteLine("Please do not turn off or disconnect the MCU".PadLeft(Console.WindowWidth / 2 + 21) + "".PadRight(Console.WindowWidth / 2 + 22));
            Console.WriteLine("Please do not press any buttons".PadLeft(Console.WindowWidth / 2 + 15) + "".PadRight(Console.WindowWidth / 2 + 16));
            for (int i = 0; i < Console.WindowHeight - 13; i++)
            {
                Console.WriteLine();
            }

            string loaders = "";
            double small = (double)(percentage / 100.0);
            double char_count = Math.Floor(small * Console.WindowWidth - 6);
            if (percentage == 99) Console.WriteLine("Press any key to continue");
            else Console.WriteLine("LOADING");
            for (int i = 0; i < char_count; i++)
            {
                loaders += '▰';
            }
            for (int i = 0; i < Math.Floor(Console.WindowWidth - 6 - char_count); i++)
            {
                loaders += ' ';
            }
            Console.Write((percentage > 9 ? percentage.ToString() : "0" + percentage.ToString()) + "% [" + loaders + ']');

            //Console.Write(loader.PadLeft((Console.WindowWidth / 2) + (loader.Length / 2)) + "".PadRight((Console.WindowWidth / 2) + (loader.Length / 2)));
        }
        public static void show()
        {
            redraw_loader(0);
            Storage.log("Initializing scom2");
            scom2.init();
            redraw_loader(2);

            if (Database.Physical.repeat_selftest) scom2.sendCommand("test");
            redraw_loader(5);

            if (Database.Physical.calib) Database.Physical.calibrate(Database.Physical.default_calib, scom2.default_count);
            redraw_loader(91);

            //Database.Physical.calib_pieces(4);

            if (scom2.advanced)
            {
                Console.WriteLine("Average:");
                int i = 1;
                foreach (int element in Database.Physical.av)
                {
                    if (i == 8)
                    {
                        Console.WriteLine(" " + element);
                        i = 0;
                    }
                    else
                    {
                        Console.Write(" " + element);
                    }
                    i++;
                }
                Console.WriteLine("\nMin:");
                i = 1;
                foreach (int element in Database.Physical.min)
                {
                    if (i == 8)
                    {
                        Console.WriteLine(" " + element);
                        i = 0;
                    }
                    else
                    {
                        Console.Write(" " + element);
                    }
                    i++;
                }
                Console.WriteLine("\nMax:");
                i = 1;
                foreach (int element in Database.Physical.max)
                {
                    if (i == 8)
                    {
                        Console.WriteLine(" " + element);
                        i = 0;
                    }
                    else
                    {
                        Console.Write(" " + element);
                    }
                    i++;
                }

                Console.WriteLine("\nNon-Funtional Fields:");
                foreach (int error in Database.Physical.error_fields) Console.Write(error + ",");
                Console.WriteLine(Environment.NewLine + "\nNon-Functional Multiplexers:");
                foreach (int error in Database.Physical.error_rows) Console.Write(error + ",");
                Console.WriteLine();
                Console.WriteLine("Press [ENTER] to acknowledge");
                while (ConsoleKey.Enter != Console.ReadKey().Key) { }
            }

            redraw_loader(99);
            //
            //Renderer.draw(true,true,Renderer.standard_direction, false, true);

            if(low_render) Renderer.low_draw(true, !show_icons);
            else if (show_magnetic) Renderer.draw_number(true, true, 0);
            else Renderer.draw(true, true, Renderer.standard_direction, false, !show_icons);
        }

        public static void showSnap(string name)
        {
            redraw_loader(0);
            Database.Display.field = Storage.GetCachedBoard(name);
            redraw_loader(99);
            Renderer.draw(true, true, Renderer.standard_direction, true);
        }

        public static void showGame(string name, int number)
        {
            redraw_loader(0);
            Console.Clear();
            Database.Display.field = Storage.getGameSnap(name, number);
            redraw_loader(99);
            Renderer.draw(true, true, Renderer.standard_direction, true);
        }

        public static int[,] requestAll_rangeMode()
        {
            int y = 0;
            for (int x = 0; x < 8; x++)
            {
                Database.Display.recent[x, y] = Database.Display.field[x, y];
                if (x == 7)
                {
                    x = 0;
                    y++;
                }
                if (y == 7) x = ++y;
            }
            //Database.Display.field.CopyTo(Database.Display.recent, 0);
            int[,] disp_board =
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

            for (byte i = 0; i < 8; i++)
            {
                if (Database.Physical.error_rows.Contains(i))
                {
                    for (byte l = 0; l < 8; l++)
                    {
                        Database.Display.field[l, i] = 100;
                    }
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("Requesting Data from Arduino");
                    List<string> a = new(scom2.multiCommand($"QRANGE {i}", 8));
                    System.Diagnostics.Debug.WriteLine("Data Recieved");
                    for (byte k = 0; k < 8; k++)
                    {
                        int index;
                        byte field_full = (byte)(i * 10 + Database.Display.inverter[k]);

                        if ((index = Database.Physical.error_fields.IndexOf(field_full)) != -1)
                        {
                            if (Database.Physical.error_fields[index] / 10 == i) Database.Display.field[k, i] = 100;
                            //Console.WriteLine("Error Field Detected " + k + "," + i);
                        }
                        else
                        {
                            if (Int32.TryParse(a[k], out int temp_field))
                            {
                                //System.Diagnostics.Debug.WriteLine($"Coordinates {k}:{i}");
                                if (temp_field < (Database.Physical.max[k, i] + Database.Physical.tolerance) && temp_field > (Database.Physical.min[k, i] - Database.Physical.tolerance))
                                {
                                    //System.Diagnostics.Debug.WriteLine(temp_field + " within Bounds");
                                    Database.Physical.av[k, i] = temp_field;
                                    temp_field = 0;
                                }
                                else
                                {
                                    temp_field -= Database.Physical.av[k, i];
                                }
                                temp_field = Convert.ToInt32(Math.Ceiling(temp_field * Database.Physical.factors[k, i]));

                                Database.Display.field[k, i] = autoEnumerator(Database.Display.field, temp_field);
                                //Place Call here

                                //System.Diagnostics.Debug.WriteLine("Writing to " + k + "," + i);
                                disp_board[k, i] = temp_field;

                            }

                        }
                    }
                    //Thread.Sleep(1000);
                    System.Diagnostics.Debug.WriteLine($"Coordinates {i}");
                }
            }

            postProcessor();
            return disp_board;
        }

        public static byte autoEnumerator(byte[,] board, int magnetic)
        {
            Byte output = 0;
            Byte tolerance = Database.Physical.tolerance;
            for (int l = 0; l < Database.Physical.piece_max.Count(); l++)
            {
                if (magnetic < Database.Physical.piece_max[l] + tolerance && magnetic > Database.Physical.piece_min[l] - tolerance)
                {
                    output = Database.Physical.piece_order[l];
                    /*if (container.ContainsCount(board,output,1))
                    {
                        if ((output > 100 && output < 108) || (output > 0 && output < 8) || output == 11 || output == 13 || output == 15 || output == 111 || output == 113 || output == 115) output++;
                    }*/

                }
                else if (magnetic < Database.Physical.piece_min.Last() - tolerance || magnetic > Database.Physical.piece_max[5] + tolerance)
                {
                    output = 255;
                }
                else
                {
                    if (output == 0) { output = 200; }
                }
            }
            if (magnetic < tolerance && magnetic > -tolerance) output = 0;
            if ((output > 100 && output < 108) || (output > 0 && output < 8))
            {
                if (board.BoardContainsCount(output, 8)) output = 254;
            }
            else if (output == 110 || output == 10)
            {
                if (board.BoardContainsCount(output, 1)) output = 254;
            }
            else
            {
                if (board.BoardContainsCount(output, 2)) output = 254;
            }

            return output;
        }

        public static void postProcessor()
        {
            if (timeout_remain == 0)
            {
                Database.Display.queued.Clear();
                timeout_remain = timeout;
            }
            HashSet<int> specials = [0, 100, 200, 255, 254];
            HashSet<int> hash = [11, 13, 15, 111, 113, 115];
            byte[,] local = Database.Display.recent;
            byte[,] simplified =
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


            for (int k = 0; k < 8; k++)
            {
                for (int i = 0; i < 8; i++)
                {
                    byte output = local[k, i];
                    byte tmp = (byte)(output - 1);
                    if (specials.Contains(output)) continue;
                    if (output > 101 && output < 109) output = 101;
                    else if (output > 1 && output < 9) output = 1;
                    else if (hash.Contains(tmp)) output--;
                    simplified[k, i] = output;

                    if (output == Database.Display.field[k, i])
                    {
                        Console.WriteLine("Copying before number");
                        Database.Display.field[k, i] = local[k, i];
                    }

                }
            }

            List<int[]> diffs = Database.Display.field.CompareDiff(local);
            Console.WriteLine("Diffs:");
            foreach (int[] diff in diffs)
            {
                foreach (int i in diff) Console.Write(i + " ");
                Console.WriteLine();
            }

            byte[,] buff = Database.Display.field;
            //Code vorher - Code nachher
            List<byte> scode_from = [];
            List<byte> scode_to = [];

            foreach (int[] diff in diffs)
            {
                scode_from.Add(simplified[diff[0], diff[1]]);
                scode_to.Add(buff[diff[0], diff[1]]);
            }


            List<int> index_from = [];
            List<int> index_to = [];
            for (int i = 0; i < diffs.Count; ++i)
            {
                index_from.Add(scode_from.IndexOf(scode_to[i]));
                index_to.Add(i);
            }

            for (int i = 0; i < index_to.Count; ++i)
            {
                if (index_from[i] != -1)
                {
                    Console.WriteLine("Error Iterator " + index_from[i]);
                    int[] diff_from = diffs[index_from[i]];
                    int[] diff_to = diffs[index_to[i]];
                    Database.Display.field[diff_to[0], diff_to[1]] = local[diff_from[0], diff_from[0]];
                }
                else { /*Nur ein Feld wurde verändert*/ }
            }


            // x/10 == 1 -> Weiß else Schwarz

            /*List<int> color_from = [];
            scode_from.ForEach((x) => color_from.Add(x/100));

            List<int> color_to = [];
            scode_to.ForEach((x) => color_to.Add(x/100));

            Console.WriteLine("Color From Count: " + color_from.Count);
            Console.WriteLine("Color To Count: " + color_to.Count);
            for(int c = 0; c < color_from.Count; ++c)
            {
                Console.WriteLine("Working Index " + c);
                if(color_from[c] == color_to[c]) {
                    scode_from[c] = 0;
                }
            }*/

            //Wenn hier nicht 0 rauskommt ist die Zahl der Veränderungen ungerade => Figur muss rausgeflogen sein oder aufgestellt worden sein
            if (diffs.Count % 2 != 0)
            {
                Console.WriteLine("Non-Move erkannt");
                Console.Write("Current Qeue ");
                Database.Display.queued.ForEach((x) => Console.Write(x));
                Console.WriteLine();
                for (int i = 0; i < scode_from.Count; ++i)
                {
                    Console.WriteLine(scode_from[i] + " " + scode_to[i]);
                }
                List<int> startedZeroIndizes = scode_from.IndizesOf<byte>(0);
                List<int> endedZeroIndizes = scode_to.IndizesOf<byte>(0);

                List<int[]> startedZero = [];
                foreach (int index in startedZeroIndizes) startedZero.Add(diffs[index]);

                List<int[]> endedZero = [];
                foreach (int index in endedZeroIndizes) endedZero.Add(diffs[index]);

                Console.WriteLine("Started ZeroIndizes " + startedZeroIndizes.Count);
                Console.WriteLine("Started Zero " + startedZero.Count);

                foreach (int[] item in endedZero)
                {
                    if (!specials.Contains(Database.Display.recent[item[0], item[1]]))
                        Database.Display.queued.Add(Database.Display.recent[item[0], item[1]]);
                }

                foreach (int[] item in startedZero)
                {
                    Console.WriteLine("Neue Figur erkannt");
                    byte output = Database.Display.field[item[0], item[1]];

                    List<byte> qeue_simple = [];
                    foreach (byte q in Database.Display.queued)
                    {
                        if (q > 101 && q < 109) qeue_simple.Add(101);
                        else if (q > 1 && q < 9) qeue_simple.Add(1);
                        else if (hash.Contains(q - 1)) qeue_simple.Add((byte)(q - 1));
                        else qeue_simple.Add(q);
                    }

                    //3.4 16:00 FEIG 

                    int i = qeue_simple.IndexOf(output);

                    if (i != -1)
                    {
                        Console.WriteLine("Figur in der Qeue gefunden!");
                        output = Database.Display.queued[i];
                        Database.Display.queued.RemoveAt(i);
                    }
                    else
                    {
                        //FIGUR ERSCHIENEN?!
                        //Neue Nummer, denn vielleicht wird das Brett noch aufgebaut
                        if (specials.Contains(output))
                        {

                        }
                        else
                        {
                            //Weißen Bauern neuen Code zuweisen
                            Console.WriteLine("Code zuweisen");
                            if (output == 101 && Database.Display.field.BoardContainsCount(101, 1))
                            {
                                while (output > 100 && output < 108 && Database.Display.field.Contains(output))
                                {
                                    Console.WriteLine("Schwarzer Bauer zuweisen");
                                    if (output++ == 108) output = 254;
                                }
                            }


                            if (output == 1 && Database.Display.field.BoardContainsCount(1, 1))
                            {
                                while (output > 0 && output < 8 && Database.Display.field.Contains(output))
                                {
                                    Console.WriteLine("Schwarzer Bauer zuweisen");
                                    if (output++ == 8) output = 254;
                                }
                            }

                            //Anderen Figuren neue Nummer zuweisen
                            while (hash.Contains(output) && Database.Display.field.BoardContainsCount(output, 1))
                            {
                                byte mod = (byte)(output % 2);
                                output += mod;
                                if (mod == 0) output = 254;
                            }

                        }

                    }
                    Database.Display.field[item[0], item[1]] = output;
                    //Database.Display.queued.Add(Database.Display.recent[item[0], item[1]]);
                }

                foreach (byte code in scode_from)
                {
                    if (scode_to.IndexOf(code) == -1)
                    {

                        //Figur gespeichert in "code" ist raus oder angehoben
                    }
                }
            }
            timeout_remain--;
        }
    }
}
