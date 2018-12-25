using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DBBankSystemControlLibrary
{
    public class DBBSContext : DbContext
    {
        static string _connStr;
        public static DBBSContext Instance { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<AccountLimit> AccountLimits { get; set; }
        public DbSet<CreditRequest> CreditRequests { get; set; }
        private DBBSContext() : base(_connStr) { }
        public static DBBSContext GetInstance(string connStr = null)
        {
            if (Instance is null || !(connStr is null))
            {
                _connStr = connStr;
                Instance = new DBBSContext();
            }
            return Instance;
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public AccountLimit AccountLimit { get; set; }
        [Index(IsUnique = true)]
        public int? AccountLimitId { get; set; }

        public Account()
        {
            AccountLimit = new AccountLimit();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private DateTime _birthDate;

        [Column(TypeName = "date")]
        public DateTime BirthDate
        {
            get { return _birthDate.Date; }
            set { _birthDate = value.Date; }
        }
        public Account Account { get; set; }
        [Index(IsUnique = true)]
        public int? AccountId { get; set; }
        

        
    }

    public class BankAccount
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public decimal CreditFunds { get; set; }
        public decimal PersonalFunds { get; set; }
        public bool IsClosed { get; set; }
        
    }

    public class AccountLimit
    {
        public int Id { get; set; }
        public decimal CashLimit { get; set; } 
        public decimal TranferLimit { get; set; }
        
    }

    public class CreditRequest
    {
        public int Id { get; set; }
        public decimal CreditSize { get; set; }
        public DateTime Created { get; set; }
        [Index(IsUnique = true)]
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
    }
}
