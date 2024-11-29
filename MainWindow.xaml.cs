using System;
using System.Collections.Generic;
using System.IO;
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
using System.Data.SQLite;
using System.Reflection.Metadata;
using System.Data.SqlClient;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _dbName = "mainDB.db";
        private const string _connectionString = $"Data Source={_dbName};Version=3;";

        private string _name;
        private string _surname;
        private string _email;
        private string _role;
        private string _description;

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(_dbName))
            {
                InitializeDatabase();
            }
        }

        private void SaveEntry(object sender, RoutedEventArgs e)
        {
            _name = fname.Text;
            _surname = surname.Text;
            _email = email.Text;
            _role = role.Text;
            _description = description.Text;

            if ((_name == string.Empty) || (_surname == string.Empty) || (_email == string.Empty) || (_role == string.Empty) || (_description == string.Empty))
            {
                MessageBox.Show("Please, fill out every form", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO info (Name, Surname, Email, Role, Description) VALUES (@Name, @Surname, @Email, @Role, @Description)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", _name);
                        command.Parameters.AddWithValue("@Surname", _surname);
                        command.Parameters.AddWithValue("@Email", _email);
                        command.Parameters.AddWithValue("@Role", _role);
                        command.Parameters.AddWithValue("@Description", _description);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("File is succesfully saved!");
                fname.Clear();
                surname.Clear();
                email.Clear();
                role.Clear();
                description.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Entries were not saved in the database! Following error has occured {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewWindowOpen(object sender, RoutedEventArgs e)
        {
            ViewWindow viewWindow = new ViewWindow();
            viewWindow.Show();
            this.Close();
        }

        private void SearchWindowOpen(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.Show();
            this.Close();
        }

        private void ClearDatabase(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the database? All the data will be erased!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM info";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
                return;
        }

        private void InitializeDatabase()
        {
            SQLiteConnection.CreateFile(_dbName);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    Create TABLE info (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Creation_Date DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
