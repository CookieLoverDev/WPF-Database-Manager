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
using System.Data.SQLite;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private const string _dbName = "mainDB.db";
        private const string _connectionString = $"Data Source={_dbName};Version=3;";

        public SearchWindow()
        {
            InitializeComponent();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Name, Surname, Email, Role, Description FROM info WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", searchBox.Text);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idBox.Text = searchBox.Text;
                            nameBox.Text = reader.GetString(0);
                            surnameBox.Text = reader.GetString(1);
                            emailBox.Text = reader.GetString(2);
                            roleBox.Text = reader.GetString(3);
                            descriptionBox.Text = reader.GetString(4);
                        }
                        else
                        {
                            MessageBox.Show("The ID you have provided is not valid!", "Error Occured", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void DeletePerson(object sender, RoutedEventArgs e)
        {

        }

        private void Exit(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
