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
        public PeerServer()
        {

        }

        public Block getCurrentBlock()
        {
            return ClientInstance.GetRecentBlock();
        }

        public List<Block> GetCurrentChain()
        {
            return ClientInstance.GetBlockchain().getBlockchain();
        }

        public void RecieveTransation(string inTrans)
        {
            ClientInstance.addTransaction(inTrans);
        }

        public void SetBlockchain(Blockchain bc)//Don't know if i need it
        {
            ClientInstance.SetBlockchain(bc);
        }
    }
}
