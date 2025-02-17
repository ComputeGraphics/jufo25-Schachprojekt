using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessCORE
{

    /* ENCODER:
     * Black: ♔ ♕ ♖ ♗ ♘ ♙   White:♚ ♛ ♜ ♝ ♞ ♟
     * 
     * Generell:
     *   Leeres Feld: 000
     * X Nicht Betriebsfähig: 100  
     * * Laden: 200
     * ? Nicht in Datenbank: 255
     * # Unzulässige Figurenanzahl: 254
     * # Platz zur Definition weiterer Figuren: 201-216
     * 
     * 
     * Schwarz:
     * ♙ (S) Bauer: 001-8
     * ♕ (S) Dame: 009
     * ♔ (S) König: 010
     * ♖ (S) Turm: 011-2
     * ♗ (S) Läufer: 013-4
     * ♘ (S) Springer: 015-6
     * 
     * Weiß:
     * ♟ (W) Bauer: 101-8
     * ♛ (W) Dame: 109
     * ♚ (W) König: 110
     * ♜ (W) Turm: 111-2
     * ♝ (W) Läufer: 113-114
     * ♞ (W) Springer: 115-116
     * 
     * ♟
     */

    internal class Database
    {
        public static class Display
        {
            /*public static Dictionary<byte,byte> inverter = new()
            {
                { 7,0 },
                { 6,1 },
                { 5,2 },
                { 4,3 },
                { 3,4 },
                { 2,5 },
                { 1,6 },
                { 0,7 },
            };*/
            public static byte[] inverter = [7,6,5,4,3,2,1,0];

            public static Dictionary<byte,string> translator = new()
            {
                    //General
                    { 0, "  " },
                    { 100, "X " },
                    { 200, "* " },
                    { 254, "# " },
                    { 255, "? " },
                    //Placeholders
                    { 201, "  " },
                    { 202, "  " },
                    { 203, "  " },
                    { 204, "  " },
                    { 205, "  " },
                    { 206, "  " },
                    { 207, "  " },
                    { 208, "  " },
                    { 209, "  " },
                    { 210, "  " },
                    { 211, "  " },
                    { 212, "  " },
                    { 213, "  " },
                    { 214, "  " },
                    { 215, "  " },
                    { 216, "  " },
                    //Weiß
                    { 101, "♟ " },
                    { 102, "♟ " },
                    { 103, "♟ " },
                    { 104, "♟ " },
                    { 105, "♟ " },
                    { 106, "♟ " },
                    { 107, "♟ " },
                    { 108, "♟ " },
                    { 109, "♛ " },
                    { 110, "♚ " },
                    { 111, "♜ " },
                    { 112, "♜ " },
                    { 113, "♝ " },
                    { 114, "♝ " },
                    { 115, "♞ " },
                    { 116, "♞ " },
                    //Schwarz
                    { 1, "♙ " },
                    { 2, "♙ " },
                    { 3, "♙ " },
                    { 4, "♙ " },
                    { 5, "♙ " },
                    { 6, "♙ " },
                    { 7, "♙ " },
                    { 8, "♙ " },
                    { 9, "♕ " },
                    { 10, "♔ " },
                    { 11, "♖ " },
                    { 12, "♖ " },
                    { 13, "♗ " },
                    { 14, "♗ " },
                    { 15, "♘ " },
                    { 16, "♘ " },

                };

            public static byte[,] field =
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

            public static byte[,] buffer =
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

            public static void writeSample()
            {
                Storage.log("Write Sample to Database");
                byte[,] sample =
                {
                { 11, 15, 13, 9, 10, 14, 16, 12 },
                { 1, 2, 3, 4, 5, 6, 7, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 101, 102, 103, 104, 105, 106, 107, 108 },
                { 111, 115, 113, 109, 110, 114, 116, 112 },
                };

                field = sample;
            }
            public static void Clear()
            {
                Storage.log("Clear Database");
                byte[,] cleared =
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
                field = cleared;
            }

            public static void ApplyPawnFix()
            {

                for (byte i = 101; i < 9; i++)
                {
                    translator[i] = "♟";
                }
                Storage.log("Applied Pawn Space Fix");
            }
        }

        public static class Physical
        {
            public static byte recalib_iterations = 1;
            public static byte default_calib = 63;
            public static int default_av = 0;
            public static byte tolerance = 4;
            public static bool calib = true;
            public static bool repeat_selftest = false;
            public static List<byte> error_rows = [];
            public static List<byte> error_fields = [];
            //FELDMISSWEISUNG
            public static int[,] min =
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
            public static int[,] max =
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
            public static int[,] av =
            {
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
                { default_av, default_av, default_av, default_av, default_av, default_av, default_av, default_av },
            };

            public static double[,] factors =
            {
                { 1,    1.04, 1.07, 1,    0.78, 0.71, 1.12, 1.04 },
                { 1,    1,    1,    1.05, 0.72, 0.71, 1,    1    },
                { 1,    1.06, 1,    1.06, 0.72, 0.71, 1.05, 1.06 },
                { 1,    1.04, 1.04, 1.05, 0.75, 0.72, 0.74, 1    },
                { 1,    1,    1,    1,    0.74, 0.71, 0.75, 1    },
                { 1,    1,    1.06, 1,    0.77, 0.72, 0.75, 1    },
                { 1,    1,    1,    1.06, 0.77, 0.72, 0.73, 1    },
                { 1.06, 1,    1.09, 1.06, 0.75, 0.74, 1,    1    },
            };

            public static void calibrate(byte res,byte data_count)
            {
                Storage.log("Requesting Error Logs...");
                //Frage nach Selbstestergebnissen
                if (repeat_selftest)
                {
                    scom2.sendCommand("test");
                }
                //Console.WriteLine("Send Test Request");
                string[] error_list = scom2.sendCommand("result").Split(',');
                /*string errors = scom2.sendCommand("result");
                Storage.log(errors);
                string[] error_list = errors.Split(',');*/

                Console.WriteLine("Result Command Returned");
                foreach (string error in error_list)
                {

                    if (error.StartsWith("EM:"))
                    {
                        error_rows.Add(Byte.Parse(error[3..].Trim()));
                    }
                    else if (error.StartsWith("EP:"))
                    {
                        error_fields.Add(Byte.Parse(error[3..].Trim()));
                    }
                }
                foreach (byte error in error_rows)
                {
                    Console.WriteLine("Error in Row " + error);
                }
                foreach (byte error in error_fields)
                {
                    System.Diagnostics.Debug.WriteLine("Error in Field " + error);
                }
                Storage.log("Error Logs Recieved!");
                board_visual.redraw_loader(8);

                //Res 0 -> nur A1
                //Res 3 -> A1,A8 + H1,H8
                //Res 7 -> A1,B2,C3,D4,E5,F6,G7,H8 (Schwarze Diagonale)
                //         00,11,22,33,44,55,66,77
                //Res 31 -> Alle Schwarzen Felder
                //Res 63 -> Alle Felder
                //Res 131 -> Alle weißen Felder
                //Res 102 -> A1 bis H4  !
                //Res 103 -> A5 bis H8  !
                //Res 253 -> Random 8   !
                //Res 254 -> Random 16  !
                //Res 255 -> Random 32  !

                if (res == 0)
                {
                    Storage.log("Calibration in RES0 Mode...");
                    int average = 0;
                    List<string> list = scom2.multiCommand("QSTREAM 00",data_count);
                    board_visual.redraw_loader(30);

                    List<int> toint = [];
                    foreach (string content in list)
                    {
                        if (Int32.TryParse(content,out int itemint))
                        {
                            toint.Add(Int32.Parse(content));
                            average += itemint;
                        }
                    }
                    average /= toint.Count;
                    toint.Sort();
                    board_visual.redraw_loader(40);

                    int[,] avg =
                    {
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                        { average, average, average, average, average, average, average, average },
                    };
                    av = avg;
                    board_visual.redraw_loader(50);
                    int minimum = toint.First();
                    int[,] minimums =
                    {
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                        { minimum, minimum, minimum, minimum, minimum, minimum, minimum, minimum },
                    };
                    min = minimums;
                    board_visual.redraw_loader(60);
                    int maximum = toint.Last();
                    int[,] maximums =
                    {
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                        { maximum, maximum, maximum, maximum, maximum, maximum, maximum, maximum },
                    };
                    max = maximums;
                    board_visual.redraw_loader(70);

                }
                else if (res == 3)
                {
                    // 00 07 70 77
                    Storage.log("Calibration in RES3 Mode...");
                    int[] average = [0,0,0,0];
                    ImmutableList<List<string>> list = [];
                    int progressor = 10; //to 70 (55)
                    for (int i = 0; i < 78; i += 7)
                    {
                        if (i == 14) i += 56;
                        //Console.WriteLine("Mode3 Read " + i);ö
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        list = list.Add(temp);
                        progressor += 11;
                        board_visual.redraw_loader(progressor);
                    }

                    List<List<int>> toint = [];
                    int j = 0;
                    foreach (List<string> content in list)
                    {
                        List<int> temp_int = [];
                        foreach (string member in content)
                        {
                            if (Int32.TryParse(member,out int itemint))
                            {
                                temp_int.Add(itemint);
                                average[j] += itemint;
                            }
                        }
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }
                    board_visual.redraw_loader(55);
                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[2], average[2], average[2], average[2], average[3], average[3], average[3], average[3], },
                        { average[2], average[2], average[2], average[2], average[3], average[3], average[3], average[3], },
                        { average[2], average[2], average[2], average[2], average[3], average[3], average[3], average[3], },
                        { average[2], average[2], average[2], average[2], average[3], average[3], average[3], average[3], },
                        { average[0], average[0], average[0], average[0], average[1], average[1], average[1], average[1], },
                        { average[0], average[0], average[0], average[0], average[1], average[1], average[1], average[1], },
                        { average[0], average[0], average[0], average[0], average[1], average[1], average[1], average[1], },
                        { average[0], average[0], average[0], average[0], average[1], average[1], average[1], average[1], },
                    };
                    av = avg;
                    board_visual.redraw_loader(60);
                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    min = (new int[,]
                    {
                        { minimums[2], minimums[2], minimums[2], minimums[2], minimums[3], minimums[3], minimums[3], minimums[3] },
                        { minimums[2], minimums[2], minimums[2], minimums[2], minimums[3], minimums[3], minimums[3], minimums[3] },
                        { minimums[2], minimums[2], minimums[2], minimums[2], minimums[3], minimums[3], minimums[3], minimums[3] },
                        { minimums[2], minimums[2], minimums[2], minimums[2], minimums[3], minimums[3], minimums[3], minimums[3] },
                        { minimums[0], minimums[0], minimums[0], minimums[0], minimums[1], minimums[1], minimums[1], minimums[1] },
                        { minimums[0], minimums[0], minimums[0], minimums[0], minimums[1], minimums[1], minimums[1], minimums[1] },
                        { minimums[0], minimums[0], minimums[0], minimums[0], minimums[1], minimums[1], minimums[1], minimums[1] },
                        { minimums[0], minimums[0], minimums[0], minimums[0], minimums[1], minimums[1], minimums[1], minimums[1] },
                    });

                    List<int> maximums = [];
                    board_visual.redraw_loader(65);
                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[2], maximums[2], maximums[2], maximums[2], maximums[3], maximums[3], maximums[3], maximums[3] },
                        { maximums[2], maximums[2], maximums[2], maximums[2], maximums[3], maximums[3], maximums[3], maximums[3] },
                        { maximums[2], maximums[2], maximums[2], maximums[2], maximums[3], maximums[3], maximums[3], maximums[3] },
                        { maximums[2], maximums[2], maximums[2], maximums[2], maximums[3], maximums[3], maximums[3], maximums[3] },
                        { maximums[0], maximums[0], maximums[0], maximums[0], maximums[1], maximums[1], maximums[1], maximums[1] },
                        { maximums[0], maximums[0], maximums[0], maximums[0], maximums[1], maximums[1], maximums[1], maximums[1] },
                        { maximums[0], maximums[0], maximums[0], maximums[0], maximums[1], maximums[1], maximums[1], maximums[1] },
                        { maximums[0], maximums[0], maximums[0], maximums[0], maximums[1], maximums[1], maximums[1], maximums[1] },
                    };
                    max = maxis;
                    board_visual.redraw_loader(70);
                }
                else if (res == 7)
                {
                    // 00 07 70 77
                    Storage.log("Calibration in RES7 Mode...");
                    int[] average = [0,0,0,0,0,0,0,0];
                    ImmutableList<List<string>> list = [];
                    int progressor = 10; //70 (55)
                    for (byte i = 0; i < 8; i++)
                    {
                        //Console.WriteLine("Mode7 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}{i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
                        progressor += 6;
                        board_visual.redraw_loader(progressor);
                    }


                    List<List<int>> toint = [];
                    int j = 0;
                    //Console.WriteLine("List Count: " + list.Count);
                    foreach (List<string> content in list)
                    {
                        //Console.WriteLine("Working List: " + j);
                        List<int> temp_int = [];
                        //Console.WriteLine("Content Count: " + content.Count);
                        for (int i = 0; i < content.Count; i++)
                        {
                            //Console.WriteLine("Inner Iteration " + i+"\nContaining: " + content[i]);
                            int itemint = Int32.Parse(content[i]);
                            temp_int.Add(itemint);
                            average[j] += itemint;
                        }
                        Console.WriteLine();
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }
                    board_visual.redraw_loader(55);
                    int[,] avg =
                    {
                        { average[7], average[7], average[7], average[7], average[7], average[7], average[7], average[7], },
                        { average[6], average[6], average[6], average[6], average[6], average[6], average[6], average[6], },
                        { average[5], average[5], average[5], average[5], average[5], average[5], average[5], average[5], },
                        { average[4], average[4], average[4], average[4], average[4], average[4], average[4], average[4], },
                        { average[3], average[3], average[3], average[3], average[3], average[3], average[3], average[3], },
                        { average[2], average[2], average[2], average[2], average[2], average[2], average[2], average[2], },
                        { average[1], average[1], average[1], average[1], average[1], average[1], average[1], average[1], },
                        { average[0], average[0], average[0], average[0], average[0], average[0], average[0], average[0], },
                    };
                    av = avg;
                    board_visual.redraw_loader(60);
                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[7], minimums[7], minimums[7], minimums[7], minimums[7], minimums[7], minimums[7], minimums[7], },
                        { minimums[6], minimums[6], minimums[6], minimums[6], minimums[6], minimums[6], minimums[6], minimums[6], },
                        { minimums[5], minimums[5], minimums[5], minimums[5], minimums[5], minimums[5], minimums[5], minimums[5], },
                        { minimums[4], minimums[4], minimums[4], minimums[4], minimums[4], minimums[4], minimums[4], minimums[4], },
                        { minimums[3], minimums[3], minimums[3], minimums[3], minimums[3], minimums[3], minimums[3], minimums[3], },
                        { minimums[2], minimums[2], minimums[2], minimums[2], minimums[2], minimums[2], minimums[2], minimums[2], },
                        { minimums[1], minimums[1], minimums[1], minimums[1], minimums[1], minimums[1], minimums[1], minimums[1], },
                        { minimums[0], minimums[0], minimums[0], minimums[0], minimums[0], minimums[0], minimums[0], minimums[0], },
                    };
                    min = minis;

                    List<int> maximums = [];
                    board_visual.redraw_loader(65);

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[7], maximums[7], maximums[7], maximums[7], maximums[7], maximums[7], maximums[7], maximums[7], },
                        { maximums[6], maximums[6], maximums[6], maximums[6], maximums[6], maximums[6], maximums[6], maximums[6], },
                        { maximums[5], maximums[5], maximums[5], maximums[5], maximums[5], maximums[5], maximums[5], maximums[5], },
                        { maximums[4], maximums[4], maximums[4], maximums[4], maximums[4], maximums[4], maximums[4], maximums[4], },
                        { maximums[3], maximums[3], maximums[3], maximums[3], maximums[3], maximums[3], maximums[3], maximums[3], },
                        { maximums[2], maximums[2], maximums[2], maximums[2], maximums[2], maximums[2], maximums[2], maximums[2], },
                        { maximums[1], maximums[1], maximums[1], maximums[1], maximums[1], maximums[1], maximums[1], maximums[1], },
                        { maximums[0], maximums[0], maximums[0], maximums[0], maximums[0], maximums[0], maximums[0], maximums[0], },
                    };
                    max = maxis;
                    board_visual.redraw_loader(70);
                }
                else if (res == 31)
                {
                    // 00 07 70 77
                    Storage.log("Calibration in RES31 Mode...");
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    ImmutableList<List<string>> list = [];
                    int progressor = 10; //55
                    for (int i = 0; i < 78; i += 2)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 8) i += 2;
                        //Console.WriteLine("Mode31 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);

                        progressor += 2;
                        board_visual.redraw_loader(progressor);
                    }

                    List<List<int>> toint = [];
                    int j = 0;
                    foreach (List<string> content in list)
                    {
                        List<int> temp_int = [];
                        foreach (string member in content)
                        {
                            if (Int32.TryParse(member,out int itemint))
                            {
                                temp_int.Add(itemint);
                                average[j] += itemint;
                            }
                        }
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }
                    board_visual.redraw_loader(75);
                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[3], average[7], average[11], average[15], average[19], average[23], average[27], average[31], },
                        { average[3], average[7], average[11], average[15], average[19], average[23], average[27], average[31], },
                        { average[2], average[6], average[10], average[14], average[18], average[22], average[26], average[30], },
                        { average[2], average[6], average[10], average[14], average[18], average[22], average[26], average[30], },
                        { average[1], average[5],  average[9], average[13], average[17], average[21], average[25], average[29], },
                        { average[1], average[5],  average[9], average[13], average[17], average[21], average[25], average[29], },
                        { average[0], average[4],  average[8], average[12], average[16], average[20], average[24], average[28], },
                        { average[0], average[4],  average[8], average[12], average[16], average[20], average[24], average[28], },
                    };
                    av = avg;
                    board_visual.redraw_loader(77);
                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[3], minimums[7], minimums[11], minimums[15], minimums[19], minimums[23], minimums[27], minimums[31], },
                        { minimums[3], minimums[7], minimums[11], minimums[15], minimums[19], minimums[23], minimums[27], minimums[31], },
                        { minimums[2], minimums[6], minimums[10], minimums[14], minimums[18], minimums[22], minimums[26], minimums[30], },
                        { minimums[2], minimums[6], minimums[10], minimums[14], minimums[18], minimums[22], minimums[26], minimums[30], },
                        { minimums[1], minimums[5],  minimums[9], minimums[13], minimums[17], minimums[21], minimums[25], minimums[29], },
                        { minimums[1], minimums[5],  minimums[9], minimums[13], minimums[17], minimums[21], minimums[25], minimums[29], },
                        { minimums[0], minimums[4],  minimums[8], minimums[12], minimums[16], minimums[20], minimums[24], minimums[28], },
                        { minimums[0], minimums[4],  minimums[8], minimums[12], minimums[16], minimums[20], minimums[24], minimums[28], },
                    };
                    min = minis;
                    board_visual.redraw_loader(79);
                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[3], maximums[7], maximums[11], maximums[15], maximums[19], maximums[23], maximums[27], maximums[31], },
                        { maximums[3], maximums[7], maximums[11], maximums[15], maximums[19], maximums[23], maximums[27], maximums[31], },
                        { maximums[2], maximums[6], maximums[10], maximums[14], maximums[18], maximums[22], maximums[26], maximums[30], },
                        { maximums[2], maximums[6], maximums[10], maximums[14], maximums[18], maximums[22], maximums[26], maximums[30], },
                        { maximums[1], maximums[5],  maximums[9], maximums[13], maximums[17], maximums[21], maximums[25], maximums[29], },
                        { maximums[1], maximums[5],  maximums[9], maximums[13], maximums[17], maximums[21], maximums[25], maximums[29], },
                        { maximums[0], maximums[4],  maximums[8], maximums[12], maximums[16], maximums[20], maximums[24], maximums[28], },
                        { maximums[0], maximums[4],  maximums[8], maximums[12], maximums[16], maximums[20], maximums[24], maximums[28], },
                    };
                    max = maxis;
                    board_visual.redraw_loader(80);
                }
                else if (res == 63)
                {
                    // 00 07 70 77
                    Storage.log("Calibration in RES63 Mode...");
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];

                    ImmutableList<List<string>> list = [];
                    int progressor = 10;
                    for (int i = 0; i < 78; i++)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 8) i++;
                        if (testfor == 8) i++;
                        //Console.WriteLine("Mode63 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
                        progressor++;
                        board_visual.redraw_loader(progressor);
                    }

                    List<List<int>> toint = [];
                    int j = 0;

                    foreach (List<string> content in list)
                    {
                        System.Diagnostics.Debug.Write("Working List: " + j);
                        List<int> temp_int = [];
                        foreach (string member in content)
                        {
                            System.Diagnostics.Debug.Write(" " + member);
                            if (Int32.TryParse(member,out int itemint))
                            {
                                temp_int.Add(itemint);
                                average[j] += itemint;
                            }
                        }

                        average[j] /= temp_int.Count;
                        System.Diagnostics.Debug.Write(" AV:" + average[j]);
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }
                    board_visual.redraw_loader(75);
                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[7], average[15], average[23], average[31], average[39], average[47], average[55], average[63], },
                        { average[6], average[14], average[22], average[30], average[38], average[46], average[54], average[62], },
                        { average[5], average[13], average[21], average[29], average[37], average[45], average[53], average[61], },
                        { average[4], average[12], average[20], average[28], average[36], average[44], average[52], average[60], },
                        { average[3], average[11], average[19], average[27], average[35], average[43], average[51], average[59], },
                        { average[2], average[10], average[18], average[26], average[34], average[42], average[50], average[58], },
                        { average[1], average[9], average[17], average[25], average[33], average[41], average[49], average[57], },
                        { average[0], average[8], average[16], average[24], average[32], average[40], average[48], average[56], },
                    };
                    av = avg;
                    board_visual.redraw_loader(76);

                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[7], minimums[15], minimums[23], minimums[31], minimums[39], minimums[47], minimums[55], minimums[63], },
                        { minimums[6], minimums[14], minimums[22], minimums[30], minimums[38], minimums[46], minimums[54], minimums[62], },
                        { minimums[5], minimums[13], minimums[21], minimums[29], minimums[37], minimums[45], minimums[53], minimums[61], },
                        { minimums[4], minimums[12], minimums[20], minimums[28], minimums[36], minimums[44], minimums[52], minimums[60], },
                        { minimums[3], minimums[11], minimums[19], minimums[27], minimums[35], minimums[43], minimums[51], minimums[59], },
                        { minimums[2], minimums[10], minimums[18], minimums[26], minimums[34], minimums[42], minimums[50], minimums[58], },
                        { minimums[1], minimums[9], minimums[17], minimums[25], minimums[33], minimums[41], minimums[49], minimums[57], },
                        { minimums[0], minimums[8], minimums[16], minimums[24], minimums[32], minimums[40], minimums[48], minimums[56], },
                    };
                    min = minis;
                    board_visual.redraw_loader(78);

                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[7], maximums[15], maximums[23], maximums[31], maximums[39], maximums[47], maximums[55], maximums[63], },
                        { maximums[6], maximums[14], maximums[22], maximums[30], maximums[38], maximums[46], maximums[54], maximums[62], },
                        { maximums[5], maximums[13], maximums[21], maximums[29], maximums[37], maximums[45], maximums[53], maximums[61], },
                        { maximums[4], maximums[12], maximums[20], maximums[28], maximums[36], maximums[44], maximums[52], maximums[60], },
                        { maximums[3], maximums[11], maximums[19], maximums[27], maximums[35], maximums[43], maximums[51], maximums[59], },
                        { maximums[2], maximums[10], maximums[18], maximums[26], maximums[34], maximums[42], maximums[50], maximums[58], },
                        { maximums[1], maximums[9],  maximums[17], maximums[25], maximums[33], maximums[41], maximums[49], maximums[57], },
                        { maximums[0], maximums[8],  maximums[16], maximums[24], maximums[32], maximums[40], maximums[48], maximums[56], },
                    };
                    max = maxis;
                    board_visual.redraw_loader(80);
                }
                else if (res == 131)
                {
                    // 00 07 70 77
                    Storage.log("Calibration in RES131 Mode...");
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    ImmutableList<List<string>> list = [];
                    int progressor = 10; //55
                    for (int i = 1; i < 79; i += 2)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 9) i += 2;
                        //Console.WriteLine("Mode31 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);

                        progressor += 2;
                        board_visual.redraw_loader(progressor);
                    }

                    List<List<int>> toint = [];
                    int j = 0;
                    foreach (List<string> content in list)
                    {
                        List<int> temp_int = [];
                        foreach (string member in content)
                        {
                            if (Int32.TryParse(member,out int itemint))
                            {
                                temp_int.Add(itemint);
                                average[j] += itemint;
                            }
                        }
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }
                    board_visual.redraw_loader(75);
                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[3], average[7], average[11], average[15], average[19], average[23], average[27], average[31], },
                        { average[3], average[7], average[11], average[15], average[19], average[23], average[27], average[31], },
                        { average[2], average[6], average[10], average[14], average[18], average[22], average[26], average[30], },
                        { average[2], average[6], average[10], average[14], average[18], average[22], average[26], average[30], },
                        { average[1], average[5],  average[9], average[13], average[17], average[21], average[25], average[29], },
                        { average[1], average[5],  average[9], average[13], average[17], average[21], average[25], average[29], },
                        { average[0], average[4],  average[8], average[12], average[16], average[20], average[24], average[28], },
                        { average[0], average[4],  average[8], average[12], average[16], average[20], average[24], average[28], },
                    };
                    av = avg;
                    board_visual.redraw_loader(77);
                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[3], minimums[7], minimums[11], minimums[15], minimums[19], minimums[23], minimums[27], minimums[31], },
                        { minimums[3], minimums[7], minimums[11], minimums[15], minimums[19], minimums[23], minimums[27], minimums[31], },
                        { minimums[2], minimums[6], minimums[10], minimums[14], minimums[18], minimums[22], minimums[26], minimums[30], },
                        { minimums[2], minimums[6], minimums[10], minimums[14], minimums[18], minimums[22], minimums[26], minimums[30], },
                        { minimums[1], minimums[5],  minimums[9], minimums[13], minimums[17], minimums[21], minimums[25], minimums[29], },
                        { minimums[1], minimums[5],  minimums[9], minimums[13], minimums[17], minimums[21], minimums[25], minimums[29], },
                        { minimums[0], minimums[4],  minimums[8], minimums[12], minimums[16], minimums[20], minimums[24], minimums[28], },
                        { minimums[0], minimums[4],  minimums[8], minimums[12], minimums[16], minimums[20], minimums[24], minimums[28], },
                    };
                    min = minis;
                    board_visual.redraw_loader(79);
                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[3], maximums[7], maximums[11], maximums[15], maximums[19], maximums[23], maximums[27], maximums[31], },
                        { maximums[3], maximums[7], maximums[11], maximums[15], maximums[19], maximums[23], maximums[27], maximums[31], },
                        { maximums[2], maximums[6], maximums[10], maximums[14], maximums[18], maximums[22], maximums[26], maximums[30], },
                        { maximums[2], maximums[6], maximums[10], maximums[14], maximums[18], maximums[22], maximums[26], maximums[30], },
                        { maximums[1], maximums[5],  maximums[9], maximums[13], maximums[17], maximums[21], maximums[25], maximums[29], },
                        { maximums[1], maximums[5],  maximums[9], maximums[13], maximums[17], maximums[21], maximums[25], maximums[29], },
                        { maximums[0], maximums[4],  maximums[8], maximums[12], maximums[16], maximums[20], maximums[24], maximums[28], },
                        { maximums[0], maximums[4],  maximums[8], maximums[12], maximums[16], maximums[20], maximums[24], maximums[28], },
                    };
                    max = maxis;
                    board_visual.redraw_loader(80);

                }
                Storage.log("Calibration finished");


                for (byte i = 0; i <= recalib_iterations; i++)
                {
                    Storage.log("DynRecalib Iteration " + i);
                    //DynRecalib();
                }
                board_visual.redraw_loader(90);
            }

            public static void DynRecalib()
            {
                int[,] disp_board = board_visual.requestAll_rangeMode();
                System.Diagnostics.Debug.WriteLine(disp_board.Length);
                int j = 0;
                for (int i = 0; i < 8; i++)
                {
                    System.Diagnostics.Debug.WriteLine(i.ToString() + j.ToString());
                    //if (disp_board[i,j] < -tolerance || disp_board[i,j] > tolerance)
                    min[i,j] += disp_board[i,j] - tolerance;
                    max[i,j] += disp_board[i,j] + tolerance;
                    av[i,j] += disp_board[i,j];

                    if (i > 6 && j != 7)
                    {
                        j++;
                        i = -1;
                    }
                }
            }

            public static byte[] detector_fields = [00,10,20,30,40,01,06,07,17,27,37,47];

            /*
            453-455
            KALIB AV 454

            BAUER WEIß 741-746 - 287-292 -> 216-350
            SPRINGER WEIß 467-471 - 13-17 -> tolerance-27
            TURM WEIß 524-548 - 70-94 -> 61-110
            DAME WEIß 610-631 - 156-177 -> 142-216
            LÄUFER WEIß 479-490 - 25-36 -> 27-61
            KÖNIG WEIß 570-595 - 116-141 -> 105-142

            BAUER SCHWARZ 180-198 - -274--256 -> (-350)-(-265)
            DAME SCHWARZ 260-300 - -194--154 -> (-265)-(-180) !
            SPRINGER SCHWARZ 427-445 - -27--9 -> (-27)-(-tolerance)
            KÖNIG SCHAWRZ 318-340 - -136--114 -> (-180)-(-102) !
            LÄUFER SCHWARZ 405-427 - -49--27 -> (-52)-(-27)
            TURM SCHWARZ            -101--66 (-102)-(-52)
            */
            //FIGUREN
            //public static int[] piece_av = [0,0,0,0,0,0,0,0,0,0,0,0];
            public static int[] piece_min = [61,tolerance,27,142,110,216,-102,-27,-52,-265,-180,-350];
            public static int[] piece_max = [110,27,61,216,142,350,-52,-tolerance,-27,-180,-102,-265];
            public static byte[] piece_order = [111,115,113,109,110,101,11,15,13,09,10,01];
            //Turm,Springer,Läufer,Dame,König,Bauer
            public static void calib_pieces(byte data_count)
            {
                //Bauer weiß Auf 00
                Storage.log("Auto Figure Calibration started... (EXPERIMENTAL)");
                Console.WriteLine("Place all Pieces on the desired fields. Software will automatically identify after the default chess configuration.\nPress [ENTER] to start the identification");
                while (ConsoleKey.Enter != Console.ReadKey().Key) { }
                int i = 0;
                foreach (byte a in detector_fields)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking {a} at {i}");
                    List<string> temp = new(scom2.multiCommand($"QSTREAM {a}",data_count));
                    List<int> toint = [];

                    byte front = (byte)(a / 10);
                    byte back = a;
                    for (; back > 9; back -= 10) { }

                    int avg = 0;
                    foreach (string list in temp)
                    {
                        if (Int32.TryParse(list,out int itemint))
                        {
                            Console.WriteLine(front + " " + back + " " + Database.Physical.av[Display.inverter[front],back]);
                            itemint -= Database.Physical.av[Display.inverter[front],back];
                            toint.Add(itemint);
                            avg += itemint;
                        }
                    }

                    toint.Sort();
                    avg /= toint.Count;
                    piece_min[i] = toint.First();
                    piece_max[i] = toint.Last();
                    //piece_av[i] = avg;
                    i++;
                }

                Storage.log("Auto Figure Calibration finished (EXPERIMENTAL)");
                /*foreach(int element in piece_av)
                {
                    System.Diagnostics.Debug.WriteLine(element);
                }*/

            }


        }

    }
}
