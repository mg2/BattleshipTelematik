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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;


    
            lblStatus.Content = "Connecting...";

           

            //send JOIN game
            byte[] packet = Encoding.UTF8.GetBytes("JOIN");

            lblStatus.Content = "Waiting for Player";


            pregamePhase pregamePhaseWindow = new pregamePhase();
            ///pregamePhaseWindow.Owner = this;
            pregamePhaseWindow.Show();
            this.Close();
            



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
    }
}
