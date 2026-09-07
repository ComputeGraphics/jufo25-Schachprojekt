using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessMagnet
{
    public partial class Form3 : Form
    {
        int WB1_POS = 71;
        int WB2_POS = 72;
        int WB3_POS = 73;
        int WB4_POS = 74;
        int WB5_POS = 75;
        int WB6_POS = 76;
        int WB7_POS = 77;
        int WB8_POS = 78;

        int WT1_POS;
        int WS1_POS;
        int WL1_POS;
        int WQ_POS;
        int WK_POS;
        int WL2_POS;
        int WS2_POS;
        int WT2_POS;

        int SB1_POS;
        int SB2_POS;
        int SB3_POS;
        int SB4_POS;
        int SB5_POS;
        int SB6_POS;
        int SB7_POS;
        int SB8_POS;

        int ST1_POS;
        int SS1_POS;
        int SL1_POS;
        int SQ_POS;
        int SK_POS;
        int SL2_POS;
        int SS2_POS;
        int ST2_POS;


        public Form3()
        {
            InitializeComponent();
            //label1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<string> names = new List<string>();

            int threshold = 70;

            if (WB1_POS > threshold)
            {
                names.Add("num1");
            }
            if (WB2_POS > threshold)
            {
                names.Add("num2");
            }
            if (WB3_POS > threshold)
            {
                names.Add("num3");
            }

            if (names.Count > 0)
            {
                Console.WriteLine("Die Variablen mit dem Wert größer als " + threshold + " sind:");
                foreach (string name in names)
                {
                    Console.WriteLine(name);
                }
            }
            else
            {
                Console.WriteLine("Keine der Variablen hat den Schwellenwert von " + threshold + " überschritten.");
            }
        }
    }
}
