using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXSApplication.Models
{
    public class Account
    {
        public String Username;
        public String Password;
        public String FirstName;
        public String LastName;
        public String Card;
        public String ExpireDate;
        public int SecurityNum;

        public Account()
        {
            Username = "";
            Password = "";
            FirstName = "";
            LastName = "";
            
        }

        public Account(String usr,String pass, String fname, String lname, String cardnum, String edate, int sec)
        {
            Username = usr;
            Password = pass;
            FirstName = fname;
            LastName = lname;
            Card = cardnum;
            ExpireDate = edate;
            SecurityNum = sec;
        }

    }
}