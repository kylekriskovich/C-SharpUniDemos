using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DetailClasses;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Web.UI;
using System.ServiceModel;
using System.Web.UI.WebControls.WebParts;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace ClientApplication.Network
{
    public class NetworkThread
    { 
        private bool alive = false;
        private List<ClientDetails> clientList;
        private ClientDetails thisClient;
        private ClientInstance client;
        public NetworkThread(ClientInstance inClient)
        {
            //the loop
            client = inClient;
            thisClient = client.getInstance();

        }

        public void start()
        {
            // run loop 
            // keep it running 
        }

        public void close()
        {
            alive = false;
        }
        public void open()
        {
            try
            {
                alive = true;
                registerClient();

                while (alive)
                {
                    findClient();
                    CheckClientforJob();
                    Thread.Sleep(5000);
                }
            }catch(ArgumentOutOfRangeException e)
            {
                printLine("NetworkThread.open " + e.Message);
            }
            

        }

        public void findClient()
        {
            try
            {
                String URL = "https://localhost:44309/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Client");
                IRestResponse resp = client.Get(request);
                clientList = JsonConvert.DeserializeObject<List<ClientDetails>>(resp.Content);
            }catch(ArgumentException e)
            {
                printLine("NetworkThread.findClient " + e.Message);
            }
            
        }

        public void CheckClientforJob()
        {
            try
            {
                bool found = false;
                ClientDetails worker = new ClientDetails();
                foreach (ClientDetails client in clientList)
                {

                    if (!(client.port.Equals(thisClient.port) && client.ipAddress.Equals(thisClient.ipAddress)))
                    {
                        printLine(String.Format("Found client with the ip:{0} and a port: {1}", client.ipAddress, client.port));
                        PeerInterface connection;
                        ChannelFactory<PeerInterface> foobFactory;
                        NetTcpBinding tcp = new NetTcpBinding();
                        string url = "net.tcp://" + client.ipAddress.ToString() + ":" + client.port.ToString() + "/DataService";
                        foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                        connection = foobFactory.CreateChannel();
                        found = connection.CheckforAvailableJob();

                        if (found)
                        {
                            printLine("Found avaliable job starting Execution");
                            //worker.ipAddress = client.ipAddress;
                            //worker.port = client.port;
                            //DoJob(worker);
                            DownloadJob(client);
                        }

                    }


                }
            }
            catch (ArgumentNullException e)
            {
                printLine("NetworkThread.checkClientForJob " + e.Message);
            }
            catch (FormatException e)
            {
                printLine("NetworkThread.checkClientForJob " + e.Message);
            }
            catch (ArgumentException e)
            {
                printLine("NetworkThread.checkClientForJob " + e.Message);
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                printLine("NetworkThread.checkClientForJob " + e.Message);
            }

        }

        public void DownloadJob(ClientDetails inClient)
        {
            try
            {
                String jobDetails;
                PeerInterface connection;
                ChannelFactory<PeerInterface> foobFactory;
                NetTcpBinding tcp = new NetTcpBinding();
                string url = "net.tcp://" + inClient.ipAddress.ToString() + ":" + inClient.port.ToString() + "/DataService";
                foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                connection = foobFactory.CreateChannel();
                jobDetails = connection.DownloadJob();

                //printLine(String.Format("Executing Job:[{0}]",jobDetails)); //Executing the job
                String result = ExecuteJob(DecodeString(jobDetails));
                if(result== null)
                {
                    result = "Failed please retry";
                    result = EncodeString(result);
                }
                connection.uploadSolution(result);
            }
            catch (ArgumentNullException e)
            {
                printLine("NetworkThread.downloadJob " + e.Message);
            }


        }

        public void registerClient()
        {
            try
            {
                String URL = "https://localhost:44309/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Client");
                request.AddJsonBody(thisClient);
                IRestResponse resp = client.Post(request);
            }
            catch (ArgumentException e)
            {
                printLine("NetworkThread.registerClient" + e.Message);
            }


        }

        public void printLine(String inString)
        {
            Console.WriteLine("\t\t"+thisClient.ipAddress.ToString() + ":" + thisClient.port.ToString() + ":\t" + inString);
        }

        public String ExecuteJob(String inJob)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                var result = engine.Execute(inJob, scope);
                String resultString = result.ToString();
                return EncodeString(resultString);
            }
            catch(ArgumentNullException e)
            {
                printLine("NetworkThread.ExecuteJob" + e.Message);
                return null;
            }
            catch (NotSupportedException e)
            {
                printLine("NetworkThread.ExecuteJob" + e.Message);
                return null;
            }
            catch (Microsoft.Scripting.SyntaxErrorException e)
            {
                printLine("NetworkThread.ExecuteJob" + e.Message);
                return null;
            }
        }

        /*
        * 	Purpose: Encodes a String into base 64
        *	Input: string to output
        *	Output: base64string
        */
        private String EncodeString(String inString)
        {
            try
            {
                if (String.IsNullOrEmpty(inString))
                {
                    return null;
                }
                byte[] textbytes = Encoding.UTF8.GetBytes(inString);
                return Convert.ToBase64String(textbytes);
            }
            catch (EncoderFallbackException e)
            {
               printLine("NetworkThread.EncodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                printLine("NetworkThread.EncodeString " + e.Message);
            }
            catch (ArgumentException e)
            {
                printLine("NetworkThread.EncodeString " + e.Message);
            }
            catch (FormatException e)
            {
                printLine("NetworkThread.EncodeString " + e.Message);
            }

            return null;
        }

        /*
        * 	Purpose: Dencodes a base 64String into string
        *	Input: base64string to output
        *	Output: string
        */
        private String DecodeString(String in64String)
        {
            try
            {
                if (String.IsNullOrEmpty(in64String))
                {
                    return null;
                }
                byte[] codebytes = Convert.FromBase64String(in64String);
                return Encoding.UTF8.GetString(codebytes);
            }
            catch (DecoderFallbackException e)
            {
                printLine("NetworkThread.DecodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                printLine("NetworkThread.DecodeString " + e.Message);
            }
            catch (ArgumentException e)
            {
                printLine("NetworkThread.DecodeString " + e.Message);
            }
            catch (FormatException e)
            {
                printLine("NetworkThread.DecodeString " + e.Message);
            }
            return null;

        }

    }
}
