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
        private int[] gameField = new int[100];

        public GameWindow()
        {
            InitializeComponent();
        }

        public GameWindow(int[] gameField)
            : this()
        {
            this.gameField = gameField;
            test.Text = "";
            for (int i = 0; i < 100; i++)
            {
                test.Text += gameField[i].ToString();
            }
        }
    }
}
