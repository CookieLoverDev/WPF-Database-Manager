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
            InitialProcess();
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

        private void EditPerson(object sender, EventArgs e)
        {
            NextBtn.IsEnabled = false;
            BackBtn.IsEnabled = false;
            ExitBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
            SaveBtn.IsEnabled = true;

            idBox.IsEnabled = true;
            nameBox.IsEnabled = true;
            surnameBox.IsEnabled = true;
            emailBox.IsEnabled = true;
            roleBox.IsEnabled = true;
            descriptionBox.IsEnabled = true;

            MessageBox.Show("You entered the Edit mode. To finish or to exit it, please press the 'Save' button!", "Inofrmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveEdit(object sender, EventArgs e)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE info SET Name = @Name, Surname = @Surname, Email = @Email, Role = @Role, Description = @Description WHERE Id = @Id";
                try
                {
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", nameBox.Text);
                        command.Parameters.AddWithValue("@Surname", surnameBox.Text);
                        command.Parameters.AddWithValue("@Email", emailBox.Text);
                        command.Parameters.AddWithValue("@Role", roleBox.Text);
                        command.Parameters.AddWithValue("@Description", descriptionBox.Text);
                        command.Parameters.AddWithValue("@Id", _currentID);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Information succesfully updated!", "Great Success!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was a problem updating information. Problem occured {ex}", "Not so great success", MessageBoxButton.OK , MessageBoxImage.Error);
                }
            }

            NextBtn.IsEnabled = true;
            BackBtn.IsEnabled = true;
            ExitBtn.IsEnabled = true;
            EditBtn.IsEnabled = true;
            InitialProcess();
        }

        private void InitialProcess()
        {
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
