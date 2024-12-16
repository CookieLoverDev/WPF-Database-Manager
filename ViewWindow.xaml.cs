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
    /// This code is the logic for the view window, which is responsibel for viewing, loading, editing and deleting records 
    /// There are few methods responsible for the main logic of the View Window: NextPerson, PreviousPerson, SaveEdit, LoadPerson, LoadPage, CalculateTotalPages, CalculateOffset
    /// 
    /// The records are divided into pages of ten records to optimize the loading. The load time has O(n) complexity, and if we loaded all the records in one time,
    /// it could negatively affect the load time, as the number of records get high.
    /// In the beginning, when the window is opened, we are calculating the total pages number by dividing total rows number in the database by the page size and rounding it to the upper limit
    /// After it has been calculated, we are loading the page to the list where the indexes of the records are stored
    /// Inside the page load method, we are calculating the offset(how many rows should be skipped, if we are not in the initial page)
    /// 
    /// When we are pressing the controll buttons(Next and Previous) we are changing the current index. Current Index is the position in the list with the Records ID.
    /// It is made, in case the database is reseted, and the databases ID are weird?
    /// After the press of the button, the next or previous person of the list loaded by the stored ID there 
    ///
    /// When we reach one of the ends in the list, the page is being updated, and depending if previous or next page is loaded, the current ID is set to 10 or 0, respectively
    /// 
    /// Save method works like any other one in the other windows
    /// </summary>
    public partial class ViewWindow : Window
    {
        private string _personID = string.Empty;
        private string _name = string.Empty;
        private string _surname = string.Empty;
        private string _email = string.Empty;
        private string _role = string.Empty;
        private string _description = string.Empty;

        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _currentID = -1;
        private int _totalPages;
        private List<int> _currentRecords = new List<int>();

        private const string _dbName = "mainDB.db";
        private const string _connectionString = $"Data Source={_dbName};Version=3;";

        public ViewWindow()
        {
            InitializeComponent();
            InitialProcess();
            _totalPages = CalculateTotalPages();
            UpdatePage();
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
            if (_currentID < _currentRecords.Count)
            {
                int id = _currentRecords[_currentID];
                LoadPerson(id);
            }
            else if (_currentID >= _currentRecords.Count)
            {
                _currentPage++;
                UpdatePage();

                _currentID = 0;
                int id = _currentRecords[_currentID];
                LoadPerson(id);
            }
        }

        private void PreviousPerson(object sender, EventArgs e)
        {
            _currentID--;
            if (_currentID >= 0)
            {
                int id = _currentRecords[_currentID];
                LoadPerson(id);
            }
            else if (_currentID < 0)
            {
                _currentPage--;
                UpdatePage();

                _currentID = _currentRecords.Count - 1;
                int id = _currentRecords[_currentID];
                LoadPerson(id);
            }
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
            string id = idBox.Text;
            string name = nameBox.Text;
            string surname = surnameBox.Text;
            string email = emailBox.Text;
            string role = roleBox.Text;
            string description = descriptionBox.Text;

            if (new[] { id, name, surname, email, role, description }.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Please, fill out every form", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE info SET PersonID = @PersonID, Name = @Name, Surname = @Surname, Email = @Email, Role = @Role, Description = @Description WHERE Id = @Id";
                try
                {
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", id);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Surname", surname);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Role", role);
                        command.Parameters.AddWithValue("@Description", description);
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

        private void UpdatePage()
        {
            _currentRecords.Clear();
            if (_currentPage > _totalPages || _currentPage <= 0)
            {
                MessageBox.Show("That was all records!", "Out of bound", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    int offset = CalculateOffsett();
                    string query = "SELECT Id FROM info ORDER BY Id LIMIT @PageSize OFFSET @Offsett";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PageSize", _pageSize);
                        command.Parameters.AddWithValue("@Offsett", offset);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _currentRecords.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
        }

        private void LoadPerson(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = $"SELECT PersonID, Name, Surname, Email, Role, Description FROM info WHERE Id=@Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _personID = reader.GetString(0);
                            _name = reader.GetString(1);
                            _surname = reader.GetString(2);
                            _email = reader.GetString(3);
                            _role = reader.GetString(4);
                            _description = reader.GetString(5);
                        }
                    }
                }
            }

            idBox.Text = _personID;
            nameBox.Text = _name;
            surnameBox.Text = _surname;
            emailBox.Text = _email;
            roleBox.Text = _role;
            descriptionBox.Text = _description;
        }

        private int CalculateOffsett()
        {
            int offsett = (_pageSize * (_currentPage - 1));
            return offsett;
        }

        private int CalculateTotalPages()
        {
            int _totalRecords = 0;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM info";
                using (var command = new SQLiteCommand(query, connection))
                {
                    _totalRecords = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return (int)Math.Ceiling((double)_totalRecords / _pageSize);
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
