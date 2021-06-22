using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DetailClasses;
using System.ServiceModel;
using System.Security.Policy;

namespace ClientApplication.Server
{
    public class ServerThread
    {
        //Does the whole creating shiz
        private bool alive = false;
        private ClientDetails thisClient;
        private ClientInstance client;
        private String URL;

        public ServerThread(ClientInstance inClient)
        {
            client = inClient;
            thisClient = client.getInstance();
            URL = String.Format("net.tcp://{0}:{1}/DataService", thisClient.ipAddress.ToString(), thisClient.port.ToString());
        }

        public void start()
        {
            alive = true;
            try
            {
                printLine("Launching Peer Dataserver");

                ServiceHost Host;

                NetTcpBinding tcp = new NetTcpBinding();

                PeerServer p = new PeerServer(client);

                Host = new ServiceHost(p);

                Host.AddServiceEndpoint(typeof(PeerInterface), tcp, URL);

                Host.Open();

                printLine("Open and running on " + URL);

                while(alive)
                {
                    
                }
                

                Host.Close();

            }catch(Exception e)
            {
                printLine(e.Message);
            }

            /*for (int ii = 0; ii < 30; ii++)
            {
                if(ii%3 != 0)
                {
                    Console.WriteLine("\n" + ii.ToString() + " Seconds have past");
                }
                Thread.Sleep(100);
            }*/
            //Testing if the Thread works



        }

        public void close()
        {
            alive = false;
        }

        public void printLine(String inString)
        {
            Console.WriteLine("\t\t"+thisClient.ipAddress.ToString() + ":" + thisClient.port.ToString() + ":\t" + inString);
        }
    }
}
