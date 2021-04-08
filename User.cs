using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_Server_Gui
{
    class User
    {
        private string user_Name;
        private int user_ID;
        private string user_Password;
        public User(string name,int id,string password)
        {
            user_Name = name;
            user_ID = id;
            user_Password = password;
        } 

        public void set_Name(string name)
        {
            user_Name = name;
        }

        public string get_Name()
        {
            return user_Name;
        }

        public void set_Password(string new_Password)
        {
            user_Password = new_Password;
        }

        public bool is_Password(string password)
        {
            return string.Equals(user_Password, password);
        }

    }
}
