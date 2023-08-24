using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using Microsoft.Data.SqlClient;
using Shop.DAL.Entity;

namespace Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection connection;
        public ObservableCollection<string> columns { get; set; } = new();
        public ObservableCollection<DAL.Entity.Product> Products { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            connection = null!;
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connection = new(App.ConnectionString);
                connection.Open();
                LoadGroups();
        }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                //Close();
            }
}

        private void Window_Closed(object sender, EventArgs e)
        {
            connection?.Dispose();
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand command = new();
            command.Connection = connection;
            command.CommandText = @"CREATE TABLE Products (
                                    Id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                    Name        NVARCHAR(50)     NOT NULL,
                                    Category    NVARCHAR(50)     NULL,
                                    Price       numeric(18,0)    NULL,
                                    Quantity    INT              NULL
                                    )";
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Table created");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertGroup_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand command = new();
            command.Connection = connection;
            command.CommandText = @"INSERT INTO Products
                                    ( Id, Name, Category, Price, Quantity)
                                VALUES
                                ('089015F4-31B5-4F2B-BA05-A813B5419285', N'Кефір', N'Молочні', N'29.50', N'20'),
                                ('A6D7858F-6B75-4C75-8A3D-C0B373828558', N'Молоко',   N'Молочні', N'32.00', N'0'),
                                ('DEF24080-00AA-440A-9690-3C9267243C43', N'Багет',  N'Хлібобулочні', N'24.99', N'15'),
                                ('2F9A22BC-43F4-4F73-BAB1-9801052D85A9', N'Свинина', N'Мясні', N'125.30', N'3'),
                                ('D6D9783F-2182-469A-BD08-A24068BC2A23', N'Яловичина', N'Мясні', N'145.00', N'12')";
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Date inserted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Insertation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CountGroup_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT COUNT(*) FROM Products";
            try
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                MessageBox.Show($"Table has {count} rows");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Query error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGroups()
        {
            using SqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM Products";
            try
            {
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    Products.Add(new() { Id = reader.GetGuid(0), Name = reader.GetString(1), Category = reader.GetString(2), Price = reader.GetDouble(3), Quantity = reader.GetInt32(4) });

                    //columns.Add($"Id: {reader.GetGuid(0).ToString()[..4]}, Name: {reader.GetString(1)}, Description: {reader.GetString(2)}, Picture: {reader.GetString(3)}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Query error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Product prod)
                {
                    MessageBox.Show(Convert.ToString(prod.Price));
                }
            }
        }
    }
}
