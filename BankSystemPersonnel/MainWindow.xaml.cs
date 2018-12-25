using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
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
        private IEnumerable _itemSourceBackUp;
        public MainWindow()
        {
            PersonellControl.InitializeDBControl();
            InitializeComponent();
            SetDBData();
            PersonellAuthorization();
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            dpd?.AddValueChanged(ListUsers, ItemSourcePropertyIsChanged);
        }
        private async void PersonellAuthorization()
        {
            while (true)
            {
                var dialres = await this.ShowLoginAsync("Авторизация", "Введите учетные данные для входа",
                    new LoginDialogSettings
                    {
                        ColorScheme = MetroDialogOptions.ColorScheme,
                        EnablePasswordPreview = true,
                        UsernameWatermark = "Имя пользователя",
                        PasswordWatermark = "Пароль",
                        NegativeButtonVisibility = Visibility.Visible,
                        NegativeButtonText = "Close",
                        DialogResultOnCancel = MessageDialogResult.Canceled
                    });
                if (dialres is null)
                {
                    this.Close();
                    return;
                }
                var acc = DBBSContext.GetInstance().PersonnelAccounts
                    .FirstOrDefault(a => a.Login == dialres.Username && a.Password == dialres.Password);
                if (acc is null)
                {
                    var msg = await this.ShowMessageAsync("Ошибка", "Неверные имя пользователя или пароль", MessageDialogStyle.Affirmative);
                }
                else
                {
                    return;
                }
            }
        }
        private void ItemSourcePropertyIsChanged(object sender, EventArgs e)
        {
            if (ListUsers.ItemsSource is ObservableCollection<User>)
            {
                ShowUserMenuItem.Visibility = Visibility.Collapsed;
                ShowAccountMenuItem.Visibility = Visibility.Visible;
                ShowBankAccountMenuItem.Visibility = Visibility.Visible;
                BackMenuItem.Visibility = Visibility.Visible;
                ShowLimitMenuItem.Visibility = Visibility.Collapsed;
            }
            else if (ListUsers.ItemsSource is ObservableCollection<Account>)
            {
                ShowUserMenuItem.Visibility = Visibility.Visible;
                ShowAccountMenuItem.Visibility = Visibility.Collapsed;
                ShowBankAccountMenuItem.Visibility = Visibility.Visible;
                BackMenuItem.Visibility = Visibility.Visible;
                ShowLimitMenuItem.Visibility = Visibility.Visible;
            }
            else if (ListUsers.ItemsSource is ObservableCollection<BankAccount>)
            {
                ShowUserMenuItem.Visibility = Visibility.Visible;
                ShowAccountMenuItem.Visibility = Visibility.Visible;
                ShowBankAccountMenuItem.Visibility = Visibility.Collapsed;
                BackMenuItem.Visibility = Visibility.Visible;
                ShowLimitMenuItem.Visibility = Visibility.Collapsed;
            }
            else if (ListUsers.ItemsSource is ObservableCollection<CreditRequest>)
            {
                ShowUserMenuItem.Visibility = Visibility.Visible;
                ShowAccountMenuItem.Visibility = Visibility.Visible;
                ShowBankAccountMenuItem.Visibility = Visibility.Visible;
                BackMenuItem.Visibility = Visibility.Visible;
                ShowLimitMenuItem.Visibility = Visibility.Collapsed;
            }
            else if (ListUsers.ItemsSource is ObservableCollection<AccountLimit>)
            {
                ShowUserMenuItem.Visibility = Visibility.Collapsed;
                ShowAccountMenuItem.Visibility = Visibility.Collapsed;
                ShowBankAccountMenuItem.Visibility = Visibility.Collapsed;
                BackMenuItem.Visibility = Visibility.Visible;
                ShowLimitMenuItem.Visibility = Visibility.Collapsed;
            }
        }
        void SetDBData()
        {
            BankAccount bankAccount1 = new BankAccount
            {
                Created = DateTime.Now.AddMonths(-2),
                Number = "1234567898765432",
                CreditFunds = 0m,
                PersonalFunds = -3000m,
                IsClosed = false
            };
            BankAccount bankAccount5 = new BankAccount
            {
                Created = DateTime.Now.AddMonths(-1),
                Number = "1234567898765433",
                CreditFunds = 501000m,
                PersonalFunds = 0m,
                IsClosed = false
            };
            Account acc1 = new Account
            {
                Created = DateTime.Now.AddMonths(-2),
                IsBlocked = false,
                Login = "valentpopov",
                Password = "123456789",
                BankAccounts = new[] { bankAccount1, bankAccount5 },
                AccountLimit = new AccountLimit()
            };
            User user1 = new User
            {
                BirthDate = new DateTime(1993, 8, 12),
                FirstName = "Valentin",
                LastName = "Popov",
                Account = acc1
            };
            personell.AddUser(user1, acc1, bankAccount1);
            BankAccount bankAccount2 = new BankAccount
            {
                Created = DateTime.Now.AddMonths(-3),
                Number = "1234567898765434",
                CreditFunds = 5000m,
                PersonalFunds = 30000m,
                IsClosed = false
            };
            BankAccount bankAccount3 = new BankAccount
            {
                Created = DateTime.Now.AddMonths(-1),
                Number = "1234567898765435",
                CreditFunds = 1000m,
                PersonalFunds = 1000m,
                IsClosed = false
            };
            BankAccount bankAccount4 = new BankAccount
            {
                Created = DateTime.Now.AddMonths(-1),
                Number = "1234567898765436",
                CreditFunds = -1000m,
                PersonalFunds = 1000m,
                IsClosed = false
            };
            Account acc2 = new Account
            {
                Created = DateTime.Now.AddMonths(-2),
                IsBlocked = false,
                Login = "uivanov",
                Password = "123456789",
                BankAccounts = new[] { bankAccount2, bankAccount3, bankAccount4 },
                AccountLimit = new AccountLimit()
            };
            User user2 = new User
            {
                BirthDate = new DateTime(1990, 5, 20),
                FirstName = "Yuri",
                LastName = "Ivanov",
                Account = acc2
            };
            personell.AddUser(user2, acc2, bankAccount2);
            DBBSContext.GetInstance().CreditRequests.Add(new CreditRequest
            {
                BankAccount = bankAccount4,
                Created = DateTime.Now,
                CreditSize = 500m
            });
            PersonnelAccount pAcc = new PersonnelAccount
            {
                Created = DateTime.Now,
                Login = "admin",
                Password = "admin"
            };
            DBBSContext.GetInstance().PersonnelAccounts.Add(pAcc);
            DBBSContext.GetInstance().SaveChanges();
        }
        private void SearchUser_OnClick(object sender, RoutedEventArgs e)
        {
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = personell.SearchUsers(SearchQueryBox.Text);
        }
        private void ShowAllUser_OnClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<User> users = personell.GetAllUsers();
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = users;
        }
        private void ShowAllAccounts_OnClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Account> accounts = personell.GetAllAccounts();
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = accounts;
        }
        private void ShowAccountMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListUsers.SelectedItem is User)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchAccount(ListUsers.SelectedItem as User);
            }
            else if (ListUsers.SelectedItem is BankAccount)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchAccount(ListUsers.SelectedItem as BankAccount);
            }
            else if (ListUsers.SelectedItem is CreditRequest)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchAccount(ListUsers.SelectedItem as CreditRequest);
            }
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
                int row = ctx.SaveChanges();
                var msg = await this.ShowMessageAsync("Изменение данных", $"Изменения успешно сохранены {row} row aff",
                    MessageDialogStyle.Affirmative, settings);
            }
        }
        private void ListUsers_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(Account) || e.PropertyName.Contains("Id") || e.PropertyName.Contains("Account") || e.PropertyName.Contains("Password"))
            {
                e.Cancel = true;
                return;
            }
            if (e.PropertyName.Contains("Funds") || e.PropertyName.Contains("Create"))
            {
                e.Column.IsReadOnly = true;
            }

            if (e.PropertyType == typeof(DateTime))
            {
                ((DataGridTextColumn)e.Column).Binding.StringFormat = e.PropertyName.Contains("Birth") ? "dd/MM/yyyy" : "dd/MM/yyyy hh:mm:ss";
            }

        }
        private void ShowBankAccountMenuItem_OnClick(object sender, RoutedEventArgs e)
        {

            if (ListUsers.SelectedItem is User)
            {
                User user = ListUsers.SelectedItem as User;
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchBankAccount(user);
            }
            else if (ListUsers.SelectedItem is Account)
            {
                Account account = ListUsers.SelectedItem as Account;
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchBankAccount(account);
            }
            else if (ListUsers.SelectedItem is CreditRequest)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchBankAccount(ListUsers.SelectedItem as CreditRequest);
            }
        }
        private void SearchAccount_OnClick(object sender, RoutedEventArgs e)
        {
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = personell.SearchAccount(SearchQueryBox.Text);
        }
        private void ShowAllDebtor_OnClick(object sender, RoutedEventArgs e)
        {
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = personell.SearchAllDebtors();
        }
        private void ShowUserMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListUsers.SelectedItem is Account)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchUsers(ListUsers.SelectedItem as Account);
            }
            else if (ListUsers.SelectedItem is BankAccount)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchUsers(ListUsers.SelectedItem as BankAccount);
            }
            else if (ListUsers.SelectedItem is CreditRequest)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.SearchUsers(ListUsers.SelectedItem as CreditRequest);
            }
        }
        private void BackMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_itemSourceBackUp != null)
                ListUsers.ItemsSource = _itemSourceBackUp;
        }
        private void ShowAllVIPClients_OnClick(object sender, RoutedEventArgs e)
        {
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = personell.SearchAllVIPClients();
        }
        private void ShowActualCreditRequests_OnClick(object sender, RoutedEventArgs e)
        {
            _itemSourceBackUp = ListUsers.ItemsSource;
            ListUsers.ItemsSource = personell.ShowCreditRequests();
        }
        private void ShowLimitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListUsers.SelectedItem is Account)
            {
                _itemSourceBackUp = ListUsers.ItemsSource;
                ListUsers.ItemsSource = personell.ShowAccountLimit(ListUsers.SelectedItem as Account);
            }
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
            _dbBankSystemControl.Database.Initialize(true);
        }
        public ObservableCollection<User> SearchUsers(string searchQuery)
        {
            return new ObservableCollection<User>(_dbBankSystemControl.Users.Where(user =>
              user.FirstName.Contains(searchQuery) ||
              user.LastName.Contains(searchQuery) ||
              user.BirthDate.ToString().Contains(searchQuery)
               ).ToList());
        }
        public ObservableCollection<User> SearchUsers(Account account)
        {
            return new ObservableCollection<User>(_dbBankSystemControl.Users.Where(user => user.AccountId == account.Id).ToList());
        }
        public ObservableCollection<User> SearchUsers(BankAccount bankAccount)
        {
            var accounts = _dbBankSystemControl.Accounts.FirstOrDefault(acc => acc.BankAccounts.Any(b => b.Id == bankAccount.Id));
            return new ObservableCollection<User>(_dbBankSystemControl.Users.Where(user => user.AccountId == accounts.Id).ToList());
        }
        public ObservableCollection<User> SearchUsers(CreditRequest creditRequest)
        {
            return SearchUsers(SearchBankAccount(creditRequest).First());
        }
        public ObservableCollection<Account> SearchAccount(User user)
        {
            return new ObservableCollection<Account>(_dbBankSystemControl.Accounts.Where(account =>
               user.AccountId == account.Id).ToList());
        }
        public ObservableCollection<Account> SearchAccount(BankAccount bankAccount)
        {
            return new ObservableCollection<Account>(_dbBankSystemControl.Accounts.Where(acc => acc.BankAccounts.Any(b => b.Id == bankAccount.Id)).ToList());
        }
        public ObservableCollection<Account> SearchAccount(CreditRequest creditRequest)
        {
            return SearchAccount(SearchBankAccount(creditRequest).First());
        }
        public ObservableCollection<Account> SearchAccount(string searchQuery)
        {
            return new ObservableCollection<Account>(_dbBankSystemControl.Accounts.Where(account =>
               account.Login.Contains(searchQuery) ||
               account.Created.ToString().Contains(searchQuery)
                ).ToList());
        }
        public ObservableCollection<BankAccount> SearchBankAccount(User user)
        {
            return SearchBankAccount(_dbBankSystemControl.Accounts.FirstOrDefault(account =>
                user.AccountId == account.Id));
        }
        public ObservableCollection<BankAccount> SearchBankAccount(Account account)
        {
            return new ObservableCollection<BankAccount>(account.BankAccounts.ToList());
        }
        public ObservableCollection<BankAccount> SearchBankAccount(CreditRequest creditRequest)
        {
            return new ObservableCollection<BankAccount>(_dbBankSystemControl.BankAccounts.Where(c => c.Id == creditRequest.BankAccountId).ToList());
        }
        public ObservableCollection<User> GetAllUsers()
        {
            return new ObservableCollection<User>(_dbBankSystemControl.Users.ToList());
        }
        public ObservableCollection<Account> GetAllAccounts()
        {
            return new ObservableCollection<Account>(_dbBankSystemControl.Accounts.ToList());
        }
        public ObservableCollection<BankAccount> SearchAllDebtors()
        {
            return new ObservableCollection<BankAccount>(_dbBankSystemControl.BankAccounts.Where(a => a.CreditFunds < 0 || a.PersonalFunds < 0).ToList());
        }
        public ObservableCollection<BankAccount> SearchAllVIPClients()
        {
            return new ObservableCollection<BankAccount>(_dbBankSystemControl.BankAccounts.Where(a => (a.PersonalFunds + a.CreditFunds) > 500000m).ToList());
        }
        public ObservableCollection<CreditRequest> ShowCreditRequests()
        {
            return new ObservableCollection<CreditRequest>(_dbBankSystemControl.CreditRequests.Where(r => r.IsClosed == false).ToList());
        }
        public ObservableCollection<AccountLimit> ShowAccountLimit(Account account)
        {
            return new ObservableCollection<AccountLimit>(_dbBankSystemControl.AccountLimits.Where(a => a.Id == account.Id).ToList());
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
