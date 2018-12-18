using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
using DBBankSystemControlLibrary;
using MahApps.Metro.Controls;

namespace BankSystemPersonnel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private string _connStr;
        private DBBSContext _dbBankSystemControl;

        public MainWindow()
        {
            InitializeDBControl();
            InitializeComponent();
        }

        public void InitializeDBControl()
        {
            _connStr = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
            _dbBankSystemControl = DBBSContext.GetInstance(_connStr);
            _dbBankSystemControl.Database.Initialize(false);
        }
    }

    public class PersonellControl
    {

    }
}
