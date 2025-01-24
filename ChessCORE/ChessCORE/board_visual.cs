using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace ChessCORE
{
    internal class board_visual
    {
        //□■█▓▰
        public int loader_progress = 0;

        public static void redraw_loader(string loader)
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
            Console.WriteLine(loader.PadLeft((Console.WindowWidth / 2) + (loader.Length/2)) + "".PadRight((Console.WindowWidth / 2) + (loader.Length / 2)));

        }
        public static void show()
        {
            redraw_loader("[                    ]");
            scom2.init();
            redraw_loader("[▰                   ]");

            if (database.physical.repeat_selftest)scom2.sendCommand("test");
            redraw_loader("[▰▰                  ]");

            if (database.physical.calib) database.physical.calibrate(database.physical.default_calib,scom2.default_count);
            redraw_loader("[▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰  ]");

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

            redraw_loader("[▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰▰]");
            //Renderer.draw_number(true,true,0);
            Renderer.draw(true,true,Renderer.standard_direction);
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
                        byte field_full = (byte)(i*10 + k);
                        if ((index = database.physical.error_fields.IndexOf(field_full)) != -1)
                        {
                            if (database.physical.error_fields[index] / 10 == i) database.display.field[database.display.inverter[k],i] = 100;
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

                                for(int l = 0; l < database.physical.piece_max.Count();  l++)
                                {
                                    if (temp_field < database.physical.piece_max[l] + database.physical.tolerance && temp_field > database.physical.piece_min[l] - database.physical.tolerance)
                                    {
                                        database.display.field[k,i] = database.physical.piece_order[l];
                                    }
                                }
                                if (temp_field < database.physical.tolerance && temp_field > -database.physical.tolerance) database.display.field[k,i] = 0;

                                System.Diagnostics.Debug.WriteLine("Writing to " + k + "," + i);
                                disp_board[k,i] = temp_field;

                            }
                        }
                    }
                    //Thread.Sleep(3000);
                }
            }

            return disp_board;
        }



    }

}
