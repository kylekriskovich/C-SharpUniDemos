using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial_4.Models
{
    public class Database
    {
        private static ExamLib.CommentDatabase commentDatabase;

        public Database()
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }
        }

        public static void AddUser(string username, string password)
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }

            commentDatabase.AddUser(username, password); 
        }

        public static bool LoginUser(string username, string password)
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }
            
            return commentDatabase.LoginUser(username, password);
            
            
        }

        public static List<string>  GetUsers()
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }
            return commentDatabase.GetUsers();
        }

        public static List<List<string>> GetComments()
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }
            return commentDatabase.GetComments();
        }

        public static void CreateComment(string username, string comment)
        {
            if (commentDatabase == null)
            {
                commentDatabase = new ExamLib.CommentDatabase();
            }
            commentDatabase.CreateComment(username, comment);
        }
    }
}