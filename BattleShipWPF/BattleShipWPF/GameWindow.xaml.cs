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

        private int[] gameField = new int[100];
        Rectangle[] playerFieldRect = new Rectangle[100];
        Rectangle[] opponentFieldRect = new Rectangle[100];

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
                opponentFieldRect[i] = new Rectangle();
                opponentFieldRect[i].Fill = water;

                Grid.SetRow(opponentFieldRect[i], i / 10);
                Grid.SetColumn(opponentFieldRect[i], i % 10);

                opponentField.Children.Add(opponentFieldRect[i]);
            }
        }
    }
}
