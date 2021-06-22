using DetailClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class PeerServer : PeerInterface
    {
        private ClientInstance client;
        public PeerServer(ClientInstance inClient)
        {
            client = inClient;
        }

        public bool CheckforAvailableJob()
        {
            return client.checkForJob();
        }

        public void CreateJob(string inJob)
        {
            client.setJob(inJob);
        }

        public string DownloadJob()
        {
            return client.getJob();
        }

        public void uploadSolution(string inSolution)
        {
            client.completed(inSolution);
        }
    }
}
