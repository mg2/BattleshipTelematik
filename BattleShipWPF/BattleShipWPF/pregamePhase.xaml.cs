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
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace BattleShipWPF
{
    /// <summary>
    /// Interaktionslogik für pregamePhase.xaml
    /// </summary>
    /// 
    public class gamePhaseState
    {
        public int[] field;
        public Socket socket;
        
        public gamePhaseState(int[] Cfield, Socket CSocket)
        {
            field = Cfield;
            socket = CSocket;
          
        }

    }

    public partial class pregamePhase : Window
    {

        const String DELIMITER = "\r\n\0";

        bool newEra = false;


        Brush set_mouseOver = new SolidColorBrush(System.Windows.Media.Colors.Red);
        Brush water = new SolidColorBrush(System.Windows.Media.Colors.Blue);
        Brush ship = new SolidColorBrush(System.Windows.Media.Colors.Black);

        int[] gameField = new int[100];
        Rectangle[] gameFieldRect = new Rectangle[100];

        LinkedList<Grid> itemlist = new LinkedList<Grid>();


        int vertical = 0;
        int shipSize = 5;
        bool validPosition = false;
        int shipsLeft = 10;
        String fieldString = "POSITION ";


        byte[] m_dataBuffer = new byte[10];
        IAsyncResult m_result;
        public AsyncCallback m_pfnCallBack;
        public Socket m_clientSocket;
        String waitForCommit = "";
        IPAddress ipAddress;

        public pregamePhase(IPAddress ipAddress, int port)
            : this()
        {


            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.
                m_clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    m_clientSocket.Connect(remoteEP);
                    WaitForData();

                    Console.WriteLine("Socket connected to {0}",
                        m_clientSocket.RemoteEndPoint.ToString());




                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    return;
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }

        public pregamePhase()
        {
            InitializeComponent();

            //Generate items for lstShipChoose
            int[,] ShipChooseConfig = new int[4,2];
            ShipChooseConfig[0,0] = 5; //Ship size
            ShipChooseConfig[0,1] = 1; //count

            ShipChooseConfig[1,0] = 4;
            ShipChooseConfig[1,1] = 2;

            ShipChooseConfig[2,0] = 3;
            ShipChooseConfig[2,1] = 3;

            ShipChooseConfig[3,0] = 2;
            ShipChooseConfig[3,1] = 4;

            
            lstShipChoose.ItemsSource = itemlist;
            
            //init Ship list
            for (int i = 0; i < 4; i++)
            {

                Grid DynamicGrid = new Grid();
                DynamicGrid.Width = 100;
                DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
                DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
                DynamicGrid.ShowGridLines = true;

                ColumnDefinition gridCol1 = new ColumnDefinition();
                ColumnDefinition gridCol2 = new ColumnDefinition();
                RowDefinition gridRow1 = new RowDefinition();

                DynamicGrid.ColumnDefinitions.Add(gridCol1);
                DynamicGrid.ColumnDefinitions.Add(gridCol2);
                DynamicGrid.RowDefinitions.Add(gridRow1);

                Label lblShip1 = new Label();
                lblShip1.Content =  ShipChooseConfig[i,0].ToString();
                Label lblShipCount1 = new Label();
                lblShipCount1.Content = ShipChooseConfig[i, 1].ToString();

                Grid.SetColumn(lblShipCount1, 1);

                DynamicGrid.Children.Add(lblShip1);
                DynamicGrid.Children.Add(lblShipCount1);

                itemlist.AddLast(DynamicGrid);
            }

            lstShipChoose.SelectionChanged += lstShipChoose_SelectionChanged;
            lstShipChoose.SelectedIndex = 0;


            
            
            //init field
            gridField.ShowGridLines = true;
            for (int i = 0; i < 100; i++)
            {
                gameField[i] = 0;

                gameFieldRect[i] = new Rectangle();
                gameFieldRect[i].Fill = water;

                Grid.SetRow(gameFieldRect[i], i / 10);
                Grid.SetColumn(gameFieldRect[i], i % 10);

                gridField.Children.Add(gameFieldRect[i]);

                gameFieldRect[i].MouseMove += new MouseEventHandler(mouseOverCell);
                gameFieldRect[i].MouseLeave += new MouseEventHandler(mouseOverCellOut);
                gameFieldRect[i].MouseLeftButtonUp += pregamePhase_MouseLeftButtonUp;
            }
        }


        private void lstShipChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            shipSize = 5 - lb.SelectedIndex;
        }

        private void mouseOverCell(object o, MouseEventArgs ea)
        {
            validPosition = false;
            int index = Array.IndexOf(gameFieldRect, (Rectangle)o);
            if (index % 10 * ((vertical + 1) % 2) + index / 10 * vertical + shipSize <= 10) //checks if inbounds
            {
                bool overlap = false;
                for (int i = 0; i < shipSize && overlap == false; i++) //check overlap
                {
                    if (gameField[index] == 1) overlap = true;
                    index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                }

                if (overlap == false)
                {
                    index = Array.IndexOf(gameFieldRect, (Rectangle)o);
                    for (int i = 0; i < shipSize; i++)
                    {
                        Rectangle me = gameFieldRect[index];
                        me.Fill = set_mouseOver;
                        index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                    }
                    validPosition = true;
                }
            }
        }

        private void mouseOverCellOut(object o, MouseEventArgs ea)
        {
            int index = Array.IndexOf(gameFieldRect, (Rectangle)o);
            if (index % 10 * ((vertical + 1) % 2) + index / 10 * vertical + shipSize <= 10) //checks if inbounds
            {
                bool overlap = false;
                for (int i = 0; i < shipSize && overlap == false; i++) //check overlap
                {
                    if (gameField[index] == 1) overlap = true;
                    index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                }

                if (overlap == false)
                {
                    index = Array.IndexOf(gameFieldRect, (Rectangle)o);
                    for (int i = 0; i < shipSize; i++)
                    {
                        Rectangle me = gameFieldRect[index];
                        me.Fill = water;
                        index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                    }
                }
            }
        }

        private void pregamePhase_MouseLeftButtonUp(object sender, MouseButtonEventArgs ea)
        {
            Grid lstItem = (Grid)lstShipChoose.SelectedItem;

            Label lblShipCount = lstItem.Children.Cast<Label>()
                .First(e => Grid.GetRow(e) == 0 && Grid.GetColumn(e) == 1);
            int newVal = Convert.ToInt32(lblShipCount.Content);

            if (validPosition == true && newVal > 0)
            {
                int index = Array.IndexOf(gameFieldRect, (Rectangle)sender);

                //Prepare the field String.
                fieldString += (index % 10).ToString() + "," + (index / 10).ToString() + "-";
                fieldString += (index % 10 + (shipSize - 1) * ((vertical + 1) % 2)).ToString() + "," + (index / 10 + (shipSize - 1) * (vertical % 2)).ToString();

                for (int i = 0; i < shipSize; i++)
                {
                    Rectangle me = gameFieldRect[index];
                    me.Fill = ship;
                    gameField[index] = 1;
                    index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                }
                newVal--;
                lblShipCount.Content = newVal.ToString();
                shipsLeft--;
                if (shipsLeft == 0)
                {
                    btnSubmit.IsEnabled = true;
                    fieldString += "\r\n";
                }
                else fieldString += " ";
            }
        }

        private void RadioButton_Checked_Horizontal(object sender, RoutedEventArgs e)
        {
            vertical = 0;
        }
        private void RadioButton_Checked_Vertical(object sender, RoutedEventArgs e)
        {
            vertical = 1;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //RESET????
        }

        private void WaitForCommit(string msg_txt)
        {
            if (waitForCommit != "")
            {
                Console.WriteLine("ERROR: Send a new commando before conforming old one");
            }
            waitForCommit = msg_txt;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];
            try
            {
                // Encode the data string into a byte array.
                byte[] msg = Encoding.UTF8.GetBytes(fieldString);

                // Send the data through the socket.
                int bytesSent = m_clientSocket.Send(msg);
                WaitForCommit("POSITION");

            }
            catch (Exception)
            {
                Console.WriteLine(e.ToString());

            }
            //in fieldString ist der Server-friendly string.

        }




        public void WaitForData()
        {

            try
            {
                if (m_pfnCallBack == null && newEra == false)
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
                MessageBox.Show(se.Message);
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


                parse(szData, asyn); 

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



        private void parse(string szData, IAsyncResult asyn)
        {
            string[] stringSeparators = new string[] { DELIMITER };
            String[] commands = szData.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            foreach (String command in commands)
            {
                String[] args = command.Split(' ');

                switch (args[0])
                {
                    case "CONFIRM":

                        switch (waitForCommit)
                        {
                            case "":
                                Console.WriteLine("Unexpected CONFIRM");
                                break;
                            case "POSITION":
                                //Start GAME

                                m_pfnCallBack = null;
                                newEra = true;
                                 gamePhaseState gps = new gamePhaseState(gameField, m_clientSocket);
                                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                new Action<gamePhaseState>(startGamePhase),
                                gps);
                                
                                break;
                                

                        }

                        waitForCommit = "";
                        break;                  


                    case "EXIT_GAME":
                        MessageBox.Show("Server ended the game", "ERROR");
                        this.Close();
                        break;

                    case "REJECT":

                        MessageBox.Show("Field was rejected", "ERROR");
                        break;

                }

            }
        }

        private void startGamePhase(gamePhaseState obj)
        {

            GameWindow gameWindow = new GameWindow(obj.field, obj.socket);
            gameWindow.Show();
            this.Close();

            
        }

        private void btnQuick_Click(object sender, RoutedEventArgs e)
        {
            fieldString = "POSITION 0,0-4,0 6,9-9,9 8,0-8,3 0,4-0,6 4,5-6,5 2,2-4,2 1,9-2,9 8,7-9,7 8,4-9,4 2,4-3,4\r\n";
            String intField = "1 1 1 1 1 0 0 0 1 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 0 0 0 1 0 0 0 0 0 0 0 0 0 1 0 1 0 1 1 0 0 0 0 1 1 1 0 0 0 1 1 1 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 1 1 1 1\r\n";

            String[] Input = intField.Split(' ');

            for(int i = 0; i < 100; i++) {
                gameField[i] = Convert.ToInt32(Input[i]);
            }
            btnSubmit.IsEnabled = true;
            
        }

        private void btnQuick2_Click(object sender, RoutedEventArgs e)
        {
                        fieldString = "POSITION 1,0-5,0 0,1-3,1 0,2-3,2 0,3-2,3 0,4-2,4 0,5-2,5 0,6-1,6 0,7-1,7 0,8-1,8 0,9-1,9\r\n";
                        String intField = "0 1 1 1 1 1 0 0 0 0 1 1 1 1 0 0 0 0 0 0 1 1 1 1 0 0 0 0 0 0 1 1 1 0 0 0 0 0 0 0 1 1 1 0 0 0 0 0 0 0 1 1 1 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 0\r\n";

            String[] Input = intField.Split(' ');

            for(int i = 0; i < 100; i++) {
                gameField[i] = Convert.ToInt32(Input[i]);
            }
            btnSubmit.IsEnabled = true;
        }
    }
}
