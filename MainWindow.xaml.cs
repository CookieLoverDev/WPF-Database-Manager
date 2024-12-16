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
    /// This is the logic code for the main window
    /// It consists of the methods, which are "SaveEntry", two methods for opening View Window and Search Window, "ClearDatabase" and "InitializeDatabase"
    /// Initialize Database is needed, to create a database in the folder, if it is non-existant
    /// Save Entry is used for saving the provided record in the database
    /// Clear Database as the name says, clears all the records in the database, but does not reset it.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _dbName = "mainDB.db";
        private const string _connectionString = $"Data Source={_dbName};Version=3;";

        private string _id = string.Empty;
        private string _name = string.Empty;
        private string _surname = string.Empty;
        private string _email = string.Empty;
        private string _role = string.Empty;
        private string _description = string.Empty;

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
            _id = idBox.Text;
            _name = fname.Text;
            _surname = surname.Text;
            _email = email.Text;
            _role = role.Text;
            _description = description.Text;

            if (new[] {_id, _name, _surname, _email, _role, _description}.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Please, fill out every form", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO info (PersonID, Name, Surname, Email, Role, Description) VALUES (@PersonID, @Name, @Surname, @Email, @Role, @Description)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", _id);
                        command.Parameters.AddWithValue("@Name", _name);
                        command.Parameters.AddWithValue("@Surname", _surname);
                        command.Parameters.AddWithValue("@Email", _email);
                        command.Parameters.AddWithValue("@Role", _role);
                        command.Parameters.AddWithValue("@Description", _description);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("File is succesfully saved!");
                idBox.Clear();
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
                    PersonID TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    Description TEXT NOT NULL
                );";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
