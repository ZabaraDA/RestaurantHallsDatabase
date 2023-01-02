using Microsoft.Win32;
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
using System.Xml.Linq;

namespace WpfApp7
{
    public partial class MainWindow : Window
    {
        int diameterTable = 100;
        int quantity = 1;
        int indent = 30;

        int xC;
        int yC;

        int xC1;
        int yC1;

        List<(int x, int y)> coordinatesList;
        List<TableParameter> tableCoordinatesList;

        TableParameter tableParameter;

        Grid selectedGrid;
        Grid grid;

        public class TableParameter
        {
            public int Number { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public TableParameter(int number, int x, int y)
            {
                Number = number;
                X = x;
                Y = y;
            }

        }
        public MainWindow()
        {
            InitializeComponent();

            NumberOfChairsComboBox.SelectedIndex = 0;
            TableTypeComboBox.SelectedIndex = 0;

            coordinatesList = new List<(int x, int y)>()
            {
                (diameterTable / 2, 0),
                (-diameterTable / 2, 0),
                (0, diameterTable / 2),
                (0, -diameterTable / 2)
            };
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int x = Convert.ToInt16(e.GetPosition(HallGrid).X) - diameterTable / 2;
            int y = Convert.ToInt16(e.GetPosition(HallGrid).Y) - diameterTable / 2;

            if (x < indent || y < indent)
            {
                MessageBox.Show("Внимание1");
                return;
            }

            if (tableCoordinatesList == null)
            {
                tableCoordinatesList = new List<TableParameter>();
                tableCoordinatesList.Add(new TableParameter(quantity, x, y) { Number = quantity, X = x, Y = y });
            }
            else if (CorrectCoordinates(x, y, diameterTable + indent, tableCoordinatesList) == false)
            {
                MessageBox.Show("Внимание2");
                return;
            }
            else
            {
                tableCoordinatesList.Add(new TableParameter(quantity, x, y) { Number = quantity, X = x, Y = y });
            }

            grid = new Grid() // контейнер (хранит все элементы стола)
            {
                Height = diameterTable,
                Width = diameterTable,
                Background = Brushes.Black,
                RenderTransform = new TranslateTransform(x, y),
                Name = "grid" + quantity.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,

            };

            grid.MouseEnter += Window_MouseEnter;
            grid.MouseLeave += Window_MouseLeave;
            grid.MouseDown += Window_MouseRightButtonDown;

            Ellipse ellipse = new Ellipse() // стол (круглый, квадратный и др.)
            {
                Fill = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\dzaba\\source\\repos\\WpfApp7\\Images\\tex.jpg", UriKind.Relative))),
                Width = diameterTable,
                Height = diameterTable,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center

            };

            for (int i = 0; i <= NumberOfChairsComboBox.SelectedIndex; i++)
            {
                Rectangle rectangle = new Rectangle()
                {
                    Width = diameterTable / 3,
                    Height = diameterTable / 3,
                    Fill = Brushes.MediumSlateBlue,
                    RenderTransform = new TranslateTransform(coordinatesList[i].x, coordinatesList[i].y)
                };
                grid.Children.Add(rectangle);
            }

            Button button = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = diameterTable,
                Height = diameterTable / 3,
                Content = "Столик" + quantity.ToString(),
                Background = Brushes.AliceBlue,
                Name = "button" + quantity.ToString(),
            };
            button.Click += DeleteButton1_Click;


            grid.Children.Add(ellipse);
            grid.Children.Add(button);
            HallGrid.Children.Add(grid);

            quantity++;
           
            lw.ItemsSource = tableCoordinatesList.ToList();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) // удаление всех
        {
            var children = HallGrid.Children.OfType<UIElement>().ToList();

            foreach (UIElement ellipse in children)
                HallGrid.Children.Remove(ellipse);
            quantity = 1;
            tableCoordinatesList.Clear();
            lw.ItemsSource = tableCoordinatesList.ToList();

        }
        private void DeleteButton1_Click(object sender, RoutedEventArgs e)  // удаление одного
        {
            Button selectedButton = sender as Button;

            Grid removedTable = selectedButton.Parent as Grid;
            HallGrid.Children.Remove(removedTable);

            var coordinatesRemovedTable = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(removedTable.Name.Replace("grid", "")))).FirstOrDefault();
            tableCoordinatesList.Remove(coordinatesRemovedTable);

            lw.ItemsSource = tableCoordinatesList.ToList();
        }

        private bool CorrectCoordinates(double coordinateX, double coordinateY, int distant, List<TableParameter> coordinatesList)
        {
            for (int j = 0; j < coordinatesList.Count;)
            {
                if (coordinateX + distant < coordinatesList[j].X || coordinateX - distant > coordinatesList[j].X || coordinateY + distant < coordinatesList[j].Y || coordinateY - distant > coordinatesList[j].Y)
                {
                    j++;

                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            var a = sender as Grid;
            a.Background = Brushes.Red;
            Cursor = Cursors.Hand;
            //selectedGrid = null;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            var a = sender as Grid;
            a.Background = Brushes.White;
            Cursor = Cursors.Arrow;
            //selectedGrid = null;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
                xC = Convert.ToInt16(e.GetPosition(HallGrid).X) - diameterTable / 2;
                yC = Convert.ToInt16(e.GetPosition(HallGrid).Y) - diameterTable / 2;
                txtbX.Text = Convert.ToString(xC);
                txtbY.Text = Convert.ToString(yC);
            if (selectedGrid != null && e.RightButton == MouseButtonState.Pressed)
            {
                selectedGrid.RenderTransform = new TranslateTransform(xC, yC);
                tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();
                tableCoordinatesList.Remove(tableParameter);
                tableCoordinatesList.Add(new TableParameter(tableParameter.Number,xC,yC) { Number = tableParameter.Number, X = xC, Y = yC });
                lw.ItemsSource = tableCoordinatesList.ToList();
            }
           
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedGrid = sender as Grid;
            tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();

            xC1 = tableParameter.X;
            yC1 = tableParameter.Y;

        }


        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedGrid != null)
            {
                var s = selectedGrid;
                tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();
                tableCoordinatesList.Remove(tableParameter);
                if (xC< indent || yC < indent || CorrectCoordinates(xC, yC, diameterTable + indent, tableCoordinatesList) == false)
                {
                    MessageBox.Show("Внимание2");
                    s.RenderTransform = new TranslateTransform(xC1, yC1);
                    tableCoordinatesList.Add(new TableParameter(tableParameter.Number, xC1, yC1) { Number = tableParameter.Number, X = xC1, Y = yC1 });
                }
                else
                {
                    tableCoordinatesList.Add(tableParameter);
                }
                    lw.ItemsSource = tableCoordinatesList.ToList();
                selectedGrid = null;
            }
        }
        private void AddTableButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

                //selectedGrid.SetValue(Canvas.TopProperty, e.GetPosition(HallGrid).Y - yC);











//UIElement element = HallGrid.FindName("ellipses") as UIElement;
//HallGrid.Children.Remove(element);



//grid.Children.Remove(sender as Button);
//grid.Background = Brushes.Tomato;




//var a = HallGrid.Children.OfType<Button>().ToList();
//foreach (object element in HallGrid.Children)
//{
//    if (element is Grid)
//    {
//        Grid grid = (Grid)element;
//        if (grid.Tag == )
//        {
//            HallGrid.Children.Remove(grid);
//        }
//    }
//}