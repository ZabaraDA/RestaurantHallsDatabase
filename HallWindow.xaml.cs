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
    public partial class HallWindow : Window
    {
        int diameterTable = 100;
        RestaurantHallsDatabaseEntities databaseEntities = new RestaurantHallsDatabaseEntities();
        public HallWindow()
        {
            InitializeComponent();

            HallComboBox.ItemsSource = databaseEntities.Зал.ToList();
            HallComboBox.DisplayMemberPath = "Наименование";
            HallComboBox.SelectedIndex = 0;
        }

        private void ViewHallButton_Click(object sender, RoutedEventArgs e)
        {
            HallGrid.Children.Clear();
            
            Зал selectedHall = databaseEntities.Зал.Where(x => x.Наименование.Equals(HallComboBox.Text)).FirstOrDefault();

            List<ЗалСтол> tableHallList = databaseEntities.ЗалСтол.Where(x => x.КодЗала.Equals(selectedHall.Код)).ToList();
            TableListView.ItemsSource = tableHallList.ToList();

            for(int i = 0; i < tableHallList.Count; i++)
            {
                var tableHall = tableHallList[i];

                Grid grid = new Grid() // контейнер (хранит все элементы стола)
                {
                    Height = diameterTable,
                    Width = diameterTable,
                    RenderTransform = new TranslateTransform(tableHall.КоординатаX, tableHall.КоординатаY),
                    Name = "grid" + tableHall.КодСтола.ToString(),
                };

                //grid.MouseEnter += Window_MouseEnter;
                //grid.MouseLeave += Window_MouseLeave;
                //grid.MouseDown += Window_MouseRightButtonDown;

                if (tableHall.Стол.ФормаСтола == 1)
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
                else if (tableHall.Стол.ФормаСтола == 2)
                {
                    RotateTransform rotateTransform = new RotateTransform(0);
                    if (tableHall.Диагональ == true)
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

                TextBlock textBlock = new TextBlock()
                {
                    Text = tableHall.Стол.Наименование,
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
                //button.Click += DeleteButton1_Click;

                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(button);

                grid.Children.Add(stackPanel);

                HallGrid.Children.Add(grid);
            }
        }
    }
}
