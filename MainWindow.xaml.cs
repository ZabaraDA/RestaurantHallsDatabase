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
using WpfApp7.Databases;

namespace WpfApp7
{
    public partial class MainWindow : Window
    {
        int diameterTable = 100;
        int quantity = 1;
        int indent = 30;

        int xMousePosition;
        int yMousePosition;

        int xCoordinate;
        int yCoordinate;

        bool angleTable;

        List<TableParameter> tableCoordinatesList = new List<TableParameter>();
        List<Стол> addedTables = new List<Стол>();

        TableParameter tableParameter;

        Grid selectedGrid;
        Grid grid;

        RestaurantHallsDatabaseEntities databaseEntities = new RestaurantHallsDatabaseEntities();

        public class TableParameter
        {
            public int Number { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public bool Angle { get; set; }
            public TableParameter(int number, int x, int y, bool angle)
            {
                Number = number;
                X = x;
                Y = y;
                Angle = angle;
            }

        }
        public MainWindow()
        {
            InitializeComponent();
            var g = databaseEntities.Стол.Where(x => x.Код.Equals(1)).FirstOrDefault();
            
            TableTypeComboBox.SelectedIndex = 0;

        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int x = Convert.ToInt16(e.GetPosition(HallGrid).X) - diameterTable / 2;
            int y = Convert.ToInt16(e.GetPosition(HallGrid).Y) - diameterTable / 2;

            if (x < indent || y < indent)
            {
                MessageBox.Show("Элемент вне границ экрана");
                return;
            }

            if (tableCoordinatesList == null)
            {
                tableCoordinatesList = new List<TableParameter>
                {
                    new TableParameter(quantity, x, y, false) { Number = quantity, X = x, Y = y, Angle = false }
                };
            }
            else if (CorrectCoordinates(x, y, diameterTable + indent, tableCoordinatesList) == false)
            {
                MessageBox.Show("Недопустимое расположение, необходим отступ от других элементов");
                return;
            }
            else
            {
                tableCoordinatesList.Add(new TableParameter(quantity, x, y, false) { Number = quantity, X = x, Y = y, Angle = false });
            }

            grid = new Grid() // контейнер (хранит все элементы стола)
            {
                Height = diameterTable,
                Width = diameterTable,
                RenderTransform = new TranslateTransform(x, y),
                Name = "grid" + quantity.ToString(),
            };

            grid.MouseEnter += Window_MouseEnter;
            grid.MouseLeave += Window_MouseLeave;
            grid.MouseDown += Window_MouseRightButtonDown;

            if (TableTypeComboBox.SelectedIndex == 0)
            {
                Ellipse ellipse = new Ellipse() // стол (круглый, квадратный и др.)
                {
                    //Fill = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\dzaba\\source\\repos\\WpfApp7\\Images\\tex.jpg", UriKind.Relative))),
                    Fill = Brushes.LightBlue,
                    Width = diameterTable,
                    Height = diameterTable,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 3

                };
                grid.Children.Add(ellipse);
            }
            else if (TableTypeComboBox.SelectedIndex == 1)
            {
                Rectangle rectangle = new Rectangle() // стол (круглый, квадратный и др.)
                {

                    Fill = Brushes.LightBlue,
                    Width = diameterTable,
                    Height = diameterTable,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 3

                };
                grid.Children.Add(rectangle);
            }

            StackPanel stackPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            TextBlock textBlock = new TextBlock()
            {
                Text = "Столик" + quantity.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Button button = new Button()
            {
                Width = diameterTable / 2 + 10,
                Height = diameterTable / 5 + 5,
                Content = "Удалить",
                FontSize = 14,
                Background = Brushes.AliceBlue,
            };
            button.Click += DeleteButton1_Click;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);


            grid.Children.Add(stackPanel);

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
            addedTables.Clear();
            FilterTables();
            lw.ItemsSource = tableCoordinatesList.ToList();


        }
        private void DeleteButton1_Click(object sender, RoutedEventArgs e)  // удаление одного
        {
            Button selectedButton = sender as Button;

            StackPanel selectedStackPanel = selectedButton.Parent as StackPanel;
            Grid removedTableGrid = selectedStackPanel.Parent as Grid;
            HallGrid.Children.Remove(removedTableGrid);

            var removedCoordinatesTable = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(removedTableGrid.Name.Replace("grid", "")))).FirstOrDefault();
            tableCoordinatesList.Remove(removedCoordinatesTable);

            var removedTable = addedTables.Where(x => x.Код.Equals(Convert.ToInt16(removedTableGrid.Name.Replace("grid", "")))).FirstOrDefault();
            addedTables.Remove(removedTable);

            FilterTables();
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
            Grid grid = sender as Grid;
            Shape shape = grid.Children.OfType<Shape>().FirstOrDefault();
            shape.Stroke = Brushes.LightGreen;
            Cursor = Cursors.Hand;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            Shape shape = grid.Children.OfType<Shape>().FirstOrDefault();
            shape.Stroke = Brushes.Gray;
            Cursor = Cursors.Arrow;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            xMousePosition = Convert.ToInt16(e.GetPosition(HallGrid).X) - diameterTable / 2;
            yMousePosition = Convert.ToInt16(e.GetPosition(HallGrid).Y) - diameterTable / 2;

            txtbX.Text = Convert.ToString(xMousePosition);
            txtbY.Text = Convert.ToString(yMousePosition);

            if (selectedGrid != null && e.RightButton == MouseButtonState.Pressed)
            {
                selectedGrid.RenderTransform = new TranslateTransform(xMousePosition, yMousePosition);
                tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();
                tableCoordinatesList.Remove(tableParameter);
                tableCoordinatesList.Add(new TableParameter(tableParameter.Number, xMousePosition, yMousePosition, angleTable) { Number = tableParameter.Number, X = xMousePosition, Y = yMousePosition, Angle = angleTable });
                lw.ItemsSource = tableCoordinatesList.ToList();
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedGrid = sender as Grid;
            tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();

            xCoordinate = tableParameter.X;
            yCoordinate = tableParameter.Y;
            angleTable = tableParameter.Angle;
        }


        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedGrid != null)
            {
                var s = selectedGrid;
                tableParameter = tableCoordinatesList.Where(x => x.Number.Equals(Convert.ToInt16(selectedGrid.Name.Replace("grid", "")))).FirstOrDefault();
                tableCoordinatesList.Remove(tableParameter);
                if (xMousePosition < indent || yMousePosition < indent || CorrectCoordinates(xMousePosition, yMousePosition, diameterTable + indent, tableCoordinatesList) == false)
                {
                    MessageBox.Show("Внимание2");
                    s.RenderTransform = new TranslateTransform(xCoordinate, yCoordinate);
                    tableCoordinatesList.Add(new TableParameter(tableParameter.Number, xCoordinate, yCoordinate, angleTable) { Number = tableParameter.Number, X = xCoordinate, Y = yCoordinate, Angle = angleTable });
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

            Стол addTable = databaseEntities.Стол.Where(d => d.Наименование.Equals(TableComboBox.Text)).FirstOrDefault();

            if (addTable == null)
            {
                MessageBox.Show("Внимание3");
                return;
            }

            int x = indent, y = indent;

            if (tableCoordinatesList.Count == 0)
            {

                tableCoordinatesList.Add(new TableParameter(addTable.Код, x, y, false) { Number = addTable.Код, X = x, Y = y, Angle = false });
            }
            else
            {
                bool corectly = false;
                while (corectly == false)
                {
                    if (CorrectCoordinates(x, y, diameterTable + indent, tableCoordinatesList) == false)
                    {
                        if (x < 1000)
                        {
                            x += diameterTable;
                        }
                        else
                        {
                            y += diameterTable;
                            x = indent;
                            if (y + indent > HallGrid.Height)
                            {
                                HallGrid.Height = y + indent + diameterTable;
                            }
                        }
                    }
                    else
                    {
                        tableCoordinatesList.Add(new TableParameter(addTable.Код, x, y, false) { Number = addTable.Код, X = x, Y = y, Angle = false });
                        corectly = true;
                    }
                }
            }

            grid = new Grid() // контейнер (хранит все элементы стола)
            {
                Height = diameterTable,
                Width = diameterTable,
                RenderTransform = new TranslateTransform(x, y),
                Name = "grid" + addTable.Код.ToString(),
            };

            grid.MouseEnter += Window_MouseEnter;
            grid.MouseLeave += Window_MouseLeave;
            grid.MouseDown += Window_MouseRightButtonDown;

            if (addTable.ФормаСтола == 1)
            {
                Ellipse ellipse = new Ellipse() // стол (круглый, квадратный и др.)
                {
                    Fill = Brushes.LightBlue,
                    Width = diameterTable,
                    Height = diameterTable,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 3
                };

                grid.Children.Add(ellipse);
            }
            else if (addTable.ФормаСтола > 1)
            {
                RotateTransform rotateTransform = new RotateTransform(0);
                if (TiltAngleCheckBox.IsChecked == true)
                {
                    rotateTransform = new RotateTransform(45, diameterTable / 2, diameterTable / 2);
                    tableCoordinatesList[tableCoordinatesList.Count - 1].Angle = true;
                }
                Rectangle rectangle = new Rectangle() // стол (круглый, квадратный и др.)
                {
                    Fill = Brushes.LightBlue,
                    Width = diameterTable,
                    Height = diameterTable,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 3,
                    RenderTransform = rotateTransform
                };
                grid.Children.Add(rectangle);
            }

            StackPanel stackPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            TextBlock nameTextBlock = new TextBlock()
            {
                Text = addTable.Наименование,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock numberTextBlock = new TextBlock()
            {
                Text = addTable.КоличествоПерсон.ToString() + " гостей",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 12
            };

            Button button = new Button()
            {
                Width = diameterTable / 2 + 10,
                Height = diameterTable / 5 + 5,
                Content = "Удалить",
                FontSize = 14,
                Background = Brushes.AliceBlue,
            };
            button.Click += DeleteButton1_Click;

            stackPanel.Children.Add(nameTextBlock);
            stackPanel.Children.Add(numberTextBlock);
            stackPanel.Children.Add(button);

            grid.Children.Add(stackPanel);

            HallGrid.Children.Add(grid);

            addedTables.Add(addTable);
            FilterTables();

            quantity++;

            lw.ItemsSource = tableCoordinatesList.ToList();
        }

        private void SaveHallButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (tableCoordinatesList.Count == 0)
            {
                errors.AppendLine("Необходимо добавить хотя бы один стол");
            }
            else
            {
                var activeHallsList = databaseEntities.Зал.Where(x => x.Статус.Equals(true)).ToList();
                List<Стол> inaccessibleTableList = new List<Стол>();

                for (int i = 0; i < activeHallsList.Count; i++)
                {
                    var tableActiveHalls = databaseEntities.ЗалСтол.ToList().Where(x => x.КодЗала.Equals(activeHallsList[i].Код)).ToList();
                    for (int q = 0; q < tableActiveHalls.Count; q++)
                    {
                        var inaccessibleTable = addedTables.Where(x => x.Код.Equals(tableActiveHalls[q].КодСтола)).Except(inaccessibleTableList).FirstOrDefault();

                        if (inaccessibleTable != null)
                        {
                            inaccessibleTableList.Add(inaccessibleTable);
                            errors.AppendLine($"{activeHallsList[i].Наименование} сейчас активен и содержит стол {inaccessibleTable.Наименование}");
                        }
                    }
                }
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            databaseEntities.Зал.Add(new Зал
            {
                Код = databaseEntities.Зал.Count(),
                Наименование = NameHallTextBox.Text,
                Статус = true
            });

            for (int i = 0; i < tableCoordinatesList.Count; i++)
            {
                databaseEntities.ЗалСтол.Add(new ЗалСтол
                {
                    КодЗала = databaseEntities.Зал.Count(),
                    КодСтола = tableCoordinatesList[i].Number,
                    КоординатаX = tableCoordinatesList[i].X,
                    КоординатаY = tableCoordinatesList[i].Y,
                    Диагональ = tableCoordinatesList[i].Angle
                });
            }
            try
            {
                databaseEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTables();

            //if (TableTypeComboBox.SelectedIndex > 1)
            //{
            //    TiltAngleCheckBox.IsEnabled = true;
            //}
            //else
            //{
            //    TiltAngleCheckBox.IsEnabled = false;
            //    TiltAngleCheckBox.IsChecked = false; 
            //}
        }
        private void FilterTables()
        {
            List<Стол> itemTableList = databaseEntities.Стол.ToList();
            if (TableTypeComboBox.SelectedIndex > 0)
            {
                itemTableList = itemTableList.Where(x => x.ФормаСтола.Equals(Convert.ToByte(TableTypeComboBox.SelectedIndex))).ToList();
            }
            itemTableList = itemTableList.Except(addedTables).ToList();
            //itemTableList.Insert(0, new Стол
            //{
            //    Наименование = "Выберите стол"
            //});
            TableComboBox.ItemsSource = itemTableList.ToList();
            TableComboBox.DisplayMemberPath = "Наименование";
            TableComboBox.SelectedIndex = 0;
        }

        private void ViewHallButton_Click(object sender, RoutedEventArgs e)
        {
            HallWindow hallWindow = new HallWindow();
            hallWindow.Show();
        }

        private void ChangeTableButton_Click(object sender, RoutedEventArgs e)
        {
            TableWindow tableWindow = new TableWindow();
            tableWindow.Show();
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