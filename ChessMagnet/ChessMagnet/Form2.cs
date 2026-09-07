using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;
using WinFormAnimation;

namespace ChessMagnet
{
    public partial class Form2 : Form
    {
        byte play;
        byte anim = 2;
        int y_pos_play;
        int seconds;
        public Form2()
        {
            InitializeComponent();

            pictureBox9.Visible = false;
            pictureBox10.Visible = false;
            pictureBox8.Visible = false;
            label6.Visible = false;  //Verbindunsaufbau
            label8.Visible = false; //Erfolg
            label9.Visible = false; //Fehlgeschlagen
            label10.Visible = true;

            Transition t = new Transition(new TransitionType_EaseInEaseOut(1000));
            t.add(panel4, "Left", GROUP_POPUP);
            t.run();
            


            this.timer1.Start();
            panel3.Visible = true;
        }
        private const int GROUP_POPUP = 663;
        private void button3_Click(object sender, EventArgs e)
        {
            int Destination = 102;
            panel3.Visible = true;
            Transition.run(panel3, "Top", Destination, new TransitionType_EaseInEaseOut(1500));
            play = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            int Destination = 680;
            Transition.run(panel3, "Top", Destination, new TransitionType_EaseInEaseOut(1500));
            //panel3.Visible = false;
            play = 0;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

            int Destination = 102;
            Transition.run(panel3, "Top", Destination, new TransitionType_EaseInEaseOut(1500));
            panel3.Visible = true;
            play = 1;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int Destination = 680;
            Transition.run(panel3, "Top", Destination, new TransitionType_EaseInEaseOut(1500));
            //panel3.Visible = false;
            
            play = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            seconds++;
            if(seconds == 1)
            {
                label10.Visible = false;
                pictureBox9.Visible = true;     //Ladeanimation
                pictureBox8.Visible = false;    //Erfolg
                pictureBox10.Visible = false;   //Fehlgeschlagen
                label6.Visible = true;  //Verbindunsaufbau
                label8.Visible = false; //Erfolg
                label9.Visible = false; //Fehlgeschlagen
            }
            if(seconds == 20)
            {
                pictureBox9.Visible = false;
                pictureBox10.Visible = false;
                pictureBox8.Visible = true;
                label6.Visible = false;  //Verbindunsaufbau
                label8.Visible = true; //Erfolg
                label9.Visible = false; //Fehlgeschlagen
            }
            if(seconds == 40 )
            {
                pictureBox9.Visible = false;
                pictureBox10.Visible = true;
                pictureBox8.Visible = false;
                label6.Visible = false;  //Verbindunsaufbau
                label8.Visible = false; //Erfolg
                label9.Visible = true; //Fehlgeschlagen
            }
            if(seconds == 60)
            {
                Transition t = new Transition(new TransitionType_EaseInEaseOut(1000));
                t.add(panel4, "Left", 1259);
                t.run();

            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();
        }
    }
}
