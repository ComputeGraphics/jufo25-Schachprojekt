using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChessCORE
{
    internal class Renderer
    {
        public static byte standard_direction = 0;
        public static bool standard_empty = true;

        public static List<string> static_log = ["","","","","","","","","","","","","","","","",];
        /*
        UNICODE FORMTATTIERUNGSZEICHEN:

        Empfohlene Fonts: MS Gothic, MS PGothic, MS UI Gothic, Segoe UI Symbol

        U+250x 	─ 	━ 	│ 	┃ 	┄ 	┅ 	┆ 	┇ 	┈ 	┉ 	┊ 	┋ 	┌ 	┍ 	┎ 	┏
        U+251x 	┐ 	┑ 	┒ 	┓ 	└ 	┕ 	┖ 	┗ 	┘ 	┙ 	┚ 	┛ 	├ 	┝ 	┞ 	┟
        U+252x 	┠ 	┡ 	┢ 	┣ 	┤ 	┥ 	┦ 	┧ 	┨ 	┩ 	┪ 	┫ 	┬ 	┭ 	┮ 	┯
        U+253x 	┰ 	┱ 	┲ 	┳ 	┴ 	┵ 	┶ 	┷ 	┸ 	┹ 	┺ 	┻ 	┼ 	┽ 	┾ 	┿
        U+254x 	╀ 	╁ 	╂ 	╃ 	╄ 	╅ 	╆ 	╇ 	╈ 	╉ 	╊ 	╋ 	╌ 	╍ 	╎ 	╏
        U+255x 	═ 	║ 	╒ 	╓ 	╔ 	╕ 	╖ 	╗ 	╘ 	╙ 	╚ 	╛ 	╜ 	╝ 	╞ 	╟
        U+256x 	╠ 	╡ 	╢ 	╣ 	╤ 	╥ 	╦ 	╧ 	╨ 	╩ 	╪ 	╫ 	╬ 	╭ 	╮ 	╯
        U+257x 	╰ 	╱ 	╲ 	╳ 	╴ 	╵ 	╶ 	╷ 	╸ 	╹ 	╺ 	╻ 	╼ 	╽ 	╾ 	╿ 
        */
        public static bool draw_demo()
        {
            try
            {
                Console.Clear();
                Console.Title = "ChessCORE Visual(EX)";
                Console.WriteLine("Reconfiguring Interface");
                Console.OutputEncoding = Encoding.UTF8;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Black: ♔ ♕ ♖ ♗ ♘ ♙   White:♚ ♛ ♜ ♝ ♞ ♟");
                Console.WriteLine();
                Console.WriteLine("Sample Board (No Renderer):");
                Console.WriteLine("    A    B    C    D    E    F    G    H");
                Console.WriteLine("  ┏━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┓");
                Console.WriteLine("8 ┃ ♖  │ ♘  │ ♗  │ ♕  │ ♔  │ ♗  │ ♘  │ ♖  ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("7 ┃ ♙  │ ♙  │ ♙  │ ♙  │ ♙  │ ♙  │ ♙  │ ♙  ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("6 ┃    │    │    │    │    │    │    │    ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("5 ┃    │    │    │    │    │    │    │    ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("4 ┃    │    │    │    │    │    │    │    ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("3 ┃    │    │    │    │    │    │    │    ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("2 ┃ ♟  │ ♟  │ ♟  │ ♟  │ ♟  │ ♟  │ ♟  │ ♟  ┃");
                Console.WriteLine("  ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine("1 ┃ ♜  │ ♞  │ ♝  │ ♛  │ ♚  │ ♝  │ ♞  │ ♜  ┃");
                Console.WriteLine("  ┗━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┛");
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static byte change_counter = 0;
        public static void draw(bool loop_refresh,bool empty = false,byte direction = 0,bool data_mode = false,bool show_codes = false)
        {
            Storage.log("Starting Renderer...");
            bool temp_refresh = true;

            string[,] disp_board =
            {
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
            };

            if (!empty)
            {
                Database.Display.writeSample();
            }


            ////////////////////// SET BOARD DIRECTION //////////////////////
            char[] dirx = ['A','B','C','D','E','F','G','H'];
            //char[] diry = { '1','2','3','4','5','6','7','8' };
            char[] diry = ['8','7','6','5','4','3','2','1'];

            switch (direction)
            {
                case 9:
                    char[] tx = ['1','2','3','4','5','6','7','8'];
                    char[] ty = ['A','B','C','D','E','F','G','H'];
                    dirx = tx; diry = ty;
                    break;
                case 18:
                    ty = ['1','2','3','4','5','6','7','8'];
                    tx = ['H','G','F','E','D','C','B','A'];
                    dirx = tx; diry = ty;
                    break;
                case 27:
                    tx = ['8','7','6','5','4','3','2','1'];
                    ty = ['H','G','F','E','D','C','B','A'];
                    dirx = tx; diry = ty;
                    break;
                default:
                    break;
            }

            int iterator = 0;
            Storage.log("Renderer Initalized. Starting Rendering...");

            while (temp_refresh && (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape))
            {
                iterator++;
                //////////////////// REQUEST DATA FROM BOARD ////////////////////
                if (data_mode) Thread.Sleep(500);
                if (!data_mode) board_visual.requestAll_rangeMode();
                Console.WriteLine("PROCESSING FINISHED " + iterator);

                //////////////////////// TRANSLATE ICONS ////////////////////////
                int y = 0;
                if (show_codes)
                {
                    for (int x = 0; y < 8; x++)
                    {
                        disp_board[y,x] = Database.Display.field[y,x].ToString();
                        if (x == 7)
                        {
                            y++;
                            x = -1;
                        }
                    }
                }
                else
                {
                    for (int x = 0; y < 8; x++)
                    {
                        disp_board[y,x] = Database.Display.translator[Database.Display.field[y,x]];
                        if (x == 7)
                        {
                            y++;
                            x = -1;
                        }
                    }
                }



                ////////////////////// DRAW THE CHESSBOARD //////////////////////
                Console.Clear();
                Console.Title = "ChessCORE Visual(EX)";
                //Console.WriteLine("Reconfiguring Interface");
                Console.OutputEncoding = Encoding.UTF8;

                //Console.WriteLine("Black: ♔ ♕ ♖ ♗ ♘ ♙   White:♚ ♛ ♜ ♝ ♞ ♟");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine(DateTime.Now.ToString("HH:mm").PadLeft(Console.WindowWidth / 2) + "".PadRight(Console.WindowWidth / 2));

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n");
                Console.WriteLine($"      {dirx[0]}    {dirx[1]}    {dirx[2]}    {dirx[3]}    {dirx[4]}    {dirx[5]}    {dirx[6]}    {dirx[7]}            LOG:");
                Console.WriteLine("    ┏━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┓            ");
                Console.WriteLine($"  {diry[0]} ┃ {disp_board[0,0]} │ {disp_board[0,1]} │ {disp_board[0,2]} │ {disp_board[0,3]} │ {disp_board[0,4]} │ {disp_board[0,5]} │ {disp_board[0,6]} │ {disp_board[0,7]} ┃            {static_log[^15]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^14]);
                Console.WriteLine($"  {diry[1]} ┃ {disp_board[1,0]} │ {disp_board[1,1]} │ {disp_board[1,2]} │ {disp_board[1,3]} │ {disp_board[1,4]} │ {disp_board[1,5]} │ {disp_board[1,6]} │ {disp_board[1,7]} ┃            {static_log[^13]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^12]);
                Console.WriteLine($"  {diry[2]} ┃ {disp_board[2,0]} │ {disp_board[2,1]} │ {disp_board[2,2]} │ {disp_board[2,3]} │ {disp_board[2,4]} │ {disp_board[2,5]} │ {disp_board[2,6]} │ {disp_board[2,7]} ┃            {static_log[^11]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^10]);
                Console.WriteLine($"  {diry[3]} ┃ {disp_board[3,0]} │ {disp_board[3,1]} │ {disp_board[3,2]} │ {disp_board[3,3]} │ {disp_board[3,4]} │ {disp_board[3,5]} │ {disp_board[3,6]} │ {disp_board[3,7]} ┃            {static_log[^9]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^8]);
                Console.WriteLine($"  {diry[4]} ┃ {disp_board[4,0]} │ {disp_board[4,1]} │ {disp_board[4,2]} │ {disp_board[4,3]} │ {disp_board[4,4]} │ {disp_board[4,5]} │ {disp_board[4,6]} │ {disp_board[4,7]} ┃            {static_log[^7]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^6]);
                Console.WriteLine($"  {diry[5]} ┃ {disp_board[5,0]} │ {disp_board[5,1]} │ {disp_board[5,2]} │ {disp_board[5,3]} │ {disp_board[5,4]} │ {disp_board[5,5]} │ {disp_board[5,6]} │ {disp_board[5,7]} ┃            {static_log[^5]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^4]);
                Console.WriteLine($"  {diry[6]} ┃ {disp_board[6,0]} │ {disp_board[6,1]} │ {disp_board[6,2]} │ {disp_board[6,3]} │ {disp_board[6,4]} │ {disp_board[6,5]} │ {disp_board[6,6]} │ {disp_board[6,7]} ┃            {static_log[^3]}");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨            " + static_log[^2]);
                Console.WriteLine($"  {diry[7]} ┃ {disp_board[7,0]} │ {disp_board[7,1]} │ {disp_board[7,2]} │ {disp_board[7,3]} │ {disp_board[7,4]} │ {disp_board[7,5]} │ {disp_board[7,6]} │ {disp_board[7,7]} ┃            {static_log[^1]}");
                Console.WriteLine("    ┗━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┛");
                Console.WriteLine("\n");
                if (data_mode == false) Console.WriteLine("REDRAW COUNT " + iterator); else Console.WriteLine("CURRENT SNAP " + OpenGame.current_snap);


                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                if (OperatingSystem.IsLinux())
                {
                    if (data_mode)
                    {
                        Console.WriteLine(" (ESC)   (F10)   (XX)".PadRight(Console.WindowWidth - 35) + "(F1)   (F2)   (F3)   (F4)   (F12) ");
                        Console.Write("   ⇱      🖴       ↻".PadRight(Console.WindowWidth - 33) + "⏪     ⏯️      ⏩     ⏹️      ⏺️   ");
                    }
                    else
                    {
                        Console.WriteLine(" (ESC)   (F10)   (F5)".PadRight(Console.WindowWidth - 35) + "(XX)   (F2)   (XX)   (F4)   (F12) ");
                        Console.Write("   ⇱      🖴       ↻".PadRight(Console.WindowWidth - 33) + "⏪     ⏯️      ⏩     ⏹️      ⏺️   ");
                    }
                }
                else
                {
                    if (data_mode)
                    {
                        Console.WriteLine(" (ESC)   (F10)   (XX)".PadRight(Console.WindowWidth - 35) + "(F1)   (F2)   (F3)   (F4)   (F12)  ");
                        Console.Write("   ⇱      🖴       ↻".PadRight(Console.WindowWidth - 33) + "⏪     ⏯️     ⏩     ⏹️     ⏺️    ");
                    }
                    else
                    {
                        Console.WriteLine(" (ESC)   (F10)   (F5)".PadRight(Console.WindowWidth - 35) + "(XX)   (F2)   (XX)   (F4)   (F12)  ");
                        Console.Write("   ⇱      🖴       ↻".PadRight(Console.WindowWidth - 33) + "⏪     ⏯️     ⏩     ⏹️     ⏺️    ");
                    }
                }

                Console.ResetColor();

                ////////////////////// RECALIB IF INTERRUPT //////////////////////
                ///

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.F5)
                    {
                        Console.WriteLine("DYN-RECALIB");
                        Database.Physical.DynRecalib();
                    }
                    else if (key == ConsoleKey.F10)
                    {
                        Console.WriteLine("CACHING FIELD");
                        Storage.cacheVisualBoard(Database.Display.field,DateTime.Now.ToString("dd-MM-yy HH-mm") + " Cache");
                    }
                    else if (key == ConsoleKey.F1 && data_mode)
                    {
                        if (OpenGame.current_snap > 0) OpenGame.current_snap--;
                        board_visual.showGame(OpenGame.filename,OpenGame.current_snap);
                    }
                    else if (key == ConsoleKey.F3 && data_mode)
                    {
                        int[] all_snaps = [];
                        if (OpenGame.filename != "") all_snaps = Storage.snapCount(OpenGame.filename);
                        if (OpenGame.current_snap + 1 < all_snaps.Length) OpenGame.current_snap++;
                        board_visual.showGame(OpenGame.filename,all_snaps[OpenGame.current_snap]);
                    }
                    else if (key == ConsoleKey.F4)
                    {
                        Storage.finishGame("test");
                    }
                    else if (key == ConsoleKey.F12)
                    {
                        Storage.createGame();
                    }
                    else if (key == ConsoleKey.F2)
                    {
                        Storage.saveGameSnap(Database.Display.field);
                    }
                    //break;
                }

                if (!data_mode)
                {
                    if (change_counter < 1)
                    {
                        System.Diagnostics.Debug.WriteLine("Increase Changer");
                        Database.Display.buffer = Database.Display.field;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Testing for Changes");

                        bool equal =
        Database.Display.buffer.Rank == Database.Display.field.Rank &&
        Enumerable.Range(0,Database.Display.buffer.Rank).All(dimension => Database.Display.buffer.GetLength(dimension) == Database.Display.field.GetLength(dimension)) &&
        Database.Display.buffer.Cast<Byte>().SequenceEqual(Database.Display.field.Cast<Byte>());

                        if (equal)
                        {
                            System.Diagnostics.Debug.WriteLine("Change Detected");
                            if (OpenGame.open) Storage.saveGameSnap(Database.Display.field);
                        }
                    }


                    change_counter++;
                    if (change_counter == 2) change_counter = 0;
                }
                temp_refresh = loop_refresh;
                if (!loop_refresh) while (Console.ReadKey().Key != ConsoleKey.Escape) { }

            }


            Storage.log("Dispose Environment and Quit to Main Menu");
            scom2.Dispose();
            Init.MainMenu();
        }


        public static void draw_number(bool loop_refresh,bool empty,byte direction)
        {
            Storage.log("Debug Number Mode");
            bool temp_refresh = true;
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

            if (!empty)
            {
                Database.Display.writeSample();
            }


            ////////////////////// SET BOARD DIRECTION //////////////////////

            char[] dirx = ['A','B','C','D','E','F','G','H'];
            //char[] diry = [ '1','2','3','4','5','6','7','8' ];
            char[] diry = ['8','7','6','5','4','3','2','1'];

            switch (direction)
            {
                case 9:
                    char[] tx = ['1','2','3','4','5','6','7','8'];
                    char[] ty = ['A','B','C','D','E','F','G','H'];
                    dirx = tx; diry = ty;
                    break;
                case 18:
                    ty = ['1','2','3','4','5','6','7','8'];
                    tx = ['H','G','F','E','D','C','B','A'];
                    dirx = tx; diry = ty;
                    break;
                case 27:
                    tx = ['8','7','6','5','4','3','2','1'];
                    ty = ['H','G','F','E','D','C','B','A'];
                    dirx = tx; diry = ty;
                    break;
                default:
                    break;
            }


            int iterator = 0;
            while (temp_refresh && (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape))
            {

                iterator++;
                Console.WriteLine("REDRAW COUNT " + iterator);
                //////////////////// REQUEST DATA FROM BOARD ////////////////////
                //Thread.Sleep(500);
                disp_board = board_visual.requestAll_rangeMode();
                Console.WriteLine("PROCESSING FINISHED " + iterator);

                ////////////////////// DRAW THE CHESSBOARD //////////////////////
                Console.Clear();
                Console.OutputEncoding = Encoding.UTF8;



                //Console.WriteLine("Black: ♔ ♕ ♖ ♗ ♘ ♙   White:♚ ♛ ♜ ♝ ♞ ♟");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine(DateTime.Now.ToString("HH:mm").PadLeft(Console.WindowWidth / 2) + "".PadRight(Console.WindowWidth / 2));

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n");
                Console.WriteLine($"      {dirx[0]}    {dirx[1]}    {dirx[2]}    {dirx[3]}    {dirx[4]}    {dirx[5]}    {dirx[6]}    {dirx[7]}");
                Console.WriteLine("    ┏━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┯━━━━┓");
                Console.WriteLine($"  {diry[0]} ┃ {disp_board[0,0]} │ {disp_board[0,1]} │ {disp_board[0,2]} │ {disp_board[0,3]} │ {disp_board[0,4]} │ {disp_board[0,5]} │ {disp_board[0,6]} │ {disp_board[0,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[1]} ┃ {disp_board[1,0]} │ {disp_board[1,1]} │ {disp_board[1,2]} │ {disp_board[1,3]} │ {disp_board[1,4]} │ {disp_board[1,5]} │ {disp_board[1,6]} │ {disp_board[1,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[2]} ┃ {disp_board[2,0]} │ {disp_board[2,1]} │ {disp_board[2,2]} │ {disp_board[2,3]} │ {disp_board[2,4]} │ {disp_board[2,5]} │ {disp_board[2,6]} │ {disp_board[2,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[3]} ┃ {disp_board[3,0]} │ {disp_board[3,1]} │ {disp_board[3,2]} │ {disp_board[3,3]} │ {disp_board[3,4]} │ {disp_board[3,5]} │ {disp_board[3,6]} │ {disp_board[3,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[4]} ┃ {disp_board[4,0]} │ {disp_board[4,1]} │ {disp_board[4,2]} │ {disp_board[4,3]} │ {disp_board[4,4]} │ {disp_board[4,5]} │ {disp_board[4,6]} │ {disp_board[4,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[5]} ┃ {disp_board[5,0]} │ {disp_board[5,1]} │ {disp_board[5,2]} │ {disp_board[5,3]} │ {disp_board[5,4]} │ {disp_board[5,5]} │ {disp_board[5,6]} │ {disp_board[5,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[6]} ┃ {disp_board[6,0]} │ {disp_board[6,1]} │ {disp_board[6,2]} │ {disp_board[6,3]} │ {disp_board[6,4]} │ {disp_board[6,5]} │ {disp_board[6,6]} │ {disp_board[6,7]} ┃");
                Console.WriteLine("    ┠────┼────┼────┼────┼────┼────┼────┼────┨");
                Console.WriteLine($"  {diry[7]} ┃ {disp_board[7,0]} │ {disp_board[7,1]} │ {disp_board[7,2]} │ {disp_board[7,3]} │ {disp_board[7,4]} │ {disp_board[7,5]} │ {disp_board[7,6]} │ {disp_board[7,7]} ┃");
                Console.WriteLine("    ┗━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┷━━━━┛");

                ////////////////////// RECALIB IF INTERRUPT //////////////////////
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.F5)
                    {
                        Console.WriteLine("DYN-RECALIB");
                        Database.Physical.DynRecalib();
                    }
                    else if (Console.ReadKey(true).Key == ConsoleKey.F12)
                    {
                        Console.WriteLine("CACHING FIELD");
                        Storage.cacheVisualBoard(Database.Display.field,DateTime.Now.ToString("dd-MM-yy HH-mm") + " Cache");
                    }
                }


                if (!loop_refresh) temp_refresh = false;
            }
            scom2.Dispose();
            Init.MainMenu();
        }


    }
}
