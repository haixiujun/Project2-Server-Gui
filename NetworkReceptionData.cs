using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2_Server_Gui
{
    class NetworkReceptionData
    {
        private Socket server_Socket;
        public NetworkReceptionData()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 40400);
            server_Socket=new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server_Socket.Bind(ip);
            server_Socket.Listen(100);
        }

        public void start_Listen_From_Client()
        {
            while (true)
            {
                try
                {
                    var accept = server_Socket.Accept();
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        while (true)
                        {
                            try
                            {
                                byte[] receive = new byte[1024];
                                int len = accept.Receive(receive);
                                string control_Code = System.Text.Encoding.UTF8.GetString(receive.Take(len).ToArray());

                                if (control_Code.Equals("REG"))
                                {
                                    REG(accept);
                                }
                                else if (control_Code.Equals("SIGNIN"))
                                {
                                    SIGNIN(accept);
                                }
                            }
                            catch(Exception e)
                            {

                            }
                            finally
                            {
                                accept.Close();
                            }
                           
                        }

                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
        public static void SIGNIN(Socket accept)
        {
            byte[] receive = new byte[1024 * 1024 * 150];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            int user_Id = Convert.ToInt32(tmp[0]);
            bool is_Password = mySQLC.is_Password(user_Id, tmp[1]);
            if (is_Password)
            {
                accept.Send(Encoding.UTF8.GetBytes("OK"));
            }
            else
            {
                accept.Send(Encoding.UTF8.GetBytes("NO"));
            }
        }

        public static void REG(Socket accept)
        {
            byte[] receive = new byte[1024*1024*150];
            byte[] clear_Receive;
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            string back_Code = "";
            accept.Send(Encoding.UTF8.GetBytes("OK"));

            int len = accept.Receive(receive);

            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            string temp_Name = tmp[0];
            int temp_ID = Convert.ToInt32(tmp[1]);
            string temp_Password = tmp[2];

            bool have=mySQLC.search_User(temp_ID);
            if (have)
            {
                accept.Send(Encoding.UTF8.GetBytes("NO"));
                return;
            }
            else
            {
                mySQLC.write_To_usertable(temp_Name,temp_ID,temp_Password);
                accept.Send(Encoding.UTF8.GetBytes("OK"));
            }
            mySQLC.stop_Connect_To_DB();
            
        }




    }
}
