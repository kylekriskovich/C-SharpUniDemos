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
        String DownloadJob();

        [OperationContract]
        void uploadSolution(String inSolution);

        [OperationContract]
        Boolean CheckforAvailableJob();

        [OperationContract]
        void CreateJob(String inJob);

    }
}
