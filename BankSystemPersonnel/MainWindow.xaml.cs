using System;
using System.Collections;
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
using MahApps.Metro.Controls.Dialogs;

namespace BankSystemPersonnel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        PersonellControl personell = new PersonellControl();


        public MainWindow()
        {
            PersonellControl.InitializeDBControl();
            InitializeComponent();
            //SetDBData();
        }

        //void SetDBData()
        //{
        //    BankAccount bankAccount1 = new BankAccount
        //    {
        //        Created = DateTime.Now.AddMonths(-2),
        //        CreditFunds = 0m,
        //        PersonalFunds = 20000m,
        //        IsClosed = false
        //    };
        //    Account acc1 = new Account
        //    {
        //        CreateDateTime = DateTime.Now.AddMonths(-2),
        //        IsBlocked = false,
        //        Login = "valentpopov"
        //    };
        //    acc1.SetPassword("123456789");
        //    acc1.SetBankAccounts(new[] { bankAccount1 });


        //    User user1 = new User
        //    {
        //        BirthDate = new DateTime(1993, 8, 12),
        //        FirstName = "Valentin",
        //        LastName = "Popov",
        //    };
        //    user1.SetAccount(acc1);
        //    personell.AddUser(user1, acc1, bankAccount1);
        //    BankAccount bankAccount2 = new BankAccount
        //    {
        //        Created = DateTime.Now.AddMonths(-3),
        //        CreditFunds = 5000m,
        //        PersonalFunds = 30000m,
        //        IsClosed = false
        //    };
        //    BankAccount bankAccount3 = new BankAccount
        //    {
        //        Created = DateTime.Now.AddMonths(-1),
        //        CreditFunds = 1000m,
        //        PersonalFunds = 1000m,
        //        IsClosed = false
        //    };
        //    Account acc2 = new Account
        //    {
        //        CreateDateTime = DateTime.Now.AddMonths(-2),
        //        IsBlocked = false,
        //        Login = "uivanov"
        //    };
        //    acc2.SetPassword("123456789");
        //    acc2.SetBankAccounts(new[] { bankAccount2, bankAccount3 });

        //    User user2 = new User
        //    {
        //        BirthDate = new DateTime(1990, 5, 20),
        //        FirstName = "Yuri",
        //        LastName = "Ivanov"
        //    };
        //    user2.SetAccount(acc2);
        //    personell.AddUser(user2, acc2, bankAccount2);

        //}

        private void SearchUser_OnClick(object sender, RoutedEventArgs e)
        {
            ListUsers.ItemsSource = personell.SearchUsers(SearchQueryBox.Text);
        }

        private void ShowAllUser_OnClick(object sender, RoutedEventArgs e)
        {
            List<User> users = personell.GetAllUsers();
            ListUsers.ItemsSource = users;
        }

        private void ShowAllAccounts_OnClick(object sender, RoutedEventArgs e)
        {
            List<Account> accounts = personell.GetAllAccounts();
            ListUsers.ItemsSource = accounts;
        }


        private void ShowAccountMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ListUsers.ItemsSource = personell.SearchAccount(ListUsers.SelectedItem as User);
        }

        private async void ListUsers_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                MetroDialogSettings settings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "OK",
                };
                DBBSContext ctx = DBBSContext.GetInstance();
                ctx.SaveChanges();
                var msg = await this.ShowMessageAsync("Изменение данных", "Изменения успешно сохранены",
                    MessageDialogStyle.Affirmative, settings);
            }
        }


        private void ListUsers_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(Account) || e.PropertyName.Contains("Id")|| e.PropertyName.Contains("Account")|| e.PropertyName.Contains("Password"))
            {
                e.Cancel=true;
                return;
            }
            if (e.PropertyName.Contains("Funds")||e.PropertyName.Contains("Create"))
            {
                e.Column.IsReadOnly = true;
            }

            if (e.PropertyType == typeof(System.DateTime))
            {
                if(e.PropertyName.Contains("Birth"))
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
                else
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy hh:mm:ss";
            }
                
        }

        private void ShowBankAccountMenuItem_OnClick(object sender, RoutedEventArgs e)
        {

            if (ListUsers.SelectedItem is User)
            {
                User user = ListUsers.SelectedItem as User;
                ListUsers.ItemsSource = personell.SearchBankAccount(user);
            }
            else
            {
                Account account = ListUsers.SelectedItem as Account;
                ListUsers.ItemsSource = personell.SearchBankAccount(account);
            }
            
        }

        private void SearchAccount_OnClick(object sender, RoutedEventArgs e)
        {
            ListUsers.ItemsSource = personell.SearchAccount(SearchQueryBox.Text);
        }
    }

    public class PersonellControl
    {
        private static string _connStr;
        private static DBBSContext _dbBankSystemControl;

        public static void InitializeDBControl()
        {
            _connStr = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
            _dbBankSystemControl = DBBSContext.GetInstance(_connStr);
            try
            {
                _dbBankSystemControl.Database.Initialize(true);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public List<User> SearchUsers(string searchQuery)
        {
            return _dbBankSystemControl.Users.Where(user =>
               user.FirstName.Contains(searchQuery) || user.LastName.Contains(searchQuery) || user.BirthDate.ToString().Contains(searchQuery)).ToList();
        }
        public List<Account> SearchAccount(User user)
        {
            return _dbBankSystemControl.Accounts.Where(account =>
                user.AccountId == account.Id).ToList();
        }
        public List<Account> SearchAccount(string searchQuery)
        {
            return _dbBankSystemControl.Accounts.Where(account =>
                account.Login.Contains(searchQuery) || account.CreateDateTime.ToString().Contains(searchQuery)).ToList();
        }

        public List<User> GetAllUsers()
        {
            return _dbBankSystemControl.Users.ToList();
        }
        public List<Account> GetAllAccounts()
        {
            return _dbBankSystemControl.Accounts.ToList();
        }
        public List<BankAccount> SearchBankAccount(User user)
        {
            return SearchBankAccount(_dbBankSystemControl.Accounts.FirstOrDefault(account =>
                user.AccountId == account.Id));
        }

        public List<BankAccount> SearchBankAccount(Account account)
        {
            return account.BankAccounts.ToList();
        }
    

        public void AddUser(User user, Account account, BankAccount bankAccount)
        {
            _dbBankSystemControl.Users.Add(user);
            _dbBankSystemControl.Accounts.Add(account);
            _dbBankSystemControl.BankAccounts.Add(bankAccount);
            _dbBankSystemControl.SaveChanges();
        }
    }
}
