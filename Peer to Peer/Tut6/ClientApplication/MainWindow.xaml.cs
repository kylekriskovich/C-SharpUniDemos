using ClientApplication.Network;
using ClientApplication.Server;
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
using ClientApplication.Server;
using System.ComponentModel;

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
        ClientInstance instance;
        Thread networkThread;
        Thread serverThread;
        int numfailed = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.ipBox.IsEnabled = true;
            this.portBox.IsEnabled = true;
            this.pythonBox.IsEnabled = false;
            this.submit_Button.IsEnabled = false;
            this.result_Button.IsEnabled = false;
            this.divi.IsEnabled = false;
            this.add.IsEnabled = false;
            this.sub.IsEnabled = false;
            this.mult.IsEnabled = false;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            launchThreads();
            this.launch_Button.IsEnabled = false;
            this.ipBox.IsEnabled = false;
            this.portBox.IsEnabled = false;
            this.pythonBox.IsEnabled = true;
            this.submit_Button.IsEnabled = true;
            this.result_Button.IsEnabled = true;
            this.divi.IsEnabled = true;
            this.add.IsEnabled = true;
            this.sub.IsEnabled = true;
            this.mult.IsEnabled = true;
        }

        public void launchThreads()
        {
            client = new ClientDetails();
            client.ipAddress = ipBox.Text.ToString();
            client.port = portBox.Text.ToString();
            instance = new ClientInstance(client);
            ServerThread server = new ServerThread(instance);
            NetworkThread network = new NetworkThread(instance);
            ThreadOperator networkDelegate = new ThreadOperator(network.open);
            ThreadOperator serverDelegate = new ThreadOperator(server.start);

            networkThread = new Thread(()=> { networkDelegate.Invoke(); });
            serverThread = new Thread(() => { serverDelegate.Invoke(); });

            try
            {
                serverThread.Start();
                networkThread.Start();
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine("launch_Threads " + e.Message);
            }
            catch (ThreadStateException e)
            {
                Console.WriteLine("launch_Threads " + e.Message);
            }



        }

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
                connection.CreateJob(pyCode);
                this.submit_Button.IsEnabled = false;
            }
            catch (ArgumentNullException exc)
            {
                Console.WriteLine("CreateJob " + exc.Message);
            }
            
        }
       

        public String DoWork(IProgress<int> progress)
        {
            
            String result = "waiting for solution";
            try
            {
                while (result.Equals("waiting for solution"))
                {
                    result = instance.getSolution();
                    if (progress != null)
                        progress.Report(1);
                    Thread.Sleep(1000);

                }
                return result;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("DoWork " + e.Message);
            }

            return result;
        }

        private async void result_Button_Click(object sender, EventArgs e)
        {
            try
            {
                wait_Bar.Value = 0;
                wait_Bar.Maximum = 100;
                String result = "";

                var progress = new Progress<int>(v =>
                {
                    // This lambda is in UI thread updating the loading bar
                    wait_Bar.Value = wait_Bar.Value + 2.5;
                });

                // Run thread
                result = await Task.Run(() => DoWork(progress));
                wait_Bar.Value = 100;

                if ((DecodeString(result).Equals("Failed please retry")))
                {
                    numfailed++;
                }
                pythonBox.Text = DecodeString(result);
                
                completed.Text = (instance.getComplete() - numfailed).ToString();
                this.submit_Button.IsEnabled = true;
            }
            catch (ArgumentNullException exc)
            {
                Console.WriteLine("result_button " + exc.Message);
            }


        }


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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            pythonBox.Text = "5 + 5";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            pythonBox.Text = "5 - 5";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            pythonBox.Text = "5 * 5";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            pythonBox.Text = "5 / 5";
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
                Console.WriteLine("Exiting App " + exc.Message);
            }
        }

       
    }
}
