
using DetailClasses;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ClientApplication.Mining;
using System.Web.Util;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ThreadOperator();
        public delegate String Update();
        ClientDetails client;
        Blockchain blockchain;
        List<string> list;
        List<string> solved;
        List<Transaction> comp;
        Thread networkThread;
        Thread serverThread;
        int submitted;

        public MainWindow()
        {
            InitializeComponent();
            this.submit_Button.IsEnabled = false;
            this.pythonBox.IsEnabled = false;
            this.result_Button.IsEnabled = false;
            this.exit.IsEnabled = false;
            submitted = 0;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            launchThreads();
            this.launch_Button.IsEnabled = false;
            this.ipBox.IsEnabled = false;
            this.portBox.IsEnabled = false;
            this.submit_Button.IsEnabled = true;
            this.pythonBox.IsEnabled = true;
            this.result_Button.IsEnabled = true;
            this.exit.IsEnabled = true;

        }

        public void launchThreads()
        {
            client = new ClientDetails();
            blockchain = new Blockchain();
            list = new List<string>();
            solved = new List<string>();
            comp = new List<Transaction>();
            

            client.ipAddress = ipBox.Text.ToString();
            client.port = portBox.Text.ToString();
            ClientInstance.SetClient(client);
            ClientInstance.SetBlockchain(blockchain);
            ClientInstance.SetTransactions(list);
            ClientInstance.SetSolved(solved);
            ClientInstance.SetComplete(comp);
            MiningThread miner = new MiningThread(ClientInstance.GetClient());
            ServerThread server = new ServerThread(ClientInstance.GetClient());
            ThreadOperator networkDelegate = new ThreadOperator(miner.open);
            ThreadOperator serverDelegate = new ThreadOperator(server.start);

            networkThread = new Thread(()=> { networkDelegate.Invoke(); });
            serverThread = new Thread(() => { serverDelegate.Invoke(); });

            try
            {
                serverThread.Start();
                networkThread.Start();
            }
            catch (OutOfMemoryException exc)
            {
                Console.WriteLine("launchThreads " + exc.Message);
            }
            catch (ThreadStateException exc)
            {
                Console.WriteLine("launchThreads " + exc.Message);
            }
            catch (Exception exc) 
            {
                Console.WriteLine("launchThreads " + exc.Message);
            }


        }

        /*
        * 	Purpose: Creates a job with the python code and sends it to the miner thread
        *	Input:  
        *	Output: 
        */
        private void CreateJob(object sender, RoutedEventArgs e)
        {
            try
            {
                String pyCode = EncodeString(pythonBox.Text.ToString());
                PeerInterface connection;
                ChannelFactory<PeerInterface> foobFactory;
                NetTcpBinding tcp = new NetTcpBinding();
                string url = "net.tcp://" + client.ipAddress.ToString() + ":" + client.port.ToString() + "/DataService";
                foobFactory = new ChannelFactory<PeerInterface>(tcp, url);
                connection = foobFactory.CreateChannel();
                connection.RecieveTransation(pyCode);
                submitted++;
                sub.Text = submitted.ToString();
            }
            catch (ArgumentNullException exc)
            {
                Console.WriteLine("CreateJob " + exc.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("CreateJob " + exc.Message);
            }
            



        }

        public List<Transaction> DoWork(IProgress<int> progress, string code)
        {

            bool check = false;
            List<Transaction> result = new List<Transaction>();
            while (!check)
            {
                check = ClientInstance.Solved(code);
                if (progress != null)
                    progress.Report(1);
                Thread.Sleep(1000);

            }
            return result;
        }

        /*
        * 	Purpose: waits until results are ready and updates the loading bar
        *	Input:  
        *	Output: 
        */
        private async void result_Button_Click(object sender, EventArgs e)
        {
            try
            {
                wait_Bar.Value = 0;
                wait_Bar.Maximum = 100;
                List<Transaction> result = new List<Transaction>();
                String code = EncodeString(pythonBox.Text.ToString());

                var progress = new Progress<int>(v =>
                {
                    // This lambda is in UI thread updating the loading bar
                    wait_Bar.Value = wait_Bar.Value + 2.5;
                });

                // Run thread
                result = await Task.Run(() => DoWork(progress, code));
                wait_Bar.Value = 100;

                completeList.Items.Clear();

                completed.Text = blockchain.Count().ToString();

                int ii = 1;
                foreach (Transaction t in comp)
                {
                    string item = ii.ToString() + ": Code= \"" + DecodeString(t.code) + "\"   Result=  " + DecodeString(t.answer);
                    //Console.WriteLine(item);
                    completeList.Items.Add(item);
                    ii++;
                }
            }
            catch (ArgumentNullException exc)
            {
                Console.WriteLine("Async Result " + exc.Message);
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
                Console.WriteLine("EncodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("EncodeString " + e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("EncodeString " + e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("EncodeString " + e.Message);
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
                Console.WriteLine("DecodeString " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("DecodeString " + e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("DecodeString " + e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("DecodeString " + e.Message);
            }
            return null;

        }


        private void exit_app(object sender, RoutedEventArgs e)
        {
            try
            {
                networkThread.Abort();
                serverThread.Abort();
                this.Close();
            }
            catch (Exception exc)
            {

            }
        }

    }
}
