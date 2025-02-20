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


namespace ChessCORE
{
    internal class board_visual
    {
        public static bool show_icons = true;
        public static bool show_magnetic = false;
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

            if (Database.Physical.calib) Database.Physical.calibrate(Database.Physical.default_calib,scom2.default_count);
            redraw_loader(91);

            //Database.Physical.calib_pieces(4);

            if (scom.advanced)
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
            if(show_magnetic) Renderer.draw_number(true,true,0);
            else Renderer.draw(true,true,Renderer.standard_direction, false,!show_icons);
        }

        public static void showSnap(string name)
        {
            redraw_loader(0);
            Database.Display.field = Storage.GetCachedBoard(name);
            redraw_loader(99);
            Renderer.draw(true,true,Renderer.standard_direction,true);
        }

        public static void showGame(string name,int number)
        {
            redraw_loader(0);
            Console.Clear();
            Database.Display.field = Storage.getGameSnap(name,number);
            redraw_loader(99);
            Renderer.draw(true,true,Renderer.standard_direction,true);
        }

        public static int[,] requestAll_rangeMode()
        {
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
                        Database.Display.field[l,i] = 100;
                    }
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("Requesting Data from Arduino");
                    List<string> a = new(scom2.multiCommand($"QRANGE {i}",8));
                    System.Diagnostics.Debug.WriteLine("Data Recieved");
                    for (byte k = 0; k < 8; k++)
                    {
                        int index;
                        byte field_full = (byte)(i * 10 + Database.Display.inverter[k]);

                        if ((index = Database.Physical.error_fields.IndexOf(field_full)) != -1)
                        {
                            if (Database.Physical.error_fields[index] / 10 == i) Database.Display.field[k,i] = 100;
                            //Console.WriteLine("Error Field Detected " + k + "," + i);
                        }
                        else
                        {
                            if (Int32.TryParse(a[k],out int temp_field))
                            {
                                //System.Diagnostics.Debug.WriteLine($"Coordinates {k}:{i}");
                                if (temp_field < (Database.Physical.max[k,i] + Database.Physical.tolerance) && temp_field > (Database.Physical.min[k,i] - Database.Physical.tolerance))
                                {
                                    //System.Diagnostics.Debug.WriteLine(temp_field + " within Bounds");
                                    Database.Physical.av[k,i] = temp_field;
                                    temp_field = 0;
                                }
                                else
                                {
                                    temp_field -= Database.Physical.av[k,i];
                                }
                                temp_field = Convert.ToInt32(Math.Ceiling(temp_field * Database.Physical.factors[k,i]));

                                Database.Display.field[k,i] = autoEnumerator(Database.Display.field,temp_field);
                                //Place Call here

                                //System.Diagnostics.Debug.WriteLine("Writing to " + k + "," + i);
                                disp_board[k,i] = temp_field;

                            }

                        }
                    }
                    //Thread.Sleep(1000);
                    System.Diagnostics.Debug.WriteLine($"Coordinates {i}");
                }
            }
            return disp_board;
        }

        public static byte autoEnumerator(byte[,] board,int magnetic)
        {
            var container = new ArrayTools();
            Byte output = 0;
            Byte tolerance = Database.Physical.tolerance;
            for (int l = 0; l < Database.Physical.piece_max.Count(); l++)
            {
                if (magnetic < Database.Physical.piece_max[l] + tolerance && magnetic > Database.Physical.piece_min[l] - tolerance)
                {
                    output = Database.Physical.piece_order[l];
                    if (container.ContainsTwice(board,output,1))
                    {
                        if ((output > 100 && output < 108) || (output > 0 && output < 8) || output == 11 || output == 13 || output == 15 || output == 111 || output == 113 || output == 115) output++;
                    }

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
            if (container.ContainsTwice(board,output,7)) output = 254;

            return output;
        }

        class ArrayTools
        {
            public bool ContainsTwice(byte[,] board,byte item,int count)
            {

                if (item == 0 || item == 100 || item == 200 || item == 255) return false;
                byte contains = 0;
                foreach (byte element in board)
                {
                    if (element == item) contains++;
                }
                return contains > count;
            }

        }
    }

}
