using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XXSApplication.Models;
using Newtonsoft.Json;
using RestSharp;
using dataclasses;
using System.Web;


namespace XXSApplication.Controllers
{
    public class CommentController : ApiController
    {
        [Route("api/Comment")]
        [HttpGet]
        public List<Comment> LoadComments()
        {
            List<Comment> coms = Database.GetComments();
            return coms;
           
        }
        [Route("api/Comment/Safe")]
        [HttpGet]
        public List<Comment> LoadCommentsSafe()
        {
            List<Comment> coms = Database.GetCommentsSafe();
            return coms;

        }

        [Route("api/Comment")]
        [HttpPost]
        public void CreateComment(Comment inCom)
        {
            Database.PostComment(inCom);
        }

        [Route("api/Comment/Safe")]
        [HttpPost]
        public void SafeComment(Comment inCom)
        {
            Database.PostCommentSafe(inCom);
        }
    }
}
