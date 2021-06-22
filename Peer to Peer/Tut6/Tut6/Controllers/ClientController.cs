using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using DetailClasses;
using WebServer.Models;
using System.Net;

namespace WebServer.Controllers
{
    public class ClientController : ApiController
    {
        private static Server server = Singleton.getServer();

        [Route("api/Client")]
        [HttpGet]
        public List<ClientDetails> GetClients()
        {
            return server.getAllClients();
        }

        [Route("api/Client")]
        [HttpPost]
        public void RegisterClient(ClientDetails inClient)
        {
            try
            {
                server.addClient(inClient);

            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

    }
       
}