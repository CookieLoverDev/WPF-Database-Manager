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
using System.Data;
using System.Xml.Linq;

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
            InitialProcess();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT PersonID, Name, Surname, Email, Role, Description FROM info WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", searchBox.Text);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idBox.Text = reader.GetString(0);
                            nameBox.Text = reader.GetString(1);
                            surnameBox.Text = reader.GetString(2);
                            emailBox.Text = reader.GetString(3);
                            roleBox.Text = reader.GetString(4);
                            descriptionBox.Text = reader.GetString(5);
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
            if (idBox.Text == string.Empty)
            {
                MessageBox.Show("To delete someone, you gotta search for someone first!", "Ты идиот?", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this person?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM info WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", searchBox.Text);
                        command.ExecuteNonQuery();

                        searchBox.Clear();
                        idBox.Clear();
                        nameBox.Clear();
                        surnameBox.Clear();
                        emailBox.Clear();
                        roleBox.Clear();
                        descriptionBox.Clear();
                    }
                }
            }
            else
                return;
        }

        private void EditPerson(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Welcome to Edit Mode", "Entered Edit Mode", MessageBoxButton.OK, MessageBoxImage.Information);
            EditPreset();
        }

        private void SavePerson(object sender, RoutedEventArgs e)
        {
            string id = idBox.Text;
            string name = nameBox.Text;
            string surname = surnameBox.Text;
            string email = emailBox.Text;
            string role = roleBox.Text;
            string description = descriptionBox.Text;

            if (new[] { id, name, surname, email, role, description }.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("The fields can not be empty", "Ну ты идиот?", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE info SET PersonID = @PersonID, Name = @Name, Surname = @Surname, Email = @Email, Role = @Role WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Id", searchBox.Text);

                    command.ExecuteNonQuery();
                }
            }

            InitialProcess();
        }

        private object GetContent()
        {
            return EditBtn.Content;
        }

        private void GoToMain(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void EditPreset()
        {
            searchBox.IsEnabled = false;
            EditBtn.IsEnabled = false;

            SaveBtn.IsEnabled = true;
            idBox.IsEnabled = true;
            nameBox.IsEnabled = true;
            surnameBox.IsEnabled = true;
            emailBox.IsEnabled = true;
            roleBox.IsEnabled = true;
            descriptionBox.IsEnabled = true;
        }

        private void InitialProcess()
        {
            searchBox.IsEnabled = true;

            SaveBtn.IsEnabled = false;
            idBox.IsEnabled = false;
            nameBox.IsEnabled = false;
            surnameBox.IsEnabled = false;
            emailBox.IsEnabled = false;
            roleBox.IsEnabled = false;
            descriptionBox.IsEnabled = false;
        }
    }
}
