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
using System.Diagnostics;

namespace ClientApplication.Mining
{
    public class MiningThread
    { 
        private bool alive = false;
        private List<ClientDetails> clientList;
        private ClientDetails thisClient;
        private Blockchain blockchain;
        List<string> transactions;
        List<string> solved;
        List<Transaction> completedJobs;
        

        public MiningThread(ClientDetails inClient)
        {

            thisClient = inClient;

        }

        public void close()
        {
            alive = false;
        }

        /*
        * 	Purpose: Creates the loop that continues to run in the Mining thread
        *	Input:  nothing
        *	Output: nothing
        */
        public void open()
        {
            try
            {
                alive = true;
                registerClient();
                clientList = findClient();
                printLine("Client count is:" + clientList.Count().ToString());

                blockchain = ClientInstance.GetBlockchain();
                transactions = ClientInstance.GetTransaction();

                //If the client is late to the blockchain it gets the current chain
                if (clientList.Count() > 1)
                {
                    blockchain = LateClient(thisClient);
                    ClientInstance.SetBlockchain(blockchain);

                }

                //The action loop of the thread
                while (alive)
                {

                    //transactions = ClientInstance.GetTransaction();
                    if (transactions.Count() > 0)
                    {
                        foreach (string trans in transactions)
                        {
                            if (!ClientInstance.Solved(trans))
                            {
                                PostTransaction(trans);
                            }

                        }

                        //printLine("Tranasction Count: " + transactions.Count());
                        //If there is more 5 transactions waiting to be executed 
                        if ((transactions.Count() % 5) == 0)
                        {
                            List<string> toCompute = new List<string>();
                            completedJobs = new List<Transaction>();
                            int ii = 0;
                            foreach (string t in transactions) //find the transactions that need to be executed
                            {
                                if ((blockchain.Count() < ii + 1) && (transactions.Count() - ii) <= 5)
                                {
                                    //printLine("bc: " + blockchain.Count().ToString() + " ii: " + ii.ToString() + "tran count: " + (transactions.Count()-ii).ToString());
                                    Console.WriteLine(t);
                                    toCompute.Add(t);
                                }
                                ii++;
                            }
                            toCompute.Sort(); //Sort by Alphabetical Order performed on the Encoded strings

                            //Execute the transactions that are waiting to be created into blocks
                            foreach (string t in toCompute)
                            {
                                string answer = ExecuteJob(t);
                                string[] trans = { t, answer };

                                Block block = Miner.MineBlock(trans);
                                if (block != null)
                                {
                                    printLine("Block to Add: " + block.blockID.ToString() + " " + block.transactions + " offset:" + block.offset.ToString() + " prevHash:" + block.previousHash.ToString() + " hash:" + block.hash.ToString());
                                    blockchain.AddToChain(block);
                                    printLine("Chain Updated new Length: " + blockchain.Count().ToString());
                                    ClientInstance.SetBlockchain(blockchain);
                                    Transaction complete = new Transaction();
                                    complete.code = t;
                                    complete.answer = answer;
                                    completedJobs.Add(complete);
                                    ClientInstance.SetComplete(completedJobs);
                                }
                            }
                            toCompute.Clear();

                        }
                    }


                    //Find the most popular blockchain
                    if (clientList.Count() > 1)
                    {
                        int popularHash = getPopularHash();
                        if (popularHash != 0)
                        {
                            Blockchain popularChain = new Blockchain();
                            popularChain.ReplaceChain(getPopularChain(popularHash));
                            ClientInstance.SetBlockchain(popularChain);
                        }

                    }

                    printLine("Length of chain " + blockchain.Count().ToString());

                    Thread.Sleep(2000);
                    //Update the Important lists
                    clientList = findClient();
                    transactions = ClientInstance.GetTransaction();

                }
            }catch(ArgumentNullException e)
            {
                printLine("MiningThread.open " + e.Message);
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                printLine("MiningThread.open " + e.Message);
            }





        }

        /*
        * 	Purpose: Retrieves the most up to date Blockchain
        *	Input:  client details of the current clients port and ip
        *	Output: The most up to date blockchain
        */
        private Blockchain LateClient(ClientDetails inClient)
        {
            Blockchain bc = new Blockchain();
            foreach(ClientDetails CD in clientList)
            {
                if(!((inClient.ipAddress.Equals(CD.ipAddress)) && (inClient.port.Equals(CD.port))))
                {
                    try
                    {
                        PeerInterface connection;
                        ChannelFactory<PeerInterface> foobFactory;
                        NetTcpBinding tcp = new NetTcpBinding();
                        string url = "net.tcp://" + CD.ipAddress.ToString() + ":" + CD.port.ToString() + "/DataService";
                        foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                        connection = foobFactory.CreateChannel();
                        int length = connection.GetCurrentChain().Count() - 1; //removing the intial block from count
                        printLine("Receiving blockchain from IP: " + CD.ipAddress.ToString() + " Port: " + CD.port.ToString() + " of blockchain length: " + length.ToString());
                        bc.ReplaceChain(connection.GetCurrentChain());
                    }
                    catch (ArgumentNullException e)
                    {
                        printLine("MiningThread.LateClient " + e.Message);
                    }
                    catch (System.ServiceModel.EndpointNotFoundException e)
                    {
                        printLine("MiningThread.LateClient " + e.Message);
                    }

                }
            }

            return bc;    
        }

        /*
        * 	Purpose: Retrieves the most popular blockchain
        *	Input:  hash related to current block of the most popular blockchain
        *	Output: The most popular blockchain
        */
        private List<Block> getPopularChain(int inHash)
        {
            foreach (ClientDetails client in clientList)
            {
                try
                {
                    PeerInterface connection;
                    ChannelFactory<PeerInterface> foobFactory;
                    NetTcpBinding tcp = new NetTcpBinding();
                    string url = "net.tcp://" + client.ipAddress.ToString() + ":" + client.port.ToString() + "/DataService";
                    foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                    connection = foobFactory.CreateChannel();
                    Block block = connection.getCurrentBlock();
                    if (inHash == block.hash)
                    {
                        return connection.GetCurrentChain();
                    }
                }
                catch (ArgumentNullException e)
                {
                    printLine("MiningThread.getPopularChain " + e.Message);
                }
                catch (System.ServiceModel.EndpointNotFoundException e)
                {
                    printLine("MiningThread.getPopularChain " + e.Message);
                }

            }
            return ClientInstance.GetBlockchain().getBlockchain();
        }

        /*
        * 	Purpose: Rest api call to retrieve the current client list
        *	Input:  
        *	Output: list of clients
        */
        public List<ClientDetails> findClient()
        {
            try
            {
                String URL = "https://localhost:44351/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest("api/Client");
                IRestResponse resp = client.Get(request);
                return JsonConvert.DeserializeObject<List<ClientDetails>>(resp.Content);
            }
            catch(Newtonsoft.Json.JsonSerializationException e)
            {
                printLine("MiningThread.findClient " + e.Message);
                return null;
            }
        }


        /*
        * 	Purpose: Posts the next transactions to all other clients on the server
        *	Input:  the transaction to be posted
        *	Output: 
        */
        public void PostTransaction(string inTrans)
        {
            
            foreach (ClientDetails client in clientList)
            {
                try
                {
                    PeerInterface connection;
                    ChannelFactory<PeerInterface> foobFactory;
                    NetTcpBinding tcp = new NetTcpBinding();
                    string url = "net.tcp://" + client.ipAddress.ToString() + ":" + client.port.ToString() + "/DataService";
                    foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                    connection = foobFactory.CreateChannel();
                    connection.RecieveTransation(inTrans);
                }
                catch (ArgumentNullException e)
                {
                    printLine("MiningThread.PostTransactions " + e.Message);
                }
                catch (System.ServiceModel.EndpointNotFoundException e)
                {
                    printLine("MiningThread.PostTransactions " + e.Message);
                }

            }
        }

        /*
        * 	Purpose: Rest api call to register this client with the server
        *	Input:  
        *	Output: 
        */
        public void registerClient()
        {
            String URL = "https://localhost:44351/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/Client");
            request.AddJsonBody(thisClient);
            IRestResponse resp = client.Post(request);
            
        }

        /*
        * 	Purpose: Gets the hash of the current block in all clients blockchain and calculates the most frequent
        *	Input:  
        *	Output: 
        */
        public int getPopularHash()
        {
            try
            {
                List<int> currentHashs = new List<int>();
                int result = 0;
                foreach (ClientDetails client in clientList)
                {
                        PeerInterface connection;
                        ChannelFactory<PeerInterface> foobFactory;
                        NetTcpBinding tcp = new NetTcpBinding();
                        string url = "net.tcp://" + client.ipAddress.ToString() + ":" + client.port.ToString() + "/DataService";
                        foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                        connection = foobFactory.CreateChannel();
                        Block block = connection.getCurrentBlock();
                        if (block.hash != 0)
                        {
                            currentHashs.Add(block.hash);
                        }
                }
                if (currentHashs.Count() > 1)
                {
                    result = Miner.mostFrequent(currentHashs);
                }
                else if (currentHashs.Count() == 1)
                {
                    result = currentHashs.First();
                }
                return result;
            }
            catch (ArgumentNullException e)
            {
                printLine("MiningThread.getPopularHash " + e.Message);
                return 0;
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                printLine("MiningThread.getPopularHash " + e.Message);
                return 0;
            }

        }

        /*
        * 	Purpose: Sets this clients blockchain to a more recent blockchain
        *	Input:  the new blockchain
        *	Output: 
        */
        public void setBlockchain(Blockchain bc)
        {
            try
            {
                PeerInterface connection;
                ChannelFactory<PeerInterface> foobFactory;
                NetTcpBinding tcp = new NetTcpBinding();
                string url = "net.tcp://" + thisClient.ipAddress.ToString() + ":" + thisClient.port.ToString() + "/DataService";
                foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                connection = foobFactory.CreateChannel();
                connection.SetBlockchain(bc);
            }
            catch (ArgumentNullException e)
            {
                printLine("MiningThread.GetBlockChain " + e.Message);
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                printLine("MiningThread.GetBlockChain " + e.Message);
            }

        }

        /*
        * 	Purpose: Formats String to include client address and port then console logs it
        *	Input: string to output
        *	Output: Console writeline / Debug
        */
        public void printLine(String inString)
        {
            //Console.WriteLine("\t\t"+thisClient.ipAddress.ToString() + ":" + thisClient.port.ToString() + ":\t" + inString);
            Debug.WriteLine("\t\t" + thisClient.ipAddress.ToString() + ":" + thisClient.port.ToString() + ":\t" + inString);
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
                printLine("MiningThread.EncodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                printLine("MiningThread.EncodeString  " + e.Message);
            }
            catch (ArgumentException e)
            {
                printLine("MiningThread.EncodeString  " + e.Message);
            }
            catch (FormatException e)
            {
                printLine("MiningThread.EncodeString  " + e.Message);
            }
            
            return null;
        }

        /*
        * 	Purpose: Dencodes a base64 string into a string
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
                printLine("MiningThread.DecodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                printLine("MiningThread.DecodeString " + e.Message);
            }
            catch (ArgumentException e)
            {
                printLine("MiningThread.DecodeString " + e.Message);
            }
            catch (FormatException e)
            {
                printLine("MiningThread.DecodeString " + e.Message);
            }
            return null;

        }


        /*
        * 	Purpose: Executes the python code
        *	Input: encoded python code
        *	Output: Result of excecution
        */
        public String ExecuteJob(String inJob)
        {
            try
            {
                String toExecute = DecodeString(inJob);
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                var result = engine.Execute(toExecute, scope);
                String resultString = result.ToString();
                return EncodeString(resultString);
            }
            catch(ArgumentNullException e)
            {
                printLine("MiningThread.ExecuteJob " + e.Message);
                return null;
            }
            catch (NotSupportedException e)
            {
                printLine("MiningThread.ExecuteJob " + e.Message);
                return null;
            }
            catch (ArgumentException e)
            {
                printLine("MiningThread.ExecuteJob " + e.Message);
                return null;
            }
            catch (Microsoft.Scripting.SyntaxErrorException e)
            {
                printLine("MiningThread.ExecuteJob " + e.Message);
                return null;
            }


        }

    }
}
