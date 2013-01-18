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
        //int[,] gameField = new int[10, 10];
        Rectangle[] gameFieldRect = new Rectangle[100];

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
            for(int i = 0; i < 100; i++) {
                gameFieldRect[i] = new Rectangle();
                gameFieldRect[i].Fill = water;

                Grid.SetRow(gameFieldRect[i], i/10);
                Grid.SetColumn(gameFieldRect[i], i%10);

                gridField.Children.Add(gameFieldRect[i]);

                gameFieldRect[i].MouseMove += new MouseEventHandler(mouseOverCell);
                gameFieldRect[i].MouseLeave += new MouseEventHandler(mouseOverCellOut);
            }
        }


        private void btnRot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mouseOverCell(object o, MouseEventArgs ea)
        {
            Rectangle me = (Rectangle)o;
            int index = Array.IndexOf(gameFieldRect, me);
            me.Fill = set_mouseOver;
            if (index>0) btnRot.Content = index.ToString();
            
        }

        private void mouseOverCellOut(object o, MouseEventArgs ea)
        {
            Rectangle me = (Rectangle)o;
            
            me.Fill = water;

        }

    }
}
