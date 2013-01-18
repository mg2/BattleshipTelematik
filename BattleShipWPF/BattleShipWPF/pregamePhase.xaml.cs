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

        public pregamePhase()
        {
            InitializeComponent();

            //Grid DynamicGrid = new Grid();
            //DynamicGrid.Width = 100;
            //DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            //DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            //DynamicGrid.ShowGridLines = true;

            //ColumnDefinition gridCol1 = new ColumnDefinition();
 

            //ColumnDefinition[] gridCols = new ColumnDefinition[10];
            //RowDefinition[] gridRows = new RowDefinition[10];

            gridField.ShowGridLines = true;






            int[,] gameField = new int[10, 10];
            Rectangle[,] gameFieldRect = new Rectangle[10, 10];

            for(int i = 0; i < 10; i++) {
                

                for (int j = 0; j < 10; j++)
                {

                    gameField[i, j] = 1;
                    gameFieldRect[i, j] = new Rectangle();
                    gameFieldRect[i, j].Fill = water;

                    Grid.SetRow(gameFieldRect[i,j], i);
                    Grid.SetColumn(gameFieldRect[i, j], j);

                    gridField.Children.Add(gameFieldRect[i, j]);

                    gameFieldRect[i, j].MouseMove += new MouseEventHandler(mouseOverCell);

                    gameFieldRect[i, j].MouseLeave += new MouseEventHandler(mouseOverCellOut);
                    

                    
                }
            }

        }


        private void btnRot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mouseOverCell(object o, MouseEventArgs ea)
        {
            Rectangle me = (Rectangle)o;

            me.Fill = set_mouseOver;
            
        }

        private void mouseOverCellOut(object o, MouseEventArgs ea)
        {
            Rectangle me = (Rectangle)o;

            me.Fill = water;

        }

    }
}
