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

        LinkedList<Grid> itemlist = new LinkedList<Grid>();


        int vertical = 0;
        int shipSize = 5;

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
            }


        }


        private void btnRot_Click(object sender, RoutedEventArgs e)
        {

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

        private void mouseClick(object o, MouseButtonEventArgs ea)
        {
                  
            Grid lstItem = (Grid)lstShipChoose.SelectedItem;

            Label lblShipCount = lstItem.Children.Cast<Label>()
                .First(e => Grid.GetRow(e) == 0 && Grid.GetColumn(e) == 1);

            int newVal = Convert.ToInt32(lblShipCount.Content) - 1;

            lblShipCount.Content = newVal.ToString();

            if (newVal == 0)
            {
                itemlist.Remove(lstItem);
            }
        }

    }
}
