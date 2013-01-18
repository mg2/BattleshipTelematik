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
    /// Interaktionslogik für pregamePhase.xaml
    /// </summary>
    public partial class pregamePhase : Window
    {
        Brush set_mouseOver = new SolidColorBrush(System.Windows.Media.Colors.Red);
        Brush water = new SolidColorBrush(System.Windows.Media.Colors.Blue);
        int[] gameField = new int[100];
        Rectangle[] gameFieldRect = new Rectangle[100];
        int vertical = 0;
        int shipSize = 5;

        public pregamePhase()
        {
            InitializeComponent();
            
            gridField.ShowGridLines = true;
            for(int i = 0; i < 100; i++) {
                gameField[i] = 0;

                gameFieldRect[i] = new Rectangle();
                gameFieldRect[i].Fill = water;

                Grid.SetRow(gameFieldRect[i], i/10);
                Grid.SetColumn(gameFieldRect[i], i%10);

                gridField.Children.Add(gameFieldRect[i]);

                gameFieldRect[i].MouseMove += new MouseEventHandler(mouseOverCell);
                gameFieldRect[i].MouseLeave += new MouseEventHandler(mouseOverCellOut);
            }
        }

        private void mouseOverCell(object o, MouseEventArgs ea)
        {
            int index = Array.IndexOf(gameFieldRect, (Rectangle)o);
            if (index % 10 * ((vertical + 1) % 2) + index / 10 * vertical + shipSize <= 10) //checks if inbounds
            {
                for (int i = 0; i < shipSize; i++)
                {
                    Rectangle me = gameFieldRect[index];
                    me.Fill = set_mouseOver;
                    index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                }
            }
        }

        private void mouseOverCellOut(object o, MouseEventArgs ea)
        {
            int index = Array.IndexOf(gameFieldRect, (Rectangle)o);
            if (index % 10 * ((vertical + 1) % 2) + index / 10 * vertical + shipSize <= 10) //checks if inbounds
            {
                for (int i = 0; i < shipSize; i++)
                {
                    Rectangle me = gameFieldRect[index];
                    me.Fill = water;
                    index = index + 1 * ((vertical + 1) % 2) + 10 * vertical;
                }
            }

        }

    }
}
