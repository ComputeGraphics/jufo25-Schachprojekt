// See https://aka.ms/new-console-template for more information
//cd /mnt/Windows/Users/Arthur/Documents/GitHub/jufo25-Schachprojekt/ChessCORE/ChessCORE/bin/Debug/net8.0/

using System.Dynamic;

namespace ChessCORE
{
    internal class Init
    {
        public static int transferindex = 0;
        public static List<Option> options = [];

        public static void Main()
        {
            Storage.start();
            if(scom2.esp32) scom2.set_esp32(scom2.esp32);
            MainMenu();
        }
        public static void MainMenu()
        {
            transferindex = 0;
            Console.ResetColor();
            Console.Clear();
            Active.title = "ChessCORE";
            Active.background = ConsoleColor.White;
            Active.foreground = ConsoleColor.Black;

            options =
            [
                new("Start Strap",() => board_visual.show()),
                new("Serial Writer",scom2.UI),
                new("Settings Menu",() => SettingsMenu(0)),
                new("Storage Manager",Storages),
                new("Leave",() => Environment.Exit(0)),
            ];
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);


            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }
        // Default action of all the options. You can create more methods
        public static void Storages()
        {
            transferindex = 0;
            Console.ResetColor();
            Console.Clear();
            Active.title = "Storage Manager";
            Active.background = ConsoleColor.Yellow;
            Active.foreground = ConsoleColor.White;

            options =
            [
                new("Saved Games",StorageSavedGames),
                new("Saved Snaps",StoragesSavedSnaps),
                new("Cached Games",() => WriteTemporaryMessage("Not Implemented")),
                new("Cached Snaps",StoragesCachedSnaps),
                new("Last Game",() => WriteTemporaryMessage("Not Implemented")),
                new("Back",MainMenu),
            ];
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }

        public static void StoragesCachedSnaps()
        {
            transferindex = 0;
            Console.ResetColor();
            Console.Clear();
            Active.title = "Storage Manager - Cached Snaps";
            Active.background = ConsoleColor.Yellow;
            Active.foreground = ConsoleColor.White;

            options.Clear();
            string[] files = Storage.GetFiles("cache");
            foreach (string file in files)
            {
                options.Add(new(file,() => board_visual.showSnap(file)));
            }
            options.Add(new("Back",Storages));
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }

        public static void StorageSavedGames()
        {
            transferindex = 0;
            Console.ResetColor();
            Console.Clear();
            Active.title = "Storage Manager - Saved Games";
            Active.background = ConsoleColor.Yellow;
            Active.foreground = ConsoleColor.White;

            options.Clear();
            string[] files = Storage.GetFiles("saves/games");
            foreach (string file in files)
            {
                options.Add(new(file,() => board_visual.showGame(file,0)));
            }
            options.Add(new("Back",Storages));
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }

        public static void StoragesSavedSnaps()
        {
            transferindex = 0;
            Console.ResetColor();
            Console.Clear();
            Active.title = "Storage Manager - Saved Snaps";
            Active.background = ConsoleColor.Yellow;
            Active.foreground = ConsoleColor.White;

            options.Clear();
            string[] files = Storage.GetFiles("saves/snaps");
            foreach (string file in files)
            {
                options.Add(new(file,() => board_visual.showSnap(file)));
            }
            options.Add(new("Back",Storages));
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();

        }



        static void WriteTemporaryMessage(string message)
        {
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(3000);
            WriteMenu(Active.title,Active.background,Active.foreground,options,options[0]);
        }
        static void WriteMenu(string title,ConsoleColor background,ConsoleColor foreground,List<Option> options,Option selectedOption)
        {
            Console.Clear();
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(title.PadRight(Console.WindowWidth) + "\n");

            foreach (Option option in options)
            {
                string full;
                Console.ResetColor();
                if (option == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    full = "> ";
                }
                else
                {
                    full = "  ";
                }

                full += option.Name;
                Console.WriteLine(full.PadRight(Console.WindowWidth));
            }
        }
        public static void SettingsMenu(int start)
        {
            int av = Database.Physical.default_av;
            transferindex = start;
            List<int> standard_bauds = [0,4800,9600,19200,38400,57600,74880,115200];
            List<byte> standard_counts = [1,2,4,8,16,32,64,128];
            List<byte> standard_directions = [0,9,18,27];
            List<byte> standard_resolutions = [0,3,7,31,63,131];
            List<byte> standard_tolerance = [0,1,2,4,8,16,32,64,96];
            List<int> caav = [av - 5,av - 4,av - 3,av - 2,av - 1,av,av + 1,av + 2,av + 3,av + 4,av + 5];
            Console.Clear();
            Console.ResetColor();
            options =
            [
                new($"(SERIAL)   AUTO IDENT                            {GetState(scom2.auto_ident)}",() => updateVar(0,scom2.auto_ident,out scom2.auto_ident),() => updateVar(0,scom2.auto_ident,out scom2.auto_ident),() => updateVar(0,scom2.auto_ident,out scom2.auto_ident)),
                new($"(SERIAL)   DEFAULT BAUD                          {scom2.default_baud}",() => WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"),() => changer(1,true,standard_bauds.IndexOf(scom2.default_baud),standard_bauds,out scom2.default_baud),() => changer(1,false,standard_bauds.IndexOf(scom2.default_baud),standard_bauds,out scom2.default_baud)),
                new($"(SERIAL)   ALLOW WIN32_SERIAL                    {GetState(scom2.allow_win32)}",() => updateVar(2,scom2.allow_win32,out scom2.allow_win32),() => updateVar(2,scom2.allow_win32,out scom2.allow_win32),() => updateVar(2,scom2.allow_win32,out scom2.allow_win32)),
                new($"(SERIAL)   ADVANCED SERIAL                       {GetState(scom2.advanced)}",() => updateVar(3,scom2.advanced,out scom2.advanced),() => updateVar(3,scom2.advanced,out scom2.advanced),() => updateVar(3,scom2.advanced,out scom2.advanced)),
                new($"(SERIAL)   DEFAULT PACKAGE LENGTH                {scom2.default_count}",() => WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"),() => changer(4,true,standard_counts.IndexOf(scom2.default_count),standard_counts,out scom2.default_count),() => changer(4,false,standard_counts.IndexOf(scom2.default_count),standard_counts,out scom2.default_count)),
                new($"(SERIAL)   AWAIT READY-SIGNAL                    {GetState(scom2.wait_ready)}",() => updateVar(5,scom2.wait_ready,out scom2.wait_ready),() => updateVar(5,scom2.wait_ready,out scom2.wait_ready),() => updateVar(5,scom2.wait_ready,out scom2.wait_ready)),
                new($"(RENDERER) STANDARD EMPTY                        {GetState(Renderer.standard_empty)}",() => updateVar(6,Renderer.standard_empty,out Renderer.standard_empty),() => updateVar(6,Renderer.standard_empty,out Renderer.standard_empty),() => updateVar(6,Renderer.standard_empty,out Renderer.standard_empty)),
                new($"(RENDERER) DEFAULT DIRECTION                     {Renderer.standard_direction}",() => WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"),() => changer(7,true,standard_directions.IndexOf(Renderer.standard_direction),standard_directions,out Renderer.standard_direction),() => changer(7,false,standard_directions.IndexOf(Renderer.standard_direction),standard_directions,out Renderer.standard_direction)),
                new($"(RENDERER) APPLY PAWN FIX                        {GetState(Database.Display.translator[1] == "♙ ")}",Database.Display.ApplyPawnFix,Database.Display.ApplyPawnFix,Database.Display.ApplyPawnFix),
                new($"(RENDERER) SHOW ICONS                            {GetState(board_visual.show_icons)}",() => updateVar(9,board_visual.show_icons,out board_visual.show_icons),() => updateVar(9,board_visual.show_icons,out board_visual.show_icons),() => updateVar(9,board_visual.show_icons,out board_visual.show_icons)),
                new($"(RENDERER) SHOW MAGNETIC                         {GetState(board_visual.show_magnetic)}",() => updateVar(10,board_visual.show_magnetic,out board_visual.show_magnetic),() => updateVar(10,board_visual.show_magnetic,out board_visual.show_magnetic),() => updateVar(10,board_visual.show_magnetic,out board_visual.show_magnetic)),
                new($"(RENDERER) SHOW LOW-RENDER                       {GetState(board_visual.low_render)}",() => updateVar(11,board_visual.low_render,out board_visual.low_render),() => updateVar(11,board_visual.low_render,out board_visual.low_render),() => updateVar(11,board_visual.low_render,out board_visual.low_render)),
                new($"(PHYSICAL) TOLERANCE                             {Database.Physical.tolerance}",() => WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"),() => changer(12,true,standard_tolerance.IndexOf(Database.Physical.tolerance),standard_tolerance,out Database.Physical.tolerance),() => changer(12,false,standard_tolerance.IndexOf(Database.Physical.tolerance),standard_tolerance,out Database.Physical.tolerance)),
                new($"(PHYSICAL) CALIBRATION                           {GetState(Database.Physical.calib)}",() => updateVar(13,Database.Physical.calib,out Database.Physical.calib),() => updateVar(13,Database.Physical.calib,out Database.Physical.calib),() => updateVar(13,Database.Physical.calib,out Database.Physical.calib)),
                new($"(PHYSICAL) CALIBRATION RESOLUTION                {Database.Physical.default_calib}",() => WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"),() => changer(14,true,standard_resolutions.IndexOf(Database.Physical.default_calib),standard_resolutions,out Database.Physical.default_calib),() => changer(14,false,standard_resolutions.IndexOf(Database.Physical.default_calib),standard_resolutions,out Database.Physical.default_calib)),
                new($"(PHYSICAL) DEFAULT CALIB AV                      {Database.Physical.default_av}",() => enterVar(15,out Database.Physical.default_av),() => changer(15,true,caav.IndexOf(av),caav,out Database.Physical.default_av),() => changer(15,false,caav.IndexOf(av),caav,out Database.Physical.default_av)),
                new($"(PYHSICAL) AUTO 2RE-CALIB                        {GetState(Database.Physical.recalib)}",() => updateVar(16,Database.Physical.recalib,out Database.Physical.recalib),() => updateVar(16,Database.Physical.recalib,out Database.Physical.recalib),() => updateVar(16,Database.Physical.recalib,out Database.Physical.recalib)),
                new($"(PYHSICAL) TOGGLE POST-PROCESSING                {GetState(board_visual.do_post)}",() => updateVar(17,board_visual.do_post,out board_visual.do_post),() => updateVar(17,board_visual.do_post,out board_visual.do_post),() => updateVar(17,board_visual.do_post,out board_visual.do_post)),
                new($"(MCU)      LAUNCH RE-SELFTEST                    {GetState(Database.Physical.repeat_selftest)}",() => updateVar(18,Database.Physical.repeat_selftest,out Database.Physical.repeat_selftest),() => updateVar(18,Database.Physical.repeat_selftest,out Database.Physical.repeat_selftest),() => updateVar(18,Database.Physical.repeat_selftest,out Database.Physical.repeat_selftest)),
                new($"(MCU)      NANO ESP32 MODE                       {GetState(scom2.esp32)}",() => scom2.set_esp32(!scom2.esp32,19),() => scom2.set_esp32(!scom2.esp32,19),() => scom2.set_esp32(!scom2.esp32,19)),
                new($"(MCU)      ANALOG INTERPOLATOR (EX)              {GetState(scom2.mcu_interpolator)}",() => updateVar(20,scom2.mcu_interpolator,out scom2.mcu_interpolator),() => updateVar(20,scom2.mcu_interpolator,out scom2.mcu_interpolator),() => updateVar(20,scom2.mcu_interpolator,out scom2.mcu_interpolator)),
                new($"(MCU)      DEBUG READBACKS                       {GetState(scom2.mcu_rb)}",() => updateVar(21,scom2.mcu_rb,out scom2.mcu_rb),() => updateVar(21,scom2.mcu_rb,out scom2.mcu_rb),() => updateVar(21,scom2.mcu_rb,out scom2.mcu_rb)),

                new("Back to Main Menu",MainMenu),
            ];

            Active.title = "SETTINGS MENU";
            Active.background = ConsoleColor.Blue;
            Active.foreground = ConsoleColor.White;

            WriteMenu(Active.title,Active.background,Active.foreground,options,options[start]);

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (transferindex + 1 < options.Count)
                    {
                        transferindex++;
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (transferindex - 1 >= 0)
                    {
                        transferindex--;
                        Console.ResetColor();
                        WriteMenu(Active.title,Active.background,Active.foreground,options,options[transferindex]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[transferindex].Selected.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.LeftArrow)
                {
                    options[transferindex].Decrease.Invoke();
                    transferindex = 0;
                }

                if (keyinfo.Key == ConsoleKey.RightArrow)
                {
                    options[transferindex].Increase.Invoke();
                    transferindex = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);
        }


        public static void updateVar(int index,bool input,out bool change)
        {
            change = !input;
            SettingsMenu(index);
        }

        public static void enterVar(int index,out int output)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Please enter a new value:");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            string? input = Console.ReadLine();
            if (int.TryParse(input,out int result))
            {
                output = result;
            }
            else
            {
                output = 0;
            }
            SettingsMenu(index);
        }


        public static void changer(int option_index,bool direction,int index,List<int> input,out int output)
        {
            switch (direction)
            {
                case true:
                    if (index + 1 < input.Count)
                    {
                        output = input[index + 1];
                    }
                    else output = input[index];
                    break;
                case false:
                    if (index - 1 >= 0)
                    {
                        output = input[index - 1];
                    }
                    else output = input[index];
                    break;
            }
            SettingsMenu(option_index);
        }

        public static void changer(int option_index,bool direction,int index,List<byte> input,out byte output)
        {
            switch (direction)
            {
                case true:
                    if (index + 1 < input.Count)
                    {
                        output = input[index + 1];
                    }
                    else output = input[index];
                    break;
                case false:
                    if (index - 1 >= 0)
                    {
                        output = input[index - 1];
                    }
                    else output = input[index];
                    break;
            }
            SettingsMenu(option_index);
        }

        public static string GetState(bool state)
        {
            return (state ? "░░██" : "██░░");
        }

    }




    public class Active
    {
        public static string title = "N/A";
        public static string? description;
        public static ConsoleColor background = ConsoleColor.Black;
        public static ConsoleColor foreground = ConsoleColor.White;

    }
    public class Option(string name,Action selected,Action? increase = null,Action? decrease = null)
    {
        public string Name { get; } = name;
        public Action Selected { get; } = selected;

        public Action Decrease { get; } = decrease ?? (() => { });
        public Action Increase { get; } = increase ?? (() => { });
    }
}