using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DetailClasses;


namespace ClientApplication
{
    [ServiceContract]
    public interface PeerInterface
    {
        [OperationContract]
        List<Block> GetCurrentChain();

        [OperationContract]
        Block getCurrentBlock();

        [OperationContract]
        void RecieveTransation(string inTrans);

        [OperationContract]
        void SetBlockchain(Blockchain inChain);

    }
}
