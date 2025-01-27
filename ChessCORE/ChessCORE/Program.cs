// See https://aka.ms/new-console-template for more information


using System.Dynamic;

namespace ChessCORE
{
    internal class Init
    {
        public static int transferindex = 0;
        public static List<Option> options = [];
        public static void Main()
        {
            transferindex = 0;
            System.Diagnostics.Debug.WriteLine("ChessCORE started");
            Console.ResetColor();
            Console.Clear();
            Active.title = "ChessCORE";
            Active.background = ConsoleColor.White;
            Active.foreground = ConsoleColor.Black;

            options = new List<Option>
            {
                new("Start Strap", board_visual.show),
                new("Serial Writer", scom.start),
                new("Settings Menu", () => SettingsMenu(0)),
                new("Docs", () =>  WriteTemporaryMessage("placeholder")),
                new("Leave", () => Environment.Exit(0)),
            };
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
        static void WriteTemporaryMessage(string message)
        {
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
                string full = "";
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
            int av = database.physical.default_av;
            transferindex = start;
            List<int> standard_bauds = [0,4800,9600,19200,38400,57600,74880,115200];
            List<byte> standard_counts = [1,2,4,8,16,32,64,128];
            List<byte> standard_directions = [0,9,18,27];
            List<byte> standard_resolutions = [0,3,7,31,63,131];
            List<byte> standard_tolerance = [0,1,2,4,8,16,32,64,96];
            List<int> caav = [av - 5,av - 4,av - 3,av - 2,av - 1,av,av + 1,av + 2,av + 3,av + 4,av + 5];
            Console.Clear();
            Console.ResetColor();
            options = new List<Option>
            {
                new($"(SERIAL)   AUTO IDENT                            {GetState(scom.auto_ident)}", () => updateVar(0, scom.auto_ident, out scom.auto_ident), () => updateVar(0, scom.auto_ident, out scom.auto_ident), () => updateVar(0, scom.auto_ident, out scom.auto_ident)),
                new($"(SERIAL)   DEFAULT BAUD                          {scom.default_baud}", () =>  WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"), () => changer(1, true, standard_bauds.IndexOf(scom.default_baud), standard_bauds, out scom.default_baud),() => changer(1,false, standard_bauds.IndexOf(scom.default_baud), standard_bauds, out scom.default_baud)),
                new($"(SERIAL)   ALLOW WIN32_SERIAL                    {GetState(scom.allow_win32)}", () => updateVar(2, scom.allow_win32, out scom.allow_win32),() => updateVar(2, scom.allow_win32, out scom.allow_win32),() => updateVar(2, scom.allow_win32, out scom.allow_win32)),
                new($"(SERIAL)   ADVANCED SERIAL                       {GetState(scom.advanced)}", () => updateVar(3, scom.advanced, out scom.advanced),() => updateVar(3, scom.advanced, out scom.advanced),() => updateVar(3, scom.advanced, out scom.advanced)),
                new($"(SERIAL)   DEFAULT PACKAGE LENGTH                {scom2.default_count}", () =>  WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"), () => changer(4, true, standard_counts.IndexOf(scom2.default_count), standard_counts, out scom2.default_count),() => changer(4,false, standard_counts.IndexOf(scom2.default_count), standard_counts, out scom2.default_count)),
                new($"(RENDERER) STANDARD EMPTY                        {GetState(Renderer.standard_empty)}", () => updateVar(5, Renderer.standard_empty, out Renderer.standard_empty),() => updateVar(5, Renderer.standard_empty, out Renderer.standard_empty),() => updateVar(5, Renderer.standard_empty, out Renderer.standard_empty)),
                new($"(RENDERER) DEFAULT DIRECTION                     {Renderer.standard_direction}", () =>  WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"), () => changer(6, true, standard_directions.IndexOf(Renderer.standard_direction), standard_directions, out Renderer.standard_direction),() => changer(6,false, standard_directions.IndexOf(Renderer.standard_direction), standard_directions, out Renderer.standard_direction)),
                new($"(RENDERER) APPLY PAWN FIX                        {GetState(database.display.translator[1] == "♙ ")}", database.display.ApplyPawnFix, database.display.ApplyPawnFix, database.display.ApplyPawnFix),
                new($"(PHYSICAL) TOLERANCE                             {database.physical.tolerance}", () =>  WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"), () => changer(8, true, standard_tolerance.IndexOf(database.physical.tolerance), standard_tolerance, out database.physical.tolerance),() => changer(8,false, standard_tolerance.IndexOf(database.physical.tolerance), standard_tolerance, out database.physical.tolerance)),
                new($"(PHYSICAL) DO CALIBRATION                        {GetState(database.physical.calib)}", () => updateVar(9, database.physical.calib, out database.physical.calib),() => updateVar(9, database.physical.calib, out database.physical.calib),() => updateVar(9, database.physical.calib, out database.physical.calib)),
                new($"(PHYSICAL) CALIBRATION RESOLUTION                {database.physical.default_calib}", () =>  WriteTemporaryMessage("Please use Arrowkeys to Adjust Value"), () => changer(10, true, standard_resolutions.IndexOf(database.physical.default_calib), standard_resolutions, out database.physical.default_calib),() => changer(10,false, standard_resolutions.IndexOf(database.physical.default_calib), standard_resolutions, out database.physical.default_calib)),
                new($"(PHYSICAL) DEFAULT CALIB AV                      {database.physical.default_av}", () =>  enterVar(11, out database.physical.default_av), () => changer(11, true, caav.IndexOf(av), caav, out database.physical.default_av),() => changer(11,false, caav.IndexOf(av), caav, out database.physical.default_av)),
                new($"(MCU)      LAUNCH RE-SELFTEST                    {GetState(database.physical.repeat_selftest)}", () => updateVar(12, database.physical.repeat_selftest, out database.physical.repeat_selftest),() => updateVar(12, database.physical.repeat_selftest, out database.physical.repeat_selftest),() => updateVar(12, database.physical.repeat_selftest, out database.physical.repeat_selftest)),

                new("Back to Main Menu", Main),
            };

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


        public static void updateVar(int index, bool input,out bool change)
        {
            change = !input;
            SettingsMenu(index);
        }

        public static void enterVar(int index, out int output)
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


        public static void changer(int option_index, bool direction, int index, List<int> input, out int output)
        {
            switch (direction)
            {
                case true:
                    if(index+1 < input.Count)
                    {
                        output = input[index+1];
                    }
                    else output = input[index];
                    break;
                case false:
                    if (index-1 >= 0)
                    {
                        output = input[index-1];
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
        public static string? description = null;
        public static ConsoleColor background = ConsoleColor.Black;
        public static ConsoleColor foreground = ConsoleColor.White;

    }
    public class Option
    {
        public string Name { get; }
        public Action Selected { get; }

        public Action Decrease { get; }
        public Action Increase { get; }

        public Option(string name,Action selected,Action increase = null,Action decrease = null)
        {
            Name = name;
            Selected = selected;
            Increase = increase ?? (() => { });
            Decrease = decrease ?? (() => { });
        }
    }
}