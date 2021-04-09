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
                                else if (control_Code.Equals("SIN"))
                                {
                                    SIN(accept);
                                }
                                else if (control_Code.Equals("DSW"))
                                {
                                    DSW(accept);
                                }
                                else if (control_Code.Equals("DSR"))
                                {
                                    DSR(accept);
                                }
                                else if (control_Code.Equals("RSW"))
                                {
                                    RSW(accept);
                                }
                                else if (control_Code.Equals("RSR"))
                                {
                                    RSR(accept);
                                }
                                else if (control_Code.Equals("DSD"))
                                {
                                    DSD(accept);
                                }
                                else if (control_Code.Equals("RSD"))
                                {
                                    RSD(accept);
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


        public static void RSD(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            mySQLC.delete_Result_Surface_Data(file_Name, data_Set_Serial);
            accept.Send(Encoding.UTF8.GetBytes("OK"));

        }


        public static void DSD(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            mySQLC.delete_Data_Set_Data(file_Name, data_Set_Serial);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
        }

        public static void RSR(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            using (MySqlDataReader reader = mySQLC.get_Result_Surface_Reader(file_Name, data_Set_Serial))
            {
                if (reader.Read())
                {
                    accept.Send(Encoding.UTF8.GetBytes("OK"));
                    len = accept.Receive(receive);
                    clear_Receive = receive.Take(len).ToArray();
                    back_Code = Encoding.UTF8.GetString(clear_Receive);
                    if (back_Code.Equals("OK"))
                    {
                        int temp_Item_Count = Convert.ToInt32(reader.GetString("ItemCount"));
                        int temp_Cubage = Convert.ToInt32(reader.GetString("Cubage"));
                        int temp_Max_Result = Convert.ToInt32(reader.GetString("MaxResult"));
                        string send_Message = temp_Item_Count.ToString() + "#";
                        send_Message += temp_Cubage.ToString() + "#";
                        send_Message += temp_Max_Result.ToString();
                        accept.Send(Encoding.UTF8.GetBytes(send_Message));
                    }
                }
                else
                {
                    accept.Send(Encoding.UTF8.GetBytes("NO"));
                }
                

            }
        }





        public static void RSW(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            int data_Count = 0;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            data_Count = Convert.ToInt32(back_Code);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            for (int i = 0; i < data_Count; i++)
            {
                len = accept.Receive(receive);
                clear_Receive = receive.Take(len).ToArray();
                back_Code = Encoding.UTF8.GetString(clear_Receive);
                string[] tmp = back_Code.Split("#");
                string temp_Name = tmp[0];
                int temp_Data_Set_Serial = Convert.ToInt32(tmp[1]);
                int temp_Item_Count = Convert.ToInt32(tmp[2]);
                int temp_Cubage = Convert.ToInt32(tmp[3]);
                int temp_Max_Result = Convert.ToInt32(tmp[4]);
                mySQLC.write_To_resultsurface(temp_Name,temp_Data_Set_Serial,temp_Item_Count,temp_Cubage,temp_Max_Result);
                accept.Send(Encoding.UTF8.GetBytes("OK"));
            }
        }



        public static void DSR(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            using (MySqlDataReader reader = mySQLC.get_Data_Set_Reader(file_Name, data_Set_Serial))
            {
                while (reader.Read())
                {
                    accept.Send(Encoding.UTF8.GetBytes("OK"));
                    len = accept.Receive(receive);
                    clear_Receive = receive.Take(len).ToArray();
                    back_Code = Encoding.UTF8.GetString(clear_Receive);
                    if (back_Code.Equals("OK"))
                    {
                        string send = reader.GetString("ItemSetSerial")+"#";
                        send += reader.GetString("Profit") + "#";
                        send += reader.GetString("Weight") + "#";
                        send += reader.GetString("Radio");
                        accept.Send(Encoding.UTF8.GetBytes(send));
                    }
                    else
                    {
                        accept.Send(Encoding.UTF8.GetBytes("NO"));
                        return;
                    }
                }
                accept.Send(Encoding.UTF8.GetBytes("NO"));
            }
        }

        public static void DSW(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            int data_Count = 0;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            data_Count = Convert.ToInt32(back_Code);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            for (int i = 0; i < data_Count; i++)
            {
                len = accept.Receive(receive);
                clear_Receive = receive.Take(len).ToArray();
                back_Code = Encoding.UTF8.GetString(clear_Receive);
                string[] tmp = back_Code.Split("#");
                string temp_Name = tmp[0];
                int temp_Data_Set_Serial = Convert.ToInt32(tmp[1]);
                int temp_Item_Set_Serial= Convert.ToInt32(tmp[2]);
                int temp_Profit= Convert.ToInt32(tmp[3]);
                int temp_Weight= Convert.ToInt32(tmp[4]);
                decimal temp_Radio= Convert.ToDecimal(tmp[5]);
                mySQLC.write_To_Data_Set_DB(temp_Name, temp_Data_Set_Serial, temp_Item_Set_Serial, temp_Profit, temp_Weight, temp_Radio);
                accept.Send(Encoding.UTF8.GetBytes("OK"));
            }

        }

        public static void SIN(Socket accept)
        {
            byte[] receive = new byte[1024 ];
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
            mySQLC.stop_Connect_To_DB();
        }

        public static void REG(Socket accept)
        {
            byte[] receive = new byte[1024];
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
