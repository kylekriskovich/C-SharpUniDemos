using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using APIClasses;
using Tutorial_4.Models;


namespace Tutorial_4.Controllers
{
    
    public class CommentController : ApiController
    {


        [Route("api/Comment")]
        [HttpGet]
        //gets a list of all comments
        public List<Comment> GetComments()
        {
            List<List<string>> allComments = Database.GetComments();
            List<Comment> comments = new List<Comment>();
            foreach (List<string> com in allComments)
            {
                Comment c = new Comment();
                c.Username = com[0];
                c.Content = com[1];
                comments.Add(c);
            }
            return comments;
        }



        [Route("api/Comment")]
        [HttpPost]
        //Adds a comment to the database if the User that placed it is contained within the database
        public void CreateComment(Comment inComm)
        {
            List<string> users = Database.GetUsers();

            if(users.Contains(inComm.Username))
            {
                Database.CreateComment(inComm.Username, inComm.Content);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotModified);
            }
        }


        [Route("api/Comment/Search/{username}")]
        [HttpGet]
        // Searches the database for comments by a certain username
        public List<Comment> search(string username)
        {
            List<List<string>> comments = Database.GetComments();
            List<Comment> usersComments = new List<Comment>();

            foreach (List<string> com in comments)
            {
                if(com[0].Equals(username))
                {
                    Comment c = new Comment();
                    c.Username = com[0];
                    c.Content = com[1];
                    usersComments.Add(c);
                }
            }

            return usersComments;
        }


    }
}