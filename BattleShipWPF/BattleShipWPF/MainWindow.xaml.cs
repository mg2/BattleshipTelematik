using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net;
using System.Net.Sockets;

namespace BattleShipWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        byte[] m_dataBuffer = new byte[10];
        IAsyncResult m_result;
        public AsyncCallback m_pfnCallBack;
        public Socket m_clientSocket;
        String waitForCommit = "";
        IPAddress ipAddress;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;


            lblStatus.Content = "Connecting...";


            // Connect to a remote device.

            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            IPHostEntry ipHostInfo = Dns.Resolve(txtServerIP.Text);
            ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9090);

            // Create a TCP/IP  socket.
            m_clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);


            // Connect the socket to the remote endpoint. Catch any errors.
            try {
                m_clientSocket.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    m_clientSocket.RemoteEndPoint.ToString());

                //Wait asynchron for answers
                WaitForData();

                // Send JOIN Command
                String msg_txt = "JOIN";
                byte[] msg = Encoding.UTF8.GetBytes(msg_txt);

                // Send the data through the socket.
                int bytesSent = m_clientSocket.Send(msg);
                WaitForCommit(msg_txt);

                
            } catch (ArgumentNullException ane) {
                Console.WriteLine("ArgumentNullException : {0}",ane.ToString());

                lblStatus.Content = "ArgumentNullException :" +ane.ToString();
                btnStart.IsEnabled = true;
                return;
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}",se.ToString());
                lblStatus.Content = "SocketException :" + se.ToString();
                btnStart.IsEnabled = true;
                return;
            } catch (Exception be) {
                Console.WriteLine("Unexpected exception : {0}", be.Message);
                lblStatus.Content = "Unexpected exception :" + be.Message;
                btnStart.IsEnabled = true;
                return;
            }

            
            



        }

        private void WaitForCommit(string msg_txt)
        {
            if (waitForCommit != "")
            {
                Console.WriteLine("ERROR: Send a new commando before conforming old one");
            }
            waitForCommit = msg_txt;
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            String field = "0000000111011111000000000001000000000110010010011001001001000000100000111000000000000000010111100111";
            int[] gameField = new int[100];
            for (int i = 0; i < 100; i++)
            {
                gameField[i] = Convert.ToInt32(field[i].ToString());
            }
            GameWindow gw = new GameWindow(gameField);
            gw.Show();
            this.Close();
        }


        public void WaitForData()
        {
            try
            {
                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket();
                theSocPkt.thisSocket = m_clientSocket;
                // Start listening to the data asynchronously
                m_result = m_clientSocket.BeginReceive(theSocPkt.dataBuffer,
                                                        0, theSocPkt.dataBuffer.Length,
                                                        SocketFlags.None,
                                                        m_pfnCallBack,
                                                        theSocPkt);
            }
            catch (SocketException se)
            {
                lblStatus.Content = "ERROR" + se.Message;
                btnStart.IsEnabled = true;
            }


        }
        public class SocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[1024];
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                SocketPacket theSockId = (SocketPacket)asyn.AsyncState;
                int iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);


                parse(szData);
                
                WaitForData();
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void parse(string szData)
        {
            String[] commands = szData.Split('\0');



            foreach (String command in commands)
            {
                String[] args = command.Split(' ');
                if (args[0] == "")
                {
                    continue;
                }
                switch (args[0])
                {
                    case "CONFIRM":

                        switch(waitForCommit) {
                            case "":
                                Console.WriteLine("Unexpected COMMIT");
                                break;
                            case "JOIN":
                                
                                //lblStatus.Content = "Waiting for other Player";
                                break;
                        }

                        waitForCommit = "";
                        break;
                    
                    case "EXIT_GAME":
                        MessageBox.Show("Server ended the game","ERROR");
                        this.Close();
                        break;

                    case "STARTGAME":

                        //close Connection
                        m_clientSocket.Close();
                        m_clientSocket = null;

                        //Start PregamePhase
                        int newport = Convert.ToInt32(args[1]);
                        pregamePhase pregamePhaseWindow = new pregamePhase(ipAddress, newport);
                        pregamePhaseWindow.Show();
                        this.Close();

                        break;

                    default:
                        Console.WriteLine("Unknow command received: " + args[0]);
                        break;   


                }
            }
        }




       
    }
}
