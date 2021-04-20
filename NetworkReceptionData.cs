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
        private List<Socket> socket_List;
        private static string journal;
        public NetworkReceptionData()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 40400);
            socket_List = new List<Socket>();
            server_Socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server_Socket.Bind(ip);
            server_Socket.Listen(100);
            journal = "";
        }

        public int get_Socket_Linker_Count()
        {
            return socket_List.ToArray().Length;
        }
        public void clear_Unused_Socket()
        {
            int count = socket_List.ToArray().Length;
            for (int i = 0; i < count; i++)
            {
                Socket s = socket_List[i];
                if (!s.Connected)
                {
                    socket_List.RemoveAt(socket_List.IndexOf(s));
                }
            }
        }

        public string get_Socket_Linkers_IP()
        {
            string ip = "";
            foreach (Socket s in socket_List)
            {
                ip += s.RemoteEndPoint.ToString() + "#";
            }
            if (ip.Length > 0)
            {
                ip = ip.Substring(0, ip.Length - 1);
            }

            return ip;
        }

        public string get_Journal()
        {
            string mess = journal;
            journal = "";
            return mess;
        }

        public void start_Listen_From_Client()
        {
            while (true)
            {
                try
                {
                    var accept = server_Socket.Accept();
                    socket_List.Add(accept);
                    journal += "\n"+DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:";
                    journal += "Connected\n";
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        while (true)
                        {
                            try
                            {
                                byte[] receive = new byte[1024];
                                int len = accept.Receive(receive);
                                string control_Code = System.Text.Encoding.UTF8.GetString(receive.Take(len).ToArray());
                                journal += DateTime.Now.ToString() + ":";
                                journal += accept.RemoteEndPoint.ToString() + ",Action:";
                                if (control_Code.Equals("REG"))
                                {
                                    journal += "REG\n";
                                    REG(accept);
                                }
                                else if (control_Code.Equals("SIN"))
                                {
                                    journal += "SIN\n";
                                    SIN(accept);
                                }
                                else if (control_Code.Equals("DSW"))
                                {
                                    journal += "DSW\n";
                                    DSW(accept);
                                }
                                else if (control_Code.Equals("DSR"))
                                {
                                    journal += "DSR\n";
                                    DSR(accept);
                                }
                                else if (control_Code.Equals("RSW"))
                                {
                                    journal += "RSW\n";
                                    RSW(accept);
                                }
                                else if (control_Code.Equals("RSRA"))
                                {
                                    journal += "RSRA\n";
                                    RSRA(accept);
                                }
                                else if (control_Code.Equals("DSD"))
                                {
                                    journal += "DSD\n";
                                    DSD(accept);
                                }
                                else if (control_Code.Equals("RSD"))
                                {
                                    journal += "RSD\n";
                                    RSD(accept);
                                }
                                else if (control_Code.Equals("END"))
                                {

                                    journal += "Connection Closed\n";
                                    accept.Shutdown(SocketShutdown.Both);
                                    accept.Close();
                                }
                                journal += "\n";
                            }
                            catch (Exception e)
                            {

                            }
                            finally
                            {

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
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '"+back_Code+"'\n";
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            mySQLC.delete_Result_Surface_Data(file_Name, data_Set_Serial);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";

        }


        public static void DSD(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            mySQLC.delete_Data_Set_Data(file_Name, data_Set_Serial);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
        }

        public static void RSRA(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            int len = 0;
            using (MySqlDataReader reader = mySQLC.get_Result_Surface_Reader())
            {
                accept.Send(Encoding.UTF8.GetBytes("OK"));
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
                len = accept.Receive(receive);
                clear_Receive = receive.Take(len).ToArray();
                back_Code = Encoding.UTF8.GetString(clear_Receive);
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
                if (back_Code.Equals("OK"))
                {
                    while (reader.Read())
                    {
                        string temp_File_Name = reader.GetString("FileName");
                        int temp_Data_Set_Serial = Convert.ToInt32(reader.GetString("DataSetSerial"));
                        int temp_Item_Count = Convert.ToInt32(reader.GetString("ItemCount"));
                        int temp_Cubage = Convert.ToInt32(reader.GetString("Cubage"));
                        int temp_Max_Result = Convert.ToInt32(reader.GetString("MaxResult"));
                        string send_Message = temp_File_Name + "#";
                        send_Message += temp_Data_Set_Serial + "#";
                        send_Message += temp_Item_Count.ToString() + "#";
                        send_Message += temp_Cubage.ToString() + "#";
                        send_Message += temp_Max_Result.ToString();
                        accept.Send(Encoding.UTF8.GetBytes(send_Message));
                        journal += "\n" + DateTime.Now.ToString() + ":";
                        journal += accept.RemoteEndPoint.ToString() + ",Action:Send '" + send_Message + "'\n";
                        len = accept.Receive(receive);
                        clear_Receive = receive.Take(len).ToArray();
                        back_Code = Encoding.UTF8.GetString(clear_Receive);
                        journal += "\n" + DateTime.Now.ToString() + ":";
                        journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
                        if (back_Code.Equals("NO"))
                        {
                            return;
                        }

                    }
                    accept.Send(Encoding.UTF8.GetBytes("NO"));
                }




            }
        }

        public static void RSR(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
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
                    journal += "\n" + DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
                    len = accept.Receive(receive);
                    clear_Receive = receive.Take(len).ToArray();
                    back_Code = Encoding.UTF8.GetString(clear_Receive);
                    journal += "\n" + DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
                    if (back_Code.Equals("OK"))
                    {
                        int temp_Item_Count = Convert.ToInt32(reader.GetString("ItemCount"));
                        int temp_Cubage = Convert.ToInt32(reader.GetString("Cubage"));
                        int temp_Max_Result = Convert.ToInt32(reader.GetString("MaxResult"));
                        string send_Message = temp_Item_Count.ToString() + "#";
                        send_Message += temp_Cubage.ToString() + "#";
                        send_Message += temp_Max_Result.ToString();
                        accept.Send(Encoding.UTF8.GetBytes(send_Message));
                        journal += "\n" + DateTime.Now.ToString() + ":";
                        journal += accept.RemoteEndPoint.ToString() + ",Action:Send '" + send_Message + "'\n";
                    }
                }
                else
                {
                    accept.Send(Encoding.UTF8.GetBytes("NO"));
                    journal += "\n" + DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'NO'\n";
                }






            }
        }


        public string[] get_Result_Items()
        {
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            return mySQLC.get_All_Result_Str().Split("@");
        }


        public static void RSW(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            int data_Count = 0;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            string[] tmp = back_Code.Split("#");
            string temp_Name = tmp[0];
            int temp_Data_Set_Serial = Convert.ToInt32(tmp[1]);
            int temp_Item_Count = Convert.ToInt32(tmp[2]);
            int temp_Cubage = Convert.ToInt32(tmp[3]);
            int temp_Max_Result = Convert.ToInt32(tmp[4]);
            try
            {
                mySQLC.write_To_resultsurface(temp_Name, temp_Data_Set_Serial, temp_Item_Count, temp_Cubage, temp_Max_Result);
            }catch(Exception e)
            {

            }
            accept.Send(Encoding.UTF8.GetBytes("OK"));

            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
        }



        public static void DSR(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            string[] tmp = back_Code.Split("#");
            string file_Name = tmp[0];
            int data_Set_Serial = Convert.ToInt32(tmp[1]);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            if (back_Code.Equals("OK"))
            {
                using (MySqlDataReader reader = mySQLC.get_Data_Set_Reader(file_Name, data_Set_Serial))
                {
                    while (reader.Read() && back_Code.Equals("OK"))
                    {
                        string send = reader.GetString("ItemSetSerial") + "#";
                        send += reader.GetString("Profit") + "#";
                        send += reader.GetString("Weight") + "#";
                        send += reader.GetString("Radio");
                        accept.Send(Encoding.UTF8.GetBytes(send));
                        journal += "\n" + DateTime.Now.ToString() + ":";
                        journal += accept.RemoteEndPoint.ToString() + ",Action:Send '"+send+"'\n";
                        len = accept.Receive(receive);
                        clear_Receive = receive.Take(len).ToArray();
                        back_Code = Encoding.UTF8.GetString(clear_Receive);
                        journal += "\n" + DateTime.Now.ToString() + ":";
                        journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
                    }
                    accept.Send(Encoding.UTF8.GetBytes("NO"));
                    journal += "\n" + DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'NO'\n";
                }
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
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            data_Count = Convert.ToInt32(back_Code);
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            for (int i = 0; i < data_Count; i++)
            {
                len = accept.Receive(receive);
                clear_Receive = receive.Take(len).ToArray();
                back_Code = Encoding.UTF8.GetString(clear_Receive);
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
                string[] tmp = back_Code.Split("#");
                string temp_Name = tmp[0];
                int temp_Data_Set_Serial = Convert.ToInt32(tmp[1]);
                int temp_Item_Set_Serial = Convert.ToInt32(tmp[2]);
                int temp_Profit = Convert.ToInt32(tmp[3]);
                int temp_Weight = Convert.ToInt32(tmp[4]);
                decimal temp_Radio = Convert.ToDecimal(tmp[5]);
                try
                {
                    mySQLC.write_To_Data_Set_DB(temp_Name, temp_Data_Set_Serial, temp_Item_Set_Serial, temp_Profit, temp_Weight, temp_Radio);

                }
                finally
                {
                    accept.Send(Encoding.UTF8.GetBytes("OK"));
                    journal += "\n" + DateTime.Now.ToString() + ":";
                    journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
                }


            }

        }

        public static void SIN(Socket accept)
        {
            byte[] receive = new byte[1024];
            byte[] clear_Receive;
            string back_Code = "";
            MySQLConnectionClass mySQLC = new MySQLConnectionClass();
            accept.Send(Encoding.UTF8.GetBytes("OK"));
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);
            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            string[] tmp = back_Code.Split("#");
            int user_Id = Convert.ToInt32(tmp[0]);
            bool is_Password = mySQLC.is_Password(user_Id, tmp[1]);
            if (is_Password)
            {
                accept.Send(Encoding.UTF8.GetBytes("OK"));
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            }
            else
            {
                accept.Send(Encoding.UTF8.GetBytes("NO"));
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'NO'\n";
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
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            int len = accept.Receive(receive);

            clear_Receive = receive.Take(len).ToArray();
            back_Code = Encoding.UTF8.GetString(clear_Receive);
            journal += "\n" + DateTime.Now.ToString() + ":";
            journal += accept.RemoteEndPoint.ToString() + ",Action:Receive '" + back_Code + "'\n";
            string[] tmp = back_Code.Split("#");
            string temp_Name = tmp[0];
            int temp_ID = Convert.ToInt32(tmp[1]);
            string temp_Password = tmp[2];

            bool have = mySQLC.search_User(temp_ID);
            if (have)
            {
                accept.Send(Encoding.UTF8.GetBytes("NO"));

                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'NO'\n";
                return;
            }
            else
            {
                mySQLC.write_To_usertable(temp_Name, temp_ID, temp_Password);
                accept.Send(Encoding.UTF8.GetBytes("OK"));
                journal += "\n" + DateTime.Now.ToString() + ":";
                journal += accept.RemoteEndPoint.ToString() + ",Action:Send 'OK'\n";
            }
            mySQLC.stop_Connect_To_DB();

        }
    }

}
