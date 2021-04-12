using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_Server_Gui
{
    class MySQLConnectionClass
    {
        private string connect_Str;
        private MySqlConnection connection_To_DB;
        public MySQLConnectionClass()
        {
            //connect_Str = init_Str;
            connect_Str = "server=127.0.0.1;port=3306;user=root;password=; database=PersonalProject2;";
            connection_To_DB = new MySqlConnection(connect_Str);
            try
            {
                connection_To_DB.Open();
                Console.WriteLine("已经建立连接");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void stop_Connect_To_DB()
        {
            try
            {
                connection_To_DB.Close();
                Console.WriteLine("已经关闭连接");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void delete_Result_Surface_Data(string fn, int dss)
        {
            string sql = "DELETE  FROM 	resultsurface WHERE FileName = '" + fn.ToString() + "' && DataSetSerial= " + dss.ToString() + ";";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            int result = cmd.ExecuteNonQuery();
        }



        public void delete_Data_Set_Data(string fn,int dss)
        {
            string sql = "DELETE  FROM 	datasetdatabase WHERE FileName = '" + fn.ToString() + "' &&DataSetSerial= " + dss.ToString() + ";";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            int result = cmd.ExecuteNonQuery();
        }

        public MySqlDataReader get_Result_Surface_Reader(string fn,int dss)
        {
            string sql = "SELECT * FROM 	resultsurface WHERE FileName = '" + fn.ToString() + "' &&DataSetSerial= " + dss.ToString() + ";";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            return cmd.ExecuteReader();
        }

        public MySqlDataReader get_Result_Surface_Reader()
        {
            string sql = "SELECT * FROM 	resultsurface" + ";";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            return cmd.ExecuteReader();
        }


        public void write_To_resultsurface(string fn, int dss, int ic, int c, int mr)
        {
            string sql = "insert into resultsurface(FileName,DataSetSerial,ItemCount,Cubage,MaxResult) values('"
                + fn + "'," + dss.ToString() + "," + ic.ToString() + "," + c.ToString() + "," + mr.ToString() + ");";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            int result = cmd.ExecuteNonQuery();
        }

        public void write_To_Data_Set_DB(string fn,int dss,int iss,int p,int w,decimal r)
        {
            try
            {
                string sql = "insert into datasetdatabase(FileName,DataSetSerial,ItemSetSerial,Profit,Weight,Radio) values('"
               + fn + "'," + dss.ToString() + "," + iss.ToString() + "," + p.ToString() + "," + w.ToString() + "," + r.ToString() + ")";
                MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
                int result = cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {

            }
            finally
            {

            }
           
        }

        public void write_To_usertable(string user_Name,int user_ID,string user_Password)
        {
            string sql = "insert into usertable(UserName,UserId,UserPassword) values('" + user_Name+"',"+user_ID.ToString()+",'"+user_Password+ "')";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            int result = cmd.ExecuteNonQuery();
        }


        public MySqlDataReader get_Data_Set_Reader(string fn,int serial)
        {
            string sql = "SELECT * FROM 	datasetdatabase WHERE FileName = '" + fn.ToString() + "' &&DataSetSerial= '"+serial.ToString()+"';";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            return cmd.ExecuteReader();
        }

        public bool search_User(int id)
        {
            string sql = "SELECT * FROM 	usertable WHERE UserId = '" + id.ToString() + "';";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            using (MySqlDataReader reader = cmd.ExecuteReader()) {

                if (!reader.Read())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string get_All_Result_Str()
        {
            string sql = "SELECT * FROM 	resultsurface";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            string result = "";
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    string temp_File_Name = reader.GetString("FileName");
                    string temp_Data_Set_Serial = reader.GetString("DataSetSerial");
                    string temp_Item_Count = reader.GetString("ItemCount");
                    string temp_Cubage = reader.GetString("Cubage");
                    string temp_Max_Result = reader.GetString("MaxResult");
                    result += temp_File_Name + "#" + temp_Data_Set_Serial + "#" + temp_Item_Count + "#" + temp_Cubage + "#" + temp_Max_Result+"@";
                }
                result = result.Substring(0, result.Length - 1);
            }
            return result;



        }


        public bool is_Password(int id,string password)
        {
            string sql = "SELECT * FROM 	usertable WHERE UserId = '" + id.ToString() + "';";
            MySqlCommand cmd = new MySqlCommand(sql, connection_To_DB);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                if (!reader.Read())
                {
                    return false;
                }
                else
                {
                    string user_Password = reader.GetString("UserPassword");
                    if (password.Equals(user_Password))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }
}
