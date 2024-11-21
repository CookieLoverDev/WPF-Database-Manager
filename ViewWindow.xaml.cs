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

namespace Text_Editor
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        private int _currentID = 0;
        private int _maximumID;

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

        }

        private void ChangeThePerson()
        {

        }
    }
}
