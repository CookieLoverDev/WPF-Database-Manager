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
        private const String _dbName = "mainDB.db";
        private const String _connection = $"Data Source={_dbName};Version=3;";

        private string _title;
        private string _content;

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(_dbName))
            {
                InitializeDatabase();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _title = title.Text;
            _content = content.Text;

            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Open();
                string query = $"INSERT INTO info (Title, Content) VALUES (@Title, @Content)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", _title);
                    command.Parameters.AddWithValue("@Content", _content);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("File is succesfully saved!");
            title.Clear();
            content.Clear();
        }

        private void InitializeDatabase()
        {
            Console.WriteLine("I was executed");
            SQLiteConnection.CreateFile(_dbName);
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Open();
                var query = @"
                    Create TABLE info (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Content TEXT NOT NULL,
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
