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

        public GameWindow()
        {
            InitializeComponent();
        }

        public GameWindow(int[] gameField)
            : this()
        {
            this.gameField = gameField;
            
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
        }

        private void opponentFieldRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = Array.IndexOf(opponentFieldRect, (Rectangle)sender);
            //x = index % 10, y = index / 10

            //Not your turn
            if (yourTurn == false) logText.Text = "Not your turn!\n" + logText.Text;
            //Invalid field
            else if (opponentGameField[index] != 0) logText.Text = "Invalid shot!\n" + logText.Text;
            else
            {
                //Send to server x, y
                String shotString = "SHOOT " + (index % 10).ToString() + "," + (index / 10).ToString() + "\r\n";

                //Wait for response
                //call 
                
            }
        }

        private void onServerResult(String result)
        {
            String resultString = "RESULT hit 4,4\r\n"; //HARDCODED!!!
            //String resultString = result;

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
                else
                {
                    // ???
                }
            }
            else
            {
                //not your turn!
                if (serverResult[1].Equals("hit"))
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
                else
                {
                    if (serverResult[1].Equals("hit"))
                    {
                        logText.Text = "Oponent hit on " +  + ".\n" + logText.Text;
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
                    else
                    {
                        // ???
                    }
                }
            }
        }
    }
}
