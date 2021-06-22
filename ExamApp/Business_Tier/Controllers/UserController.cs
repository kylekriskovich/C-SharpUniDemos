using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using APIClasses;
using Tutorial_4.Models;
//using Tutorial_4.Filters;

namespace Tutorial_4.Controllers
{
    //[UserExceptionFilter]
    public class UserController : ApiController
    {
 

        [Route("api/User")]
        [HttpGet]
        // Retrieves list of users
        public List<string> GetUsers()
        {
            return Database.GetUsers();   
        }

      

        [Route("api/User")]
        [HttpPost]
        public void AddUser(UserDetails inUser)
        {
            Database.AddUser(inUser.Username, inUser.Password);   
        }

        [Route("api/User/Login")]
        [HttpPost]
        // Trys to log the user in and throws a httpresponse NotFound if it can't
        public bool Login(UserDetails inUser)
        {
            try
            {
                return Database.LoginUser(inUser.Username, inUser.Password);
            }catch(ExamLib.NoUserException e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            
        }


    }
}