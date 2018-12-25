using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

namespace BankSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ClientControl _clientControl;
        public MainWindow()
        {
            _clientControl = ClientControl.InitializeDBControl();
            InitializeComponent();
            ClientAuthorization();
        }
        private async void ClientAuthorization()
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
                var acc = DBBSContext.GetInstance().Accounts
                    .FirstOrDefault(a => a.Login == dialres.Username && a.Password == dialres.Password);
                if (acc is null)
                {
                    var msg = await this.ShowMessageAsync("Ошибка", "Неверные имя пользователя или пароль", MessageDialogStyle.Affirmative);
                }
                else
                {
                    _clientControl.CurrentAccount= acc;
                    return;
                }
            }
        }

        private void ShowBankAccountsButton_OnClick(object sender, RoutedEventArgs e)
        {
            BankAccountListPanel.Visibility = Visibility.Visible;
            BankAccountList.ItemsSource = _clientControl.GetBankAccounts();
            TransferPanel.Visibility = Visibility.Collapsed;
        }

        private void TransferButton_OnClick(object sender, RoutedEventArgs e)
        {
            TransferPanel.Visibility = Visibility.Visible;
            var bAccCollection = _clientControl.GetBankAccounts();
            TransferFromComboBox.ItemsSource = bAccCollection;
            TransferToComboBox.ItemsSource = bAccCollection;
            BankAccountListPanel.Visibility = Visibility.Collapsed;
        }

        private async void CreditRequestButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(CreditAmountTxtBox.Text, out decimal amount))
            {
                var msg = await this.ShowMessageAsync("Ошибка", "Введите сумму погашения кредита");
                return;
            }
            _clientControl.SetCreditRequest(BankAccountList.SelectedItem as BankAccount,amount);
        }

        private async void PayToCredit_OnClick(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(CreditAmountTxtBox.Text, out decimal amount))
            {
                var msg = await this.ShowMessageAsync("Ошибка", "Введите сумму погашения кредита");
                return;
            }
            _clientControl.PayToCredit(BankAccountList.SelectedItem as BankAccount, amount);
        }

        private void ShowCreditRequests_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CashToCard_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void BlockBankAccount_OnClick(object sender, RoutedEventArgs e)
        {
            BankAccount bankAccount = BankAccountList.SelectedItem as BankAccount;
            if (bankAccount.CreditFunds == 0)
            {
                bankAccount.IsClosed = true;
                DBBSContext.GetInstance().SaveChanges();
            }
            else
            {
                var msg = await this.ShowMessageAsync("Ошибка", "Кредит полностью не погашен");
            }
        }

        private void BankAccountList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BankAccountBorder.Visibility = Visibility.Visible;
            TransactionHistoryDataGrid.ItemsSource =
                _clientControl.GetTransactionHistory(BankAccountList.SelectedItem as BankAccount);
        }
        private async void BeginTransferButton_OnClick(object sender, RoutedEventArgs e)
        {
            BankAccount transferFrom = TransferFromComboBox.SelectedItem as BankAccount;
            BankAccount transferTo = TransferToComboBox.SelectedItem as BankAccount;
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount)||transferFrom==transferTo)
            {
                var res = await this.ShowMessageAsync("Ошибка", "Введены некорректные данные");
                return;
            }
            if (transferTo is null)
            {
                transferTo=_clientControl.SearchBankAccount(TransferToComboBox.Text);
            }
            _clientControl.BeginTransfer(transferFrom, transferTo, amount);
        }
    }

    public class ClientControl
    {
        public Account CurrentAccount;
        private string _connStr;
        private DBBSContext _dbBankSystemControl;
        public static ClientControl Instance;
        private ClientControl() {
            _connStr = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
            _dbBankSystemControl = DBBSContext.GetInstance(_connStr);
            _dbBankSystemControl.Database.Initialize(true);
        }
        public static ClientControl InitializeDBControl()
        {
            return Instance ?? (Instance = new ClientControl());
        }

        public ObservableCollection<BankAccount> GetBankAccounts()
        {
            return new ObservableCollection<BankAccount>(CurrentAccount.BankAccounts);
        }
        public ObservableCollection<TransactionHistory> GetTransactionHistory(BankAccount bankAccount)
        {
            return new ObservableCollection<TransactionHistory>(_dbBankSystemControl.TransactionHistories.Where(t=>t.FromBankAccount==bankAccount.Number||t.ToBankAccount==bankAccount.Number).ToList());
        }

        public async void BeginTransfer(BankAccount From, BankAccount To, decimal amount)
        {
            using (var transaction = _dbBankSystemControl.Database.BeginTransaction())
            {
                try
                {
                    if (From.PersonalFunds < amount) throw new Exception("Недостаточно средств");
                    From.PersonalFunds = From.PersonalFunds - amount;
                    To.PersonalFunds = To.PersonalFunds + amount;
                    _dbBankSystemControl.TransactionHistories.Add(new TransactionHistory
                    {
                        Created = DateTime.Now,
                        FromBankAccount = From.Number,
                        ToBankAccount = To.Number,
                        RemittanceAmount = amount
                    });
                    _dbBankSystemControl.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    var res = await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync("Ошибка", e.Message);
                    transaction.Rollback();
                    return;
                }
                var result = await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync("Успех", "Транзакция прошла успешно");
            }
        }

        public async void PayToCredit(BankAccount bankAccount, decimal amount)
        {
            using (var transaction = _dbBankSystemControl.Database.BeginTransaction())
            {
                try
                {
                    if (bankAccount.PersonalFunds < amount) throw new Exception("Недостаточно средств");
                    bankAccount.PersonalFunds = bankAccount.PersonalFunds - amount;
                    bankAccount.CreditFunds = bankAccount.CreditFunds - amount;
                    _dbBankSystemControl.TransactionHistories.Add(new TransactionHistory
                    {
                        Created = DateTime.Now,
                        FromBankAccount = bankAccount.Number,
                        ToBankAccount = bankAccount.Number,
                        RemittanceAmount = amount
                    });
                    _dbBankSystemControl.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    var res = await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync("Ошибка", e.Message);
                    transaction.Rollback();
                    return;
                }
                var result = await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync("Успех", "Транзакция прошла успешно");
            }
        }

        public BankAccount SearchBankAccount(string number)
        {
            return _dbBankSystemControl.BankAccounts.FirstOrDefault(a => a.Number == number);
        }

        public void SetCreditRequest(BankAccount bankAccount, decimal creditSize)
        {
            _dbBankSystemControl.CreditRequests.Add(new CreditRequest
            {
                BankAccount = bankAccount,
                Created = DateTime.Now,
                CreditSize = creditSize
            });
            _dbBankSystemControl.SaveChanges();
        }
    }
}
