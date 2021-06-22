using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using dataclasses;

namespace XXSApplication.Models
{
    public class Database
    {
        private static Dictionary<String,Account> accountMap;
        private static List<Comment> comments;

        public Database()
        {
            if (accountMap == null)
            {
                accountMap = new Dictionary<string, Account>();
                BuildDefault();
            }
        }

        public static Dictionary<String, Account> AccountsDB()
        {
            if (accountMap == null)
            {
                accountMap = new Dictionary<string, Account>();
                BuildDefault();
            }
            return accountMap;
        }

        public static List<Comment> GetCommentsSafe()
        {
            List<Comment> list = new List<Comment>();
            if (comments == null)
            {
                comments = new List<Comment>();

                BuildDefault();             
            }
            foreach (Comment c in comments)
            {
                Comment com = new Comment();
                com.comment = CleanString(c.comment);
                com.Username = CleanString(c.Username);
                list.Add(com);
            }
            return list;
        }

        public static void PostComment(Comment inCom)
        {
            if (comments == null)
            {
                comments = new List<Comment>();
                BuildDefault();
            }
            comments.Add(inCom);
        }

        public static void PostCommentSafe(Comment inCom)
        {
            if (comments == null)
            {
                comments = new List<Comment>();
                BuildDefault();
            }
            inCom.Username = CleanString(inCom.Username);
            inCom.comment = CleanString(inCom.comment);
            comments.Add(inCom);
        }

        public static string CleanString(string inString)
        {
            StringBuilder output = new StringBuilder();
            foreach (char c in inString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c==' ')
                {
                    output.Append(c);
                }
            }
            return output.ToString();
        }

        public static List<Comment> GetComments()
        {
            if (comments == null)
            {
                comments = new List<Comment>();
                BuildDefault();
            }

            return comments;
        }

        public static AccountDetails GetAccount(String login)
        {

            if (accountMap == null)
            {
                accountMap = new Dictionary<string, Account>();
                BuildDefault();
            }
            Account outAcc = null;
            AccountDetails accountJson = null;
            CardDetails cardJson = new CardDetails();
            
                accountMap.TryGetValue(login, out outAcc);
                if (outAcc != null)
                {
                    accountJson = new AccountDetails();
                    accountJson.FirstName = outAcc.FirstName;
                    accountJson.LastName = outAcc.LastName;
                    accountJson.Username = outAcc.Username;
                    accountJson.Password = outAcc.Password;

                    cardJson.CardNumber = outAcc.Card;
                    cardJson.ExpireDate = outAcc.ExpireDate;
                    cardJson.SecurityNum = outAcc.SecurityNum;
                    accountJson.card = cardJson;
                    return accountJson;
                }
            
            

            return null;

        }


        private static void BuildDefault()
        {
            if (comments == null)
            {
                comments = new List<Comment>(); 
            }
            if (accountMap == null)
            {
                accountMap = new Dictionary<string, Account>();
            }

            Account ac1 = new Account("Ch0s3n1","Padme","Anakin","Skywalker", "1111-2222-3333-4444", "05/21", 405);
            Account ac2 = new Account("St1lL_10","IChooseYou","Ash","Ketchum", "2222-3333-4444-5555", "02/22", 234);
            Account ac3 = new Account("pHR0d0","MyFellowship", "Frodo","Baggins", "3333-4444-5555-6666", "03/23", 345);
            Account ac4 = new Account("n0T_1R0N_mAN","Tony123", "Tony", "Stark", "4444-5555-6666-7777", "04/24", 456);
            Account ac5 = new Account("W4tCH3R","Ghost", "Jon", "Snow", "5555-6666-7777-8888", "05/25", 567);
            

            accountMap.Add("Ch0s3n1Padme",ac1);
            accountMap.Add("St1lL_10IChooseYou", ac2);
            accountMap.Add("pHR0d0MyFellowship", ac3);
            accountMap.Add("n0T_1R0N_mANTony123", ac4);
            accountMap.Add("W4tCH3RGhost", ac5);

            Comment c1 = new Comment();
            c1.Username = ac1.Username;
            c1.comment = "I have the high ground now!";

            Comment c2 = new Comment();
            c2.Username = ac2.Username;
            c2.comment = "FIRST";

            Comment c3 = new Comment();
            c3.Username = ac3.Username;
            c3.comment = "One does not simply comment";

            Comment c4 = new Comment();
            c4.Username = ac5.Username;
            c4.comment = "I Know Nothing";

            comments.Add(c2);
            comments.Add(c1);
            comments.Add(c3);
            comments.Add(c4);

        }
    }
}