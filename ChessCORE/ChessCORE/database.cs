using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
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

    internal class database
    {
        public static class display
        {
            public static Dictionary<byte,byte> inverter = new()
            {
                { 7,0 },
                { 6,1 },
                { 5,2 },
                { 4,3 },
                { 3,4 },
                { 2,5 },
                { 1,6 },
                { 0,7 },
            };

            public static Dictionary<byte,string> translator = new()
            {
                    //General
                    { 0, "  " },
                    { 100, "X " },
                    { 200, "* " },
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

            public static void writeSample()
            {

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
        }

        public static class physical
        {
            public static byte default_calib = 7;
            public static int default_av = 454;
            public static byte tolerance = 16;
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

            public static void calibrate(byte res,byte data_count)
            {
                //Frage nach Selbstestergebnissen
                if (repeat_selftest)
                {
                    scom2.sendCommand("test");
                }
                string[] error_list = scom2.sendCommand("result").Split(',');

                foreach (string error in error_list)
                {
                    System.Diagnostics.Debug.WriteLine(error);
                    if (error.StartsWith("EM:"))
                    {
                        error_rows.Add(Byte.Parse(error[3..].Trim()));
                    }
                    else if (error.StartsWith("EP:"))
                    {
                        error_fields.Add(Byte.Parse(error[3..].Trim()));
                    }
                }

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
                    int average = 0;
                    List<string> list = scom2.multiCommand("QSTREAM 00",data_count);
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

                }
                else if (res == 3)
                {
                    // 00 07 70 77
                    int[] average = [0,0,0,0];

                    ImmutableList<List<string>> list = [];
                    for (int i = 0; i < 78; i += 7)
                    {
                        if (i == 14) i += 56;
                        //Console.WriteLine("Mode3 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        list = list.Add(temp);
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

                }
                else if (res == 7)
                {
                    // 00 07 70 77
                    int[] average = [ 0,0,0,0,0,0,0,0 ];
                    ImmutableList<List<string>> list = [];

                    for (byte i = 0; i < 8; i++)
                    {
                        //Console.WriteLine("Mode7 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}{i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
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
                            Int32.TryParse(content[i],out int itemint);
                            temp_int.Add(itemint);
                            average[j] += itemint;
                        }
                        Console.WriteLine();
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }

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

                }
                else if (res == 31)
                {
                    // 00 07 70 77
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];

                    ImmutableList<List<string>> list = [];
                    for (int i = 0; i < 78; i += 2)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 8) i += 2;
                        //Console.WriteLine("Mode31 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
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

                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[28], average[28], average[29], average[29], average[30], average[30], average[31], average[31], },
                        { average[24], average[24], average[25], average[25], average[26], average[26], average[27], average[27], },
                        { average[20], average[20], average[21], average[21], average[22], average[22], average[23], average[23], },
                        { average[16], average[16], average[17], average[17], average[18], average[18], average[19], average[19], },
                        { average[12], average[12], average[13], average[13], average[14], average[14], average[15], average[15], },
                        { average[8], average[8], average[9], average[9], average[10], average[10], average[11], average[11], },
                        { average[4], average[4], average[5], average[5], average[6], average[6], average[7], average[7], },
                        { average[0], average[0], average[1], average[1], average[2], average[2], average[3], average[3], },
                    };
                    av = avg;

                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[28], minimums[28], minimums[29], minimums[29], minimums[30], minimums[30], minimums[31], minimums[31], },
                        { minimums[24], minimums[24], minimums[25], minimums[25], minimums[26], minimums[26], minimums[27], minimums[27], },
                        { minimums[20], minimums[20], minimums[21], minimums[21], minimums[22], minimums[22], minimums[23], minimums[23], },
                        { minimums[16], minimums[16], minimums[17], minimums[17], minimums[18], minimums[18], minimums[19], minimums[19], },
                        { minimums[12], minimums[12], minimums[13], minimums[13], minimums[14], minimums[14], minimums[15], minimums[15], },
                        { minimums[8], minimums[8], minimums[9], minimums[9], minimums[10], minimums[10], minimums[11], minimums[11], },
                        { minimums[4], minimums[4], minimums[5], minimums[5], minimums[6], minimums[6], minimums[7], minimums[7], },
                        { minimums[0], minimums[0], minimums[1], minimums[1], minimums[2], minimums[2], minimums[3], minimums[3], },
                    };
                    min = minis;

                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[28], maximums[28], maximums[29], maximums[29], maximums[30], maximums[30], maximums[31], maximums[31], },
                        { maximums[24], maximums[24], maximums[25], maximums[25], maximums[26], maximums[26], maximums[27], maximums[27], },
                        { maximums[20], maximums[20], maximums[21], maximums[21], maximums[22], maximums[22], maximums[23], maximums[23], },
                        { maximums[16], maximums[16], maximums[17], maximums[17], maximums[18], maximums[18], maximums[19], maximums[19], },
                        { maximums[12], maximums[12], maximums[13], maximums[13], maximums[14], maximums[14], maximums[15], maximums[15], },
                        { maximums[8], maximums[8], maximums[9], maximums[9], maximums[10], maximums[10], maximums[11], maximums[11], },
                        { maximums[4], maximums[4], maximums[5], maximums[5], maximums[6], maximums[6], maximums[7], maximums[7], },
                        { maximums[0], maximums[0], maximums[1], maximums[1], maximums[2], maximums[2], maximums[3], maximums[3], },
                    };
                    max = maxis;

                }
                else if (res == 63)
                {
                    // 00 07 70 77
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];

                    ImmutableList<List<string>> list = [];
                    for (int i = 0; i < 78; i++)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 8 || testfor == 9) i++;
                        //Console.WriteLine("Mode31 Read " + i);
                        List<string> temp = new(scom2.multiCommand($"QSTREAM {i}",data_count));
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
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

                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[28], average[28], average[29], average[29], average[30], average[30], average[31], average[31], },
                        { average[24], average[24], average[25], average[25], average[26], average[26], average[27], average[27], },
                        { average[20], average[20], average[21], average[21], average[22], average[22], average[23], average[23], },
                        { average[16], average[16], average[17], average[17], average[18], average[18], average[19], average[19], },
                        { average[12], average[12], average[13], average[13], average[14], average[14], average[15], average[15], },
                        { average[8], average[8], average[9], average[9], average[10], average[10], average[11], average[11], },
                        { average[4], average[4], average[5], average[5], average[6], average[6], average[7], average[7], },
                        { average[0], average[0], average[1], average[1], average[2], average[2], average[3], average[3], },
                    };
                    av = avg;

                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[28], minimums[28], minimums[29], minimums[29], minimums[30], minimums[30], minimums[31], minimums[31], },
                        { minimums[24], minimums[24], minimums[25], minimums[25], minimums[26], minimums[26], minimums[27], minimums[27], },
                        { minimums[20], minimums[20], minimums[21], minimums[21], minimums[22], minimums[22], minimums[23], minimums[23], },
                        { minimums[16], minimums[16], minimums[17], minimums[17], minimums[18], minimums[18], minimums[19], minimums[19], },
                        { minimums[12], minimums[12], minimums[13], minimums[13], minimums[14], minimums[14], minimums[15], minimums[15], },
                        { minimums[8], minimums[8], minimums[9], minimums[9], minimums[10], minimums[10], minimums[11], minimums[11], },
                        { minimums[4], minimums[4], minimums[5], minimums[5], minimums[6], minimums[6], minimums[7], minimums[7], },
                        { minimums[0], minimums[0], minimums[1], minimums[1], minimums[2], minimums[2], minimums[3], minimums[3], },
                    };
                    min = minis;

                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[28], maximums[28], maximums[29], maximums[29], maximums[30], maximums[30], maximums[31], maximums[31], },
                        { maximums[24], maximums[24], maximums[25], maximums[25], maximums[26], maximums[26], maximums[27], maximums[27], },
                        { maximums[20], maximums[20], maximums[21], maximums[21], maximums[22], maximums[22], maximums[23], maximums[23], },
                        { maximums[16], maximums[16], maximums[17], maximums[17], maximums[18], maximums[18], maximums[19], maximums[19], },
                        { maximums[12], maximums[12], maximums[13], maximums[13], maximums[14], maximums[14], maximums[15], maximums[15], },
                        { maximums[8], maximums[8], maximums[9], maximums[9], maximums[10], maximums[10], maximums[11], maximums[11], },
                        { maximums[4], maximums[4], maximums[5], maximums[5], maximums[6], maximums[6], maximums[7], maximums[7], },
                        { maximums[0], maximums[0], maximums[1], maximums[1], maximums[2], maximums[2], maximums[3], maximums[3], },
                    };
                    max = maxis;

                }
                else if (res == 131)
                {
                    // 00 07 70 77
                    int[] average = [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];

                    ImmutableList<List<string>> list = [];
                    for (int i = 1; i < 77; i += 2)
                    {
                        int testfor = i;
                        for (; testfor > 9; testfor -= 10) ;
                        if (testfor == 9) i += 2;
                        //Console.WriteLine("Mode31 Read " + i);
                        List<string> temp = [];
                        //foreach (string s in temp) Console.WriteLine(s);
                        list = list.Add(temp);
                    }

                    List<List<int>> toint = [];
                    int j = 0;
                    foreach (List<string> content in list)
                    {
                        List<int> temp_int = [];
                        foreach (string member in content)
                        {
                            Int32.TryParse(member,out int itemint);
                            temp_int.Add(itemint);
                            average[j] += itemint;
                        }
                        average[j] /= temp_int.Count;
                        temp_int.Sort();
                        toint.Add(temp_int);
                        j++;
                    }

                    //toint.Sort();

                    int[,] avg =
                    {
                        { average[28], average[28], average[29], average[29], average[30], average[30], average[31], average[31], },
                        { average[24], average[24], average[25], average[25], average[26], average[26], average[27], average[27], },
                        { average[20], average[20], average[21], average[21], average[22], average[22], average[23], average[23], },
                        { average[16], average[16], average[17], average[17], average[18], average[18], average[19], average[19], },
                        { average[12], average[12], average[13], average[13], average[14], average[14], average[15], average[15], },
                        { average[8], average[8], average[9], average[9], average[10], average[10], average[11], average[11], },
                        { average[4], average[4], average[5], average[5], average[6], average[6], average[7], average[7], },
                        { average[0], average[0], average[1], average[1], average[2], average[2], average[3], average[3], },
                    };
                    av = avg;

                    List<int> minimums = [];

                    foreach (List<int> a in toint)
                    {
                        minimums.Add(a.First());
                    }

                    int[,] minis =
                    {
                        { minimums[28], minimums[28], minimums[29], minimums[29], minimums[30], minimums[30], minimums[31], minimums[31], },
                        { minimums[24], minimums[24], minimums[25], minimums[25], minimums[26], minimums[26], minimums[27], minimums[27], },
                        { minimums[20], minimums[20], minimums[21], minimums[21], minimums[22], minimums[22], minimums[23], minimums[23], },
                        { minimums[16], minimums[16], minimums[17], minimums[17], minimums[18], minimums[18], minimums[19], minimums[19], },
                        { minimums[12], minimums[12], minimums[13], minimums[13], minimums[14], minimums[14], minimums[15], minimums[15], },
                        { minimums[8], minimums[8], minimums[9], minimums[9], minimums[10], minimums[10], minimums[11], minimums[11], },
                        { minimums[4], minimums[4], minimums[5], minimums[5], minimums[6], minimums[6], minimums[7], minimums[7], },
                        { minimums[0], minimums[0], minimums[1], minimums[1], minimums[2], minimums[2], minimums[3], minimums[3], },
                    };
                    min = minis;

                    List<int> maximums = [];

                    foreach (List<int> a in toint)
                    {
                        maximums.Add(a.Last());
                    }

                    int[,] maxis =
                    {
                        { maximums[28], maximums[28], maximums[29], maximums[29], maximums[30], maximums[30], maximums[31], maximums[31], },
                        { maximums[24], maximums[24], maximums[25], maximums[25], maximums[26], maximums[26], maximums[27], maximums[27], },
                        { maximums[20], maximums[20], maximums[21], maximums[21], maximums[22], maximums[22], maximums[23], maximums[23], },
                        { maximums[16], maximums[16], maximums[17], maximums[17], maximums[18], maximums[18], maximums[19], maximums[19], },
                        { maximums[12], maximums[12], maximums[13], maximums[13], maximums[14], maximums[14], maximums[15], maximums[15], },
                        { maximums[8], maximums[8], maximums[9], maximums[9], maximums[10], maximums[10], maximums[11], maximums[11], },
                        { maximums[4], maximums[4], maximums[5], maximums[5], maximums[6], maximums[6], maximums[7], maximums[7], },
                        { maximums[0], maximums[0], maximums[1], maximums[1], maximums[2], maximums[2], maximums[3], maximums[3], },
                    };
                    max = maxis;

                }
            }

            public static byte[] detector_fields = [00,10,20,30,40,01,06,07,17,27,37,47];

            public static void calib_pieces(byte data_count)
            {
                //Bauer weiß Auf 00
                Console.WriteLine("Place all Pieces on the desired fields. Software will automatically identify after the default chess configuration.\nPress [ENTER] to start the identification");
                while (ConsoleKey.Enter != Console.ReadKey().Key) { }
                int i = 0;
                foreach(byte a in detector_fields)
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
                            Console.WriteLine(back + " " + front + " " + database.physical.av[front,display.inverter[back]]);
                            itemint -= database.physical.av[front,display.inverter[back]];
                            toint.Add(itemint);
                            avg += itemint;
                        }
                    }

                    toint.Sort();
                    avg /= toint.Count;
                    piece_min[i] = toint.First();
                    piece_max[i] = toint.Last();
                    piece_av[i] = avg;
                    i++;
                }
                

                foreach(int element in piece_av)
                {
                    Console.WriteLine(element);
                }
                
            }
            //FIGUREN
            public static int[] piece_av = [0,0,0,0,0,0,0,0,0,0,0,0];
            public static int[] piece_min = [0,0,0,0,0,0,0,0,0,0,0,0];
            public static int[] piece_max = [0,0,0,0,0,0,0,0,0,0,0,0];
            public static byte[] piece_order = [111,115,113,109,110,101,11,15,13,09,10,01];

        }
    }
}
