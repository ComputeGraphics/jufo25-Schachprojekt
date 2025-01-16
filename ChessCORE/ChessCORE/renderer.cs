using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ChessCORE
{
    internal class Renderer
    {
        public static byte standard_direction = 0;
        public static bool standard_empty = true;
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

        public static void draw(bool loop_refresh,bool empty,byte direction)
        {
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
                database.display.writeSample();
            }

            ////////////////////// SET BOARD DIRECTION //////////////////////
            char[] dirx = [ 'A','B','C','D','E','F','G','H' ];
            //char[] diry = { '1','2','3','4','5','6','7','8' };
            char[] diry = [ '8','7','6','5','4','3','2','1' ];

            switch (direction)
            {
                case 9:
                    char[] tx = { '1','2','3','4','5','6','7','8' };
                    char[] ty = { 'A','B','C','D','E','F','G','H' };
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

            while (temp_refresh)
            {
                iterator++;
                //////////////////// REQUEST DATA FROM BOARD ////////////////////
                //Thread.Sleep(500);
                board_visual.requestAll_rangeMode();
                Console.WriteLine("PROCESSING FINISHED " + iterator);

                //////////////////////// TRANSLATE ICONS ////////////////////////
                int y = 0;
                for (int x = 0; y < 8; x++)
                {
                    disp_board[y,x] = database.display.translator[database.display.field[y,x]];
                    if (x == 7)
                    {
                        y++;
                        x = -1;
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
                Console.WriteLine(DateTime.Now.ToString("HH:mm").PadLeft(Console.WindowWidth / 2 - 2) + "".PadRight(Console.WindowWidth / 2 + 2));

                Console.BackgroundColor = ConsoleColor.Black;
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
                Console.WriteLine("\n");
                Console.WriteLine("REDRAW COUNT " + iterator);

                ////////////////////// REBOOT IF INTERRUPT //////////////////////
                /*if (ConsoleKey.F5 != Console.ReadKey().Key)
                {
                    Console.WriteLine("REBOOTING...");
                    scom2.sendCommand("test");
                    break;
                }*/

                temp_refresh = loop_refresh;
            }
        }


        public static void draw_number(bool loop_refresh,bool empty,byte direction)
        {
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
                database.display.writeSample();
            }


            ////////////////////// SET BOARD DIRECTION //////////////////////

            char[] dirx = ['A','B','C','D','E','F','G','H'];
            //char[] diry = [ '1','2','3','4','5','6','7','8' ];
            char[] diry = ['8','7','6','5','4','3','2','1'];

            switch (direction)
            {
                case 9:
                    char[] tx = { '1','2','3','4','5','6','7','8' };
                    char[] ty = { 'A','B','C','D','E','F','G','H' };
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
            while (temp_refresh)
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
                Console.WriteLine(DateTime.Now.ToString("HH:mm").PadLeft(Console.WindowWidth / 2 - 2) + "".PadRight(Console.WindowWidth / 2 + 2));


                Console.BackgroundColor = ConsoleColor.Black;
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

                ////////////////////// REBOOT IF INTERRUPT //////////////////////

                if (!loop_refresh) temp_refresh = false;
            }
        }


    }
}
