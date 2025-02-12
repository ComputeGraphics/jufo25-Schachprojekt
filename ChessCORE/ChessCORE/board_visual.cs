using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
            //Console.WriteLine("Please wait while the system is loading...");
            Console.WriteLine("This may take a few minutes".PadLeft(Console.WindowWidth / 2 + 13) + "".PadRight(Console.WindowWidth / 2 + 14));
            Console.WriteLine("Please do not open other programs using COM".PadLeft(Console.WindowWidth / 2 + 21) + "".PadRight(Console.WindowWidth / 2 + 22));
            Console.WriteLine("Please do not turn off or disconnect the MCU".PadLeft(Console.WindowWidth / 2 + 21) + "".PadRight(Console.WindowWidth / 2 + 22));
            Console.WriteLine("Please do not press any buttons".PadLeft(Console.WindowWidth / 2 + 15) + "".PadRight(Console.WindowWidth / 2 + 16));
            for(int i = 0; i < Console.WindowHeight-13; i++){
                Console.WriteLine();
            }
            
            string loaders = "";
            double small = (double)(percentage/100.0);
            double char_count = Math.Floor(small * Console.WindowWidth-6);
            if(percentage == 99) Console.WriteLine("Press any key to continue");
            else Console.WriteLine("LOADING");
            for(int i = 0; i < char_count;i++ ) {
                loaders += '▰';
            }
            for(int i = 0; i < Math.Floor(Console.WindowWidth-6-char_count); i++) {
                loaders += ' ';
            }
            Console.Write((percentage > 9? percentage.ToString():"0" + percentage.ToString()) + "% [" + loaders + ']');

            //Console.Write(loader.PadLeft((Console.WindowWidth / 2) + (loader.Length / 2)) + "".PadRight((Console.WindowWidth / 2) + (loader.Length / 2)));
        }
        public static void show()
        {
            redraw_loader(0);
            storage.log("Initializing scom2");        
            scom2.init();
            redraw_loader(2);

            if (database.physical.repeat_selftest) scom2.sendCommand("test");
            redraw_loader(5);
            //redraw_loader("[▰▰                  ]");
            
            if (database.physical.calib) database.physical.calibrate(database.physical.default_calib,scom2.default_count);
            redraw_loader(91);

            //database.physical.calib_pieces(4);

            if (scom.advanced)
            {
                Console.WriteLine("Average:");
                int i = 1;
                foreach (int element in database.physical.av)
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
                foreach (int element in database.physical.min)
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
                foreach (int element in database.physical.max)
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

                Console.WriteLine("Press [ENTER] to acknowledge");
                while (ConsoleKey.Enter != Console.ReadKey().Key) { }
            }

            redraw_loader(99);
            //Renderer.draw_number(true,true,0);
            Renderer.draw(true,true,Renderer.standard_direction);
        }

        public static void showSnap(string name)
        {
            redraw_loader(0);
            database.display.field = storage.GetCachedBoard(name);
            redraw_loader(99);
            Renderer.draw(false, true, Renderer.standard_direction, true);
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
                if (database.physical.error_rows.Contains(i))
                {
                    for (byte l = 0; l < 8; l++)
                    {
                        database.display.field[l,i] = 100;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Requesting Data from Arduino");
                    List<string> a = new(scom2.multiCommand($"QRANGE {i}",8));
                    System.Diagnostics.Debug.WriteLine("Data Recieved");
                    for (byte k = 0; k < 8; k++)
                    {
                        int index;
                        byte field_full = (byte)(i * 10 + database.display.inverter[k]);

                        if ((index = database.physical.error_fields.IndexOf(field_full)) != -1)
                        {
                            if (database.physical.error_fields[index] / 10 == i) database.display.field[k,i] = 100;
                            System.Diagnostics.Debug.WriteLine("Error Field Detected " + k + "," + i);
                        }
                        else
                        {
                            if (Int32.TryParse(a[k],out int temp_field))
                            {
                                System.Diagnostics.Debug.WriteLine($"Coordinates {k}:{i}");
                                if (temp_field < (database.physical.max[k,i] + database.physical.tolerance) && temp_field > (database.physical.min[k,i] - database.physical.tolerance))
                                {
                                    System.Diagnostics.Debug.WriteLine(temp_field + " within Bounds");
                                    database.physical.av[k,i] = temp_field;
                                    temp_field = 0;
                                }
                                else
                                {
                                    temp_field -= database.physical.av[k,i];
                                }

                                autoEnumerator(database.display.field,temp_field);
                                //Place Call here

                                System.Diagnostics.Debug.WriteLine("Writing to " + k + "," + i);
                                disp_board[k,i] = temp_field;

                            }

                        }
                    }
                    //Thread.Sleep(1000);
                }
            }

            return disp_board;
        }

        public static byte autoEnumerator(byte[,] board, int magnetic)
        {
            var container = new ArrayTools();
            byte output = 0;
            byte tolerance = database.physical.tolerance;
            for (int l = 0; l < database.physical.piece_max.Count(); l++)
            {
                if (magnetic < database.physical.piece_max[l] + tolerance && magnetic > database.physical.piece_min[l] - tolerance)
                {
                    output = database.physical.piece_order[l];
                    if(container.Contains(board, output))
                    {
                        if ((output > 100 && output < 108) || (output > 0 && output < 8) || output == 11 || output == 13 || output == 15 || output == 111 || output == 113 || output == 115) output++;
                    }
                       
                }
                else if (magnetic < database.physical.piece_min.Last() - tolerance || magnetic > database.physical.piece_max[5] + tolerance)
                {
                    output = 255;
                }
                else
                {
                    if (output == 0) { output = 200; }
                }
            }
            if (magnetic < tolerance && magnetic > -tolerance) output = 0;

            if (container.Contains(board,output)) output = 254;

            return output;
        }

        class ArrayTools
        {
            public bool Contains(byte[,] board,byte item)
            {
                int rows = board.GetLength(0);
                int cols = board.GetLength(1);
                for (int i = 0; i < rows * cols; i++)
                {
                    int row = i / cols;
                    int col = i % cols;
                    if (board[row,col] == item)
                        return false;
                }
                return true;
            }
        }


    }

}
