using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DetailClasses;

namespace WebServer.Models
{
    public class Server
    {
        private List<ClientDetails> clientList = new List<ClientDetails>();

        public Server()
        {

        }

        public void addClient(ClientDetails inClient)
        {
            if(inClient.ipAddress.Equals(null) || inClient.port.Equals(null))
            {
                const string Message = "Client could not be added as a containing values were NULL";
                throw new NullReferenceException(Message);
            }
            else if (inClient.ipAddress.Equals("") || inClient.port.Equals(""))
            {
                
            }
            else
            {
                clientList.Add(inClient);
            }
        }


        public List<ClientDetails> getAllClients()
        {
            return clientList;
        }

        public void removeClient(ClientDetails inClient)
        {
            clientList.Remove(inClient);
        }


    }
}