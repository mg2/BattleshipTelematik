﻿using System;
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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        Brush set_mouseOver = new SolidColorBrush(System.Windows.Media.Colors.Red);
        Brush water = new SolidColorBrush(System.Windows.Media.Colors.Blue);
        Brush ship = new SolidColorBrush(System.Windows.Media.Colors.Black);
        Brush miss = new SolidColorBrush(System.Windows.Media.Colors.White);

        private int[] gameField = new int[100];
        private int[] opponentGameField = new int[100];
        Rectangle[] playerFieldRect = new Rectangle[100];
        Rectangle[] opponentFieldRect = new Rectangle[100];

        bool yourTurn = true;
        bool waiting = true;


        byte[] m_dataBuffer = new byte[10];
        IAsyncResult m_result;
        public AsyncCallback m_pfnCallBack;
        public Socket m_clientSocket;
        String waitForCommit = "";
        IPAddress ipAddress;

        public GameWindow()
        {
            InitializeComponent();
        }

        public GameWindow(int[] gameField, Socket sock)
            : this()
        {
            this.gameField = gameField;
            m_clientSocket = sock;

            
            
            //init player field
            playerField.ShowGridLines = true;
            for (int i = 0; i < 100; i++)
            {
                playerFieldRect[i] = new Rectangle();

                if (gameField[i] == 1) playerFieldRect[i].Fill = ship;
                else playerFieldRect[i].Fill = water;

                Grid.SetRow(playerFieldRect[i], i / 10);
                Grid.SetColumn(playerFieldRect[i], i % 10);

                playerField.Children.Add(playerFieldRect[i]);
            }

            //init opponent field
            opponentField.ShowGridLines = true;
            for (int i = 0; i < 100; i++)
            {
                opponentGameField[i] = 0;
                opponentFieldRect[i] = new Rectangle();
                opponentFieldRect[i].Fill = water;

                Grid.SetRow(opponentFieldRect[i], i / 10);
                Grid.SetColumn(opponentFieldRect[i], i % 10);

                opponentField.Children.Add(opponentFieldRect[i]);
                opponentFieldRect[i].MouseLeftButtonUp += opponentFieldRect_MouseLeftButtonUp;
            }


            WaitForData();
        }

        //Click = Shot
        private void opponentFieldRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = Array.IndexOf(opponentFieldRect, (Rectangle)sender);
            //x = index % 10, y = index / 10

            //waiting for server response
            if (waiting == true) logText.Text = "Waiting for server response!\n" + logText.Text;
            //Not your turn
            else if (yourTurn == false) logText.Text = "Not your turn!\n" + logText.Text;
            //Invalid field
            else if (opponentGameField[index] != 0) logText.Text = "Invalid shot!\n" + logText.Text;
            else
            {
                //Send to server x, y
                String shotString = "SHOOT " + (index % 10).ToString() + "," + (index / 10).ToString() + "\r\n";
                waiting = true;
            }
        }

        //On command from server
        private void onServerCommand(String command)
        {
            String typ = command.Split(' ')[0];
            if (typ.Equals("TURN")) onServerTurn(command);
            else if (typ.Equals("RESULT")) onServerResult(command);
            else if (typ.Equals("OVER")) onServerOver(command);
        }


        //TURN
        private void onServerTurn(String response)
        {
            this.waiting = false;
            String resultString = response;
            if (resultString.Split(' ')[1].Equals("true"))
            {
                this.yourTurn = true;
                logText.Text = "It is your turn.\n" + logText.Text;
            }
            else
            {
                this.yourTurn = false;
            }
        }

        //GAME OVER
        private void onServerOver(String response)
        {
            String resultString = response;

            if (resultString.Split(' ')[1].Equals("WIN"))
            {
                //WIN!!!!!!!!!!!!!!!
                MessageBox.Show("You won!", "GAME OVER");
            }
            else
            {
                //LOOOOOOOOSEE!!!!
                MessageBox.Show("You lost!", "GAME OVER");
            }
            //TODO Verbindung trennen!!!
            m_clientSocket.Close();
            this.Close();
        }

        //RESULT
        private void onServerResult(String response)
        {
            String resultString = response;

            resultString.Replace("\r\n", "");
            String[] serverResult = resultString.Split(' ');

            int x = Convert.ToInt32(serverResult[2].Split(',')[0]);
            int y = Convert.ToInt32(serverResult[2].Split(',')[1]);
            int index =  x + 10 * y;

            if ((serverResult[0] + " " + serverResult[1]).Equals("TURN FALSE"))
            {
                logText.Text = "Not your turn!\n" + logText.Text;
                yourTurn = false;
            }
            else if (yourTurn == true)
            {
                //reject
                if (serverResult[0].Equals("REJECT"))
                {
                    logText.Text = "Shot rejected from server!\n" + logText.Text;
                }
                //hit
                else if (serverResult[1].Equals("hit"))
                {
                    logText.Text = "Hit!\n" + logText.Text;
                    Rectangle me = opponentFieldRect[index];
                    me.Fill = ship;
                    opponentGameField[index] = 1;
                }
                //miss
                else if (serverResult[1].Equals("miss"))
                {
                    logText.Text = "Miss!\n" + logText.Text;
                    Rectangle me = opponentFieldRect[index];
                    me.Fill = miss;
                    yourTurn = false;
                }
                //sunk
                else if (serverResult[1].Equals("sunk"))
                {
                    logText.Text = "Ship sunk!\n" + logText.Text;
                    Rectangle me = opponentFieldRect[index];
                    me.Fill = ship;
                    opponentGameField[index] = 1;
                }
            }
            else
            {
                //not your turn
                if (serverResult[1].Equals("hit"))
                {
                    logText.Text = "Oponent hit on " + serverResult[2] +".\n" + logText.Text;
                    Rectangle me = playerFieldRect[index];
                    me.Fill = set_mouseOver;
                    opponentGameField[index] = 1;
                }
                //miss
                else if (serverResult[1].Equals("miss"))
                {
                    logText.Text = "Oponent missed on " + serverResult[2] + ".\n" + logText.Text;
                    Rectangle me = playerFieldRect[index];
                    me.Fill = miss;
                    yourTurn = false;
                }
                //sunk
                else if (serverResult[1].Equals("sunk"))
                {
                    logText.Text = "Oponent hit on " + serverResult[2] + " and sunk your ship.\n" + logText.Text;
                    Rectangle me = playerFieldRect[index];
                    me.Fill = set_mouseOver;
                    opponentGameField[index] = 1;
                }
            }
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
                MessageBox.Show("ERROR" + se.Message);
                this.Close();
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

                ///MAGIC
                //parse(szData);

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
    }
}
