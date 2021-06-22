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
    public class AccountController : ApiController
    {
        [Route("api/Account/Login/{login}")]
        [HttpGet]
        public AccountDetails loadAccount(String login)
        {
            AccountDetails acc = Database.GetAccount(login);

            if(acc == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("doesn't exist", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                //throw new HttpResponseException(response);
            }

            return acc;
           
        }

        [Route("api/Account/Bang")]
        [HttpGet]
        public String Bang()
        {
            return "Bang";
        }
    }
}
