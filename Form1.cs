using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySQLConnectionClass mySQL = new MySQLConnectionClass();
            networkReceptionData.start_Listen_From_Client();
        }
    }
}
