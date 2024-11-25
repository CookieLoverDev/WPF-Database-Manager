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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        private int _currentID = 0;
        private int _maximumID;

        private string _name;
        private string _surname;
        private string _email;
        private string _role;
        private string _description;

        private const string _dbName = "mainDB.db";
        private const string _connectionString = $"Data Source={_dbName};Version=3;";

        public ViewWindow()
        {
            InitializeComponent();
        }

        private void ExitToMain(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void NextPersonBtn(object sender, EventArgs e)
        {
            _currentID++;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = $"SELECT Name, Surname, Email, Role, Description FROM info WHERE Id=@Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", _currentID);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _name = reader.GetString(0);
                            _surname = reader.GetString(1);
                            _email = reader.GetString(2);
                            _role = reader.GetString(3);
                            _description = reader.GetString(4);
                        }
                        else
                        {
                            MessageBox.Show("Thats all folks", "Out of bound error", MessageBoxButton.OK, MessageBoxImage.Error);
                            _currentID--;
                        }
                    }
                }
            }

            idBox.Text = _currentID.ToString();
            nameBox.Text = _name;
            surnameBox.Text = _surname;
            emailBox.Text = _email;
            roleBox.Text = _role;
            descriptionBox.Text = _description;
        }

        private void PreviousPerson(object sender, EventArgs e)
        {
            _currentID--;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = $"SELECT Name, Surname, Email, Role, Description FROM info WHERE Id=@Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", _currentID);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _name = reader.GetString(0);
                            _surname = reader.GetString(1);
                            _email = reader.GetString(2);
                            _role = reader.GetString(3);
                            _description = reader.GetString(4);
                        }
                        else
                        {
                            MessageBox.Show("Thats all folks", "Out of bound error", MessageBoxButton.OK, MessageBoxImage.Error);
                            _currentID = 0;
                        }
                    }
                }
            }

            idBox.Text = _currentID.ToString();
            nameBox.Text = _name;
            surnameBox.Text = _surname;
            emailBox.Text = _email;
            roleBox.Text = _role;
            descriptionBox.Text = _description;
        }
    }
}
