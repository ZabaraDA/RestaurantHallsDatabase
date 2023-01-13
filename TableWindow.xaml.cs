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
using WpfApp7.Databases;

namespace WpfApp7
{
    /// <summary>
    /// Логика взаимодействия для TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window
    {
        RestaurantHallsDatabaseEntities databaseEntities = new RestaurantHallsDatabaseEntities();
        private int diameterTable = 200;

        public TableWindow()
        {
            InitializeComponent();
            //CreateTableView();
            FilterTable();
            StateTableComboBox.SelectedIndex = 0;
            TableTypeComboBox.SelectedIndex = 0;
            ShapeTableComboBox.SelectedIndex = 0;
            StateTableComboBox1.SelectedIndex = 0;
        }

        private void AddTableButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeTableButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            FilterTable();
        }

        private void FilterTable()
        {
            List<Стол> tableList = databaseEntities.Стол.ToList();

            if (StateTableComboBox.SelectedIndex > 0)
            {
                if(StateTableComboBox.SelectedIndex == 1)
                {
                    tableList = tableList.Where(x => x.Статус.Equals(true)).ToList();
                }
                else
                {
                    tableList = tableList.Where(x => x.Статус.Equals(false)).ToList();
                }
            }
            if (TableTypeComboBox.SelectedIndex > 0)
            {
                if (TableTypeComboBox.SelectedIndex == 1)
                {
                    tableList = tableList.Where(x => x.ФормаСтола.Equals(1)).ToList();
                }
                else if(TableTypeComboBox.SelectedIndex == 2)
                {
                    tableList = tableList.Where(x => x.ФормаСтола.Equals(2)).ToList();
                }
            }
            TableListView.ItemsSource = tableList;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CreateTableView();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateTableView();
        }

        private void CreateTableView()
        {
            HallCanvas.Children.Clear();

            Grid grid = new Grid() // контейнер (хранит все элементы стола)
            {
                Height = diameterTable,
                Width = diameterTable,   
                //HorizontalAlignment = HorizontalAlignment.Center,
                //VerticalAlignment = VerticalAlignment.Center,
                RenderTransform = new TranslateTransform(50,50)
            };

            if (ShapeTableComboBox.SelectedIndex == 0)
            {
                Ellipse ellipse = new Ellipse() 
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
            else if (ShapeTableComboBox.SelectedIndex == 1)
            {
                RotateTransform rotateTransform = new RotateTransform(0);
                if (TiltAngleCheckBox.IsChecked == true)
                {
                    rotateTransform = new RotateTransform(45, diameterTable / 2, diameterTable / 2);
                }
                Rectangle rectangle = new Rectangle() 
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
                Text = NameTableTextBox.Text,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.WrapWithOverflow,
                Width = diameterTable - 10,
            };

            TextBlock numberTextBlock = new TextBlock()
            {
                Text = NumberGuestsTableTextBox.Text + " гостей",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 24
            };

            //Button button = new Button()
            //{
            //    Width = diameterTable / 2 + 10,
            //    Height = diameterTable / 5 + 5,
            //    Content = "Удалить",
            //    FontSize = 14,
            //    Background = Brushes.AliceBlue,
            //};
            

            stackPanel.Children.Add(nameTextBlock);
            stackPanel.Children.Add(numberTextBlock);
            //stackPanel.Children.Add(button);

            grid.Children.Add(stackPanel);

            HallCanvas.Children.Add(grid);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CreateTableView();
        }
    }
}
