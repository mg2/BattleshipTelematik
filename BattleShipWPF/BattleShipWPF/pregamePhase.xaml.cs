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
        Brush ship = new SolidColorBrush(System.Windows.Media.Colors.Black);

        int[] gameField = new int[100];
        Rectangle[] gameFieldRect = new Rectangle[100];

        LinkedList<Grid> itemlist = new LinkedList<Grid>();


        int vertical = 0;
        int shipSize = 5;
        bool validPosition = false;
        int shipsLeft = 10;
        String fieldString = "POSITION ";

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

        private void mouseOverCellOut(object o, MouseEventArgs ea) //TODO if previous color = black and not water?
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

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //in fieldString ist der Server-friendly string.
            GameWindow gameWindow = new GameWindow(gameField);
            gameWindow.Show();
            this.Close();
        }
    }
}
