using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project2_Server_Gui
{
    public partial class Form1 : Form
    {
        private NetworkReceptionData networkReceptionData;
        public Form1()
        {
            InitializeComponent();
            networkReceptionData = new NetworkReceptionData();
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                MySQLConnectionClass mySQL = new MySQLConnectionClass();
                networkReceptionData.start_Listen_From_Client();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] temps = networkReceptionData.get_Result_Items();
            listBox2.Items.Clear();
            for(int i = 0; i < temps.Length; i++)
            {
                listBox2.Items.Add(temps[i].Replace("#", "|"));
            }
            
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            networkReceptionData.clear_Unused_Socket();
            string all_IP = networkReceptionData.get_Socket_Linkers_IP();
            string[] ips = all_IP.Split("#");
            for(int i = 0; i < ips.Length; i++)
            {
                listBox1.Items.Add(ips[i]);
            }
            richTextBox1.Text += networkReceptionData.get_Journal();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
}
