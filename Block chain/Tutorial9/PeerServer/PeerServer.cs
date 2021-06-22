using DetailClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DetailClasses;

namespace ClientApplication
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    internal class PeerServer : PeerInterface
    {
        public PeerServer()
        {
            Console.WriteLine("Connected to dataserver");
        }

        public bool jobExists()
        {
            
        }

        public String recieveJob(ClientDetails inClient)
        {
            
        }

        public void uploadSolution(string inSolution)
        {
            
        }


    }
}
