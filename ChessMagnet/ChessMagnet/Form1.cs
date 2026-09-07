using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Transitions;

namespace ChessMagnet
{
    public partial class Form1 : Form
    {

        int serial_open = 0;
        int seconds = 0;
        string baud;
        string com;

        int movement_speed = 5;

        byte anti_iterate;
        byte anti_iterate_x;

        int WB1_XPOS = 69;
        int WB1_YPOS = 453;
        int WB1_YMOV;
        int WB1_XMOV;
        int WB1_POS_CACHE;
        int WB1_MOV_CACHE;
        int WB1_POS_CACHE_x;
        int WB1_MOV_CACHE_x;

        int WB2_XPOS = 134;
        int WB2_YPOS = 453;
        int WB2_YMOV;
        int WB2_XMOV;
        int WB2_POS_CACHE;
        int WB2_MOV_CACHE;
        int WB2_POS_CACHE_x;
        int WB2_MOV_CACHE_x;

        int WB3_XPOS = 199;
        int WB3_YPOS = 453;
        int WB3_YMOV;
        int WB3_XMOV;
        int WB3_POS_CACHE;
        int WB3_MOV_CACHE;
        int WB3_POS_CACHE_x;
        int WB3_MOV_CACHE_x;

        int WB4_XPOS = 263;
        int WB4_YPOS = 453;
        int WB4_YMOV;
        int WB4_XMOV;
        int WB4_POS_CACHE;
        int WB4_MOV_CACHE;
        int WB4_POS_CACHE_x;
        int WB4_MOV_CACHE_x;

        int WB5_XPOS = 327;
        int WB5_YPOS = 453;
        int WB5_YMOV;
        int WB5_XMOV;
        int WB5_POS_CACHE;
        int WB5_MOV_CACHE;
        int WB5_POS_CACHE_x;
        int WB5_MOV_CACHE_x;

        int WB6_XPOS = 392;
        int WB6_YPOS = 453;
        int WB6_YMOV;
        int WB6_XMOV;
        int WB6_POS_CACHE;
        int WB6_MOV_CACHE;
        int WB6_POS_CACHE_x;
        int WB6_MOV_CACHE_x;

        int WB7_XPOS = 457;
        int WB7_YPOS = 453;
        int WB7_YMOV;
        int WB7_XMOV;
        int WB7_POS_CACHE;
        int WB7_MOV_CACHE;
        int WB7_POS_CACHE_x;
        int WB7_MOV_CACHE_x;

        int WB8_XPOS = 521;
        int WB8_YPOS = 453;
        int WB8_YMOV;
        int WB8_XMOV;
        int WB8_POS_CACHE;
        int WB8_MOV_CACHE;
        int WB8_POS_CACHE_x;
        int WB8_MOV_CACHE_x;



        SerialPort mySerialPort = new SerialPort("COM6", 9600);
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            timer2.Start();
            label1.Text = "N/A";
            //label6.Visible = false;

            //                 Weiße Bauern                 //
            WB1.BackColor = System.Drawing.Color.Transparent;
            WB2.BackColor = System.Drawing.Color.Transparent;
            WB3.BackColor = System.Drawing.Color.Transparent;
            WB4.BackColor = System.Drawing.Color.Transparent;
            WB5.BackColor = System.Drawing.Color.Transparent;
            WB6.BackColor = System.Drawing.Color.Transparent;
            WB7.BackColor = System.Drawing.Color.Transparent;
            WB8.BackColor = System.Drawing.Color.Transparent;

            //                 Weiße Türme                  //
            WT1.BackColor = System.Drawing.Color.Transparent;
            WT2.BackColor = System.Drawing.Color.Transparent;

            //                 Weiße Läufer                 //
            WL1.BackColor = System.Drawing.Color.Transparent;
            WL2.BackColor = System.Drawing.Color.Transparent;

            //                Weiße Springer                //
            WS1.BackColor = System.Drawing.Color.Transparent;
            WS2.BackColor = System.Drawing.Color.Transparent;

            //                  Weiße Dame                  //
            WD.BackColor = System.Drawing.Color.Transparent;

            //                 Weißer König                 //
            WK.BackColor = System.Drawing.Color.Transparent;
            //////////////////////////////////////////////////
            //                Schwarze Bauern               //
            SB1.BackColor = System.Drawing.Color.Transparent;
            SB2.BackColor = System.Drawing.Color.Transparent;
            SB3.BackColor = System.Drawing.Color.Transparent;
            SB4.BackColor = System.Drawing.Color.Transparent;
            SB5.BackColor = System.Drawing.Color.Transparent;
            SB6.BackColor = System.Drawing.Color.Transparent;
            SB7.BackColor = System.Drawing.Color.Transparent;
            SB8.BackColor = System.Drawing.Color.Transparent;

            //                Schwarze Türme                //
            ST1.BackColor = System.Drawing.Color.Transparent;
            ST2.BackColor = System.Drawing.Color.Transparent;

            //               Schwarze Läufer                //
            SL1.BackColor = System.Drawing.Color.Transparent;
            SL2.BackColor = System.Drawing.Color.Transparent;

            //               Schwarze Springer              //
            SS1.BackColor = System.Drawing.Color.Transparent;
            SS2.BackColor = System.Drawing.Color.Transparent;

            //                Schwarze Dame                 //
            SD.BackColor = System.Drawing.Color.Transparent;

            //               Schwarzer König                //
            SK.BackColor = System.Drawing.Color.Transparent;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            com = comboBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySerialPort.Open();
            serial_open = 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (seconds == 4)
            {
                if (serial_open == 1)
                {
                    string CurrentLine = mySerialPort.ReadLine();
                    label1.Text = CurrentLine;
                }
                seconds = 0;
            }
            label6.Text = seconds.ToString();
            seconds++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serial_open == 1)
            {
                mySerialPort.Close();
                serial_open = 0;
                label1.Text = "N/A";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //label1.Text=comboBox2.Text;
            baud = comboBox2.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "WB1 to F1")
            {
                this.WB1.Location = new System.Drawing.Point(69, 407);
                WB1_YPOS = 407;
                WB1_XPOS = 69;
                //Thread.Sleep(50);
            }
            if (textBox1.Text == "WB1 to F2")
            {
                this.WB1.Location = new System.Drawing.Point(69, 342);
            }
            if (textBox1.Text == "WB1 to F3")
            {
                this.WB1.Location = new System.Drawing.Point(69, 276);
            }
            if (textBox1.Text == "WB1 to F4")
            {
                this.WB1.Location = new System.Drawing.Point(69, 211);
            }
            if (textBox1.Text == "WB1 reset")
            {
                this.WB1.Location = new System.Drawing.Point(69, 472);
            }
            if (textBox1.Text == "WB1 Y+1")
            {
                WB1_YMOV = 1;
                WB1_MOV_CACHE = WB1_YMOV * 13;
                WB1_POS_CACHE = WB1_MOV_CACHE + WB1_YPOS;

                Transition.run(WB1, "Top", WB1_POS_CACHE, new TransitionType_EaseInEaseOut(1500));
            }
            //Addiere oder Subtrahiere 65 für ein Feld auf Y
            //Addiere oder Subtrahiere x für ein Feld auf X
            //Addiere oder Subtrahiere x und y für Diagonalbewegung
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// MOVEMENT BAUER WB1 //////////////////////////////////////////////////////////////////////////////////////
            this.WB1.Location = new System.Drawing.Point(WB1_XPOS, WB1_YPOS);  
            if(WB1_YMOV != 0)
            {           
                if(anti_iterate < 13)
                {
                    WB1_MOV_CACHE = WB1_YMOV * 3;
                    anti_iterate++;

                    if (WB1_POS_CACHE != WB1_YPOS)
                    {
                        if (WB1_YMOV > 0)
                        {
                            WB1_YPOS -= movement_speed;
                            WB1_POS_CACHE = WB1_YPOS + WB1_MOV_CACHE;
                        }
                        if (WB1_YMOV < 0)
                        {
                            WB1_YPOS += movement_speed;
                            WB1_POS_CACHE = WB1_YPOS - WB1_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB1_YMOV < 0)
                        {
                            WB1_YMOV++;
                        }
                        if (WB1_YMOV > 0)
                        {
                            WB1_YMOV--;
                        }
                        
                        anti_iterate = 0;
                        WB1_POS_CACHE = 0;
                        WB1_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB1_YMOV < 0)
                    {
                        WB1_YMOV++;
                    }
                    if (WB1_YMOV > 0)
                    {
                        WB1_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB1_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB1_MOV_CACHE_x = WB1_XMOV * 3;
                    WB1_POS_CACHE_x = WB1_XPOS + WB1_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB1_POS_CACHE_x != WB1_XPOS)
                    {
                        if (WB1_XMOV > 0)
                        {
                            WB1_XPOS += movement_speed;
                        }
                        if (WB1_XMOV < 0)
                        {
                            WB1_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB1_XMOV < 0)
                        {
                            WB1_XMOV++;
                        }
                        if (WB1_XMOV > 0)
                        {
                            WB1_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB1_XMOV < 0)
                    {
                        WB1_XMOV++;
                    }
                    if (WB1_XMOV > 0)
                    {
                        WB1_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB1_POS_CACHE_x = 0;
                    WB1_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB2 //////////////////////////////////////////////////////////////////////////////////////
            this.WB2.Location = new System.Drawing.Point(WB2_XPOS, WB2_YPOS);
            if (WB2_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB2_MOV_CACHE = WB2_YMOV * 3;
                    anti_iterate++;

                    if (WB2_POS_CACHE != WB2_YPOS)
                    {
                        if (WB2_YMOV > 0)
                        {
                            WB2_YPOS -= movement_speed;
                            WB2_POS_CACHE = WB2_YPOS + WB2_MOV_CACHE;
                        }
                        if (WB2_YMOV < 0)
                        {
                            WB2_YPOS += movement_speed;
                            WB2_POS_CACHE = WB2_YPOS - WB2_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB2_YMOV < 0)
                        {
                            WB2_YMOV++;
                        }
                        if (WB2_YMOV > 0)
                        {
                            WB2_YMOV--;
                        }

                        anti_iterate = 0;
                        WB2_POS_CACHE = 0;
                        WB2_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB2_YMOV < 0)
                    {
                        WB2_YMOV++;
                    }
                    if (WB2_YMOV > 0)
                    {
                        WB2_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB2_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB2_MOV_CACHE_x = WB2_XMOV * 3;
                    WB2_POS_CACHE_x = WB2_XPOS + WB2_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB2_POS_CACHE_x != WB2_XPOS)
                    {
                        if (WB2_XMOV > 0)
                        {
                            WB2_XPOS += movement_speed;
                        }
                        if (WB2_XMOV < 0)
                        {
                            WB2_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB2_XMOV < 0)
                        {
                            WB2_XMOV++;
                        }
                        if (WB2_XMOV > 0)
                        {
                            WB2_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB2_XMOV < 0)
                    {
                        WB2_XMOV++;
                    }
                    if (WB2_XMOV > 0)
                    {
                        WB2_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB2_POS_CACHE_x = 0;
                    WB2_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB3 //////////////////////////////////////////////////////////////////////////////////////
            this.WB3.Location = new System.Drawing.Point(WB3_XPOS, WB3_YPOS);
            if (WB3_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB3_MOV_CACHE = WB3_YMOV * 3;
                    anti_iterate++;

                    if (WB3_POS_CACHE != WB3_YPOS)
                    {
                        if (WB3_YMOV > 0)
                        {
                            WB3_YPOS -= movement_speed;
                            WB3_POS_CACHE = WB3_YPOS + WB3_MOV_CACHE;
                        }
                        if (WB3_YMOV < 0)
                        {
                            WB3_YPOS += movement_speed;
                            WB3_POS_CACHE = WB3_YPOS - WB3_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB3_YMOV < 0)
                        {
                            WB3_YMOV++;
                        }
                        if (WB3_YMOV > 0)
                        {
                            WB3_YMOV--;
                        }

                        anti_iterate = 0;
                        WB3_POS_CACHE = 0;
                        WB3_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB3_YMOV < 0)
                    {
                        WB3_YMOV++;
                    }
                    if (WB3_YMOV > 0)
                    {
                        WB3_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB3_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB3_MOV_CACHE_x = WB3_XMOV * 3;
                    WB3_POS_CACHE_x = WB3_XPOS + WB3_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB3_POS_CACHE_x != WB3_XPOS)
                    {
                        if (WB3_XMOV > 0)
                        {
                            WB3_XPOS += movement_speed;
                        }
                        if (WB3_XMOV < 0)
                        {
                            WB3_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB3_XMOV < 0)
                        {
                            WB3_XMOV++;
                        }
                        if (WB3_XMOV > 0)
                        {
                            WB3_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB3_XMOV < 0)
                    {
                        WB3_XMOV++;
                    }
                    if (WB3_XMOV > 0)
                    {
                        WB3_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB3_POS_CACHE_x = 0;
                    WB3_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB4 //////////////////////////////////////////////////////////////////////////////////////
            this.WB4.Location = new System.Drawing.Point(WB4_XPOS, WB4_YPOS);
            if (WB4_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB4_MOV_CACHE = WB4_YMOV * 3;
                    anti_iterate++;

                    if (WB4_POS_CACHE != WB4_YPOS)
                    {
                        if (WB4_YMOV > 0)
                        {
                            WB4_YPOS -= movement_speed;
                            WB4_POS_CACHE = WB4_YPOS + WB4_MOV_CACHE;
                        }
                        if (WB4_YMOV < 0)
                        {
                            WB4_YPOS += movement_speed;
                            WB4_POS_CACHE = WB4_YPOS - WB4_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB4_YMOV < 0)
                        {
                            WB4_YMOV++;
                        }
                        if (WB4_YMOV > 0)
                        {
                            WB4_YMOV--;
                        }

                        anti_iterate = 0;
                        WB4_POS_CACHE = 0;
                        WB4_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB4_YMOV < 0)
                    {
                        WB4_YMOV++;
                    }
                    if (WB4_YMOV > 0)
                    {
                        WB4_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB4_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB4_MOV_CACHE_x = WB4_XMOV * 3;
                    WB4_POS_CACHE_x = WB4_XPOS + WB4_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB4_POS_CACHE_x != WB4_XPOS)
                    {
                        if (WB4_XMOV > 0)
                        {
                            WB4_XPOS += movement_speed;
                        }
                        if (WB4_XMOV < 0)
                        {
                            WB4_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB4_XMOV < 0)
                        {
                            WB4_XMOV++;
                        }
                        if (WB4_XMOV > 0)
                        {
                            WB4_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB4_XMOV < 0)
                    {
                        WB4_XMOV++;
                    }
                    if (WB4_XMOV > 0)
                    {
                        WB4_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB4_POS_CACHE_x = 0;
                    WB4_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB5 //////////////////////////////////////////////////////////////////////////////////////
            this.WB5.Location = new System.Drawing.Point(WB5_XPOS, WB5_YPOS);
            if (WB5_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB5_MOV_CACHE = WB5_YMOV * 3;
                    anti_iterate++;

                    if (WB5_POS_CACHE != WB5_YPOS)
                    {
                        if (WB5_YMOV > 0)
                        {
                            WB5_YPOS -= movement_speed;
                            WB5_POS_CACHE = WB5_YPOS + WB5_MOV_CACHE;
                        }
                        if (WB5_YMOV < 0)
                        {
                            WB5_YPOS += movement_speed;
                            WB5_POS_CACHE = WB5_YPOS - WB5_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB5_YMOV < 0)
                        {
                            WB5_YMOV++;
                        }
                        if (WB5_YMOV > 0)
                        {
                            WB5_YMOV--;
                        }

                        anti_iterate = 0;
                        WB5_POS_CACHE = 0;
                        WB5_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB5_YMOV < 0)
                    {
                        WB5_YMOV++;
                    }
                    if (WB5_YMOV > 0)
                    {
                        WB5_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB5_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB5_MOV_CACHE_x = WB5_XMOV * 3;
                    WB5_POS_CACHE_x = WB5_XPOS + WB5_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB5_POS_CACHE_x != WB5_XPOS)
                    {
                        if (WB5_XMOV > 0)
                        {
                            WB5_XPOS += movement_speed;
                        }
                        if (WB5_XMOV < 0)
                        {
                            WB5_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB5_XMOV < 0)
                        {
                            WB5_XMOV++;
                        }
                        if (WB5_XMOV > 0)
                        {
                            WB5_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB5_XMOV < 0)
                    {
                        WB5_XMOV++;
                    }
                    if (WB5_XMOV > 0)
                    {
                        WB5_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB5_POS_CACHE_x = 0;
                    WB5_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB6 //////////////////////////////////////////////////////////////////////////////////////
            this.WB6.Location = new System.Drawing.Point(WB6_XPOS, WB6_YPOS);
            if (WB6_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB6_MOV_CACHE = WB6_YMOV * 3;
                    anti_iterate++;

                    if (WB6_POS_CACHE != WB6_YPOS)
                    {
                        if (WB6_YMOV > 0)
                        {
                            WB6_YPOS -= movement_speed;
                            WB6_POS_CACHE = WB6_YPOS + WB6_MOV_CACHE;
                        }
                        if (WB6_YMOV < 0)
                        {
                            WB6_YPOS += movement_speed;
                            WB6_POS_CACHE = WB6_YPOS - WB6_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB6_YMOV < 0)
                        {
                            WB6_YMOV++;
                        }
                        if (WB6_YMOV > 0)
                        {
                            WB6_YMOV--;
                        }

                        anti_iterate = 0;
                        WB6_POS_CACHE = 0;
                        WB6_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB6_YMOV < 0)
                    {
                        WB6_YMOV++;
                    }
                    if (WB6_YMOV > 0)
                    {
                        WB6_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB6_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB6_MOV_CACHE_x = WB6_XMOV * 3;
                    WB6_POS_CACHE_x = WB6_XPOS + WB6_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB6_POS_CACHE_x != WB6_XPOS)
                    {
                        if (WB6_XMOV > 0)
                        {
                            WB6_XPOS += movement_speed;
                        }
                        if (WB6_XMOV < 0)
                        {
                            WB6_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB6_XMOV < 0)
                        {
                            WB6_XMOV++;
                        }
                        if (WB6_XMOV > 0)
                        {
                            WB6_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB6_XMOV < 0)
                    {
                        WB6_XMOV++;
                    }
                    if (WB6_XMOV > 0)
                    {
                        WB6_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB6_POS_CACHE_x = 0;
                    WB6_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB7 //////////////////////////////////////////////////////////////////////////////////////
            this.WB7.Location = new System.Drawing.Point(WB7_XPOS, WB7_YPOS);
            if (WB7_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB7_MOV_CACHE = WB7_YMOV * 3;
                    anti_iterate++;

                    if (WB7_POS_CACHE != WB7_YPOS)
                    {
                        if (WB7_YMOV > 0)
                        {
                            WB7_YPOS -= movement_speed;
                            WB7_POS_CACHE = WB7_YPOS + WB7_MOV_CACHE;
                        }
                        if (WB7_YMOV < 0)
                        {
                            WB7_YPOS += movement_speed;
                            WB7_POS_CACHE = WB7_YPOS - WB7_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB7_YMOV < 0)
                        {
                            WB7_YMOV++;
                        }
                        if (WB7_YMOV > 0)
                        {
                            WB7_YMOV--;
                        }

                        anti_iterate = 0;
                        WB7_POS_CACHE = 0;
                        WB7_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB7_YMOV < 0)
                    {
                        WB7_YMOV++;
                    }
                    if (WB7_YMOV > 0)
                    {
                        WB7_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB7_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB7_MOV_CACHE_x = WB7_XMOV * 3;
                    WB7_POS_CACHE_x = WB7_XPOS + WB7_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB7_POS_CACHE_x != WB7_XPOS)
                    {
                        if (WB7_XMOV > 0)
                        {
                            WB7_XPOS += movement_speed;
                        }
                        if (WB7_XMOV < 0)
                        {
                            WB7_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB7_XMOV < 0)
                        {
                            WB7_XMOV++;
                        }
                        if (WB7_XMOV > 0)
                        {
                            WB7_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB7_XMOV < 0)
                    {
                        WB7_XMOV++;
                    }
                    if (WB7_XMOV > 0)
                    {
                        WB7_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB7_POS_CACHE_x = 0;
                    WB7_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// MOVEMENT BAUER WB8 //////////////////////////////////////////////////////////////////////////////////////
            this.WB8.Location = new System.Drawing.Point(WB8_XPOS, WB8_YPOS);
            if (WB8_YMOV != 0)
            {
                if (anti_iterate < 13)
                {
                    WB8_MOV_CACHE = WB8_YMOV * 3;
                    anti_iterate++;

                    if (WB8_POS_CACHE != WB8_YPOS)
                    {
                        if (WB8_YMOV > 0)
                        {
                            WB8_YPOS -= movement_speed;
                            WB8_POS_CACHE = WB8_YPOS + WB8_MOV_CACHE;
                        }
                        if (WB8_YMOV < 0)
                        {
                            WB8_YPOS += movement_speed;
                            WB8_POS_CACHE = WB8_YPOS - WB8_MOV_CACHE;
                        }
                    }
                    else
                    {
                        if (WB8_YMOV < 0)
                        {
                            WB8_YMOV++;
                        }
                        if (WB8_YMOV > 0)
                        {
                            WB8_YMOV--;
                        }

                        anti_iterate = 0;
                        WB8_POS_CACHE = 0;
                        WB8_MOV_CACHE = 0;
                    }
                }
                else
                {
                    if (WB8_YMOV < 0)
                    {
                        WB8_YMOV++;
                    }
                    if (WB8_YMOV > 0)
                    {
                        WB8_YMOV--;
                    }
                    anti_iterate = 0;
                }
                //label1.Text = anti_iterate.ToString();
                label2.Text = anti_iterate.ToString();
            }
            if (WB8_XMOV != 0)
            {
                if (anti_iterate_x < 13)
                {
                    WB8_MOV_CACHE_x = WB8_XMOV * 3;
                    WB8_POS_CACHE_x = WB8_XPOS + WB8_MOV_CACHE_x;
                    anti_iterate_x++;

                    if (WB8_POS_CACHE_x != WB8_XPOS)
                    {
                        if (WB8_XMOV > 0)
                        {
                            WB8_XPOS += movement_speed;
                        }
                        if (WB8_XMOV < 0)
                        {
                            WB8_XPOS -= movement_speed;
                        }
                    }
                    else
                    {
                        if (WB8_XMOV < 0)
                        {
                            WB8_XMOV++;
                        }
                        if (WB8_XMOV > 0)
                        {
                            WB8_XMOV--;
                        }
                        anti_iterate_x = 0;
                    }
                }
                else
                {
                    if (WB8_XMOV < 0)
                    {
                        WB8_XMOV++;
                    }
                    if (WB8_XMOV > 0)
                    {
                        WB8_XMOV--;
                    }
                    anti_iterate_x = 0;
                    WB8_POS_CACHE_x = 0;
                    WB8_MOV_CACHE_x = 0;
                }
                //label1.Text = anti_iterate.ToString();
            }





































            //label2.Text = anti_iterate_x.ToString();
            this.Invalidate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WB1_YMOV = 1;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WB1_YMOV = -1;
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox1.Text, out WB1_YMOV);
            //toolStripTextBox1.Text = null;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            WB1_XPOS = 69;
            WB1_YPOS = 453;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            WB1_YMOV = 1;
            WB1_XMOV = 1;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            WB1_YMOV = 1;
            WB1_XMOV = -1;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            WB2_YMOV = 1;
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            WB2_YMOV = -1;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            WB2_YMOV = 1;
            WB2_XMOV = -1;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            WB2_YMOV = 1;
            WB2_XMOV = 1;
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            WB2_XPOS = 134;
            WB2_YPOS = 453;
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox2.Text, out WB2_YMOV);
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            WB3_YMOV = 1;
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            WB3_YMOV = -1;
        }

        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox3.Text, out WB3_YMOV);
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            WB3_YMOV = 1;
            WB3_XMOV = -1;
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            WB3_YMOV = 1;
            WB3_XMOV = 1;
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            WB3_XPOS = 199;
            WB3_YPOS = 453;
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            WB4_XPOS = 263;
            WB4_YPOS = 453;
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            WB4_XMOV = 1;
            WB4_YMOV = 1;
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            WB4_XMOV = -1;
            WB4_YMOV = 1;
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            WB4_YMOV = -1;
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            WB4_YMOV = 1;
        }

        private void toolStripTextBox4_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox4.Text, out WB4_YMOV);
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            WB5_YMOV = 1;
        }

        private void toolStripTextBox5_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox5.Text, out WB5_YMOV);
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            WB5_YMOV = -1;
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            WB5_YMOV = 1;
            WB5_XMOV = -1;
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            WB5_YMOV = 1;
            WB5_XMOV = 1;
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            WB5_XPOS = 327;
            WB5_YPOS = 453;
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            WB6_YMOV = 1;
        }


        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            WB6_YMOV = -1;
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            WB6_YMOV = 1;
            WB6_XMOV = -1;
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            WB6_YMOV = 1;
            WB6_XMOV = 1;
        }

        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            WB6_XPOS = 392;
            WB6_YPOS = 453;
        }

        private void toolStripTextBox6_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox6.Text, out WB6_YMOV);
        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            WB7_YMOV = 1;
        }

        private void toolStripTextBox7_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox7.Text, out WB7_YMOV);
        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            WB7_YMOV = -1;
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
            WB7_YMOV = 1;
            WB7_XMOV = -1;
        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            WB7_YMOV = 1;
            WB7_XMOV = 1;
        }

        private void toolStripMenuItem35_Click(object sender, EventArgs e)
        {
            WB7_XPOS = 457;
            WB7_YPOS = 453;
        }

        private void toolStripMenuItem36_Click(object sender, EventArgs e)
        {
            WB8_YMOV = 1;
        }

        private void toolStripTextBox8_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(toolStripTextBox8.Text, out WB8_YMOV);
        }

        private void toolStripMenuItem37_Click(object sender, EventArgs e)
        {
            WB8_YMOV = -1;
        }

        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            WB8_YMOV = 1;
            WB8_XMOV = -1;
        }

        private void toolStripMenuItem39_Click(object sender, EventArgs e)
        {
            WB8_YMOV = 1;
            WB8_XMOV = 1;
        }

        private void toolStripMenuItem40_Click(object sender, EventArgs e)
        {
            WB8_XPOS = 521;
            WB8_YPOS = 453;
        }
        ///////////////////////

    }
}
