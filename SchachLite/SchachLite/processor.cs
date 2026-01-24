namespace SchachLite
{
    internal class processor
    {
        public static byte[] inverter = [7,6,5,4,3,2,1,0];
        public static bool show_icons = true;

        public static int timeout = 8;
        public static int timeout_remain = 0;

        public static void show()
        {
            Storage.log("Initializing scom2");
            scom.init();
            Renderer.draw_number(true,true,0);
        }

        public static int[,] requestAll_rangeMode()
        {
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
                //System.Diagnostics.Debug.WriteLine("Requesting Data from Arduino");
                List<string> a = new(scom.multiCommand($"QRANGE {i}",8));
                System.Diagnostics.Debug.WriteLine("Data Recieved");
                for (byte k = 0; k < 8; k++)
                {
                    if (Int32.TryParse(a[k],out int temp_field))
                    {
                        disp_board[k,i] = temp_field;
                    }
                }
            }
            return disp_board;
        }

    }
}