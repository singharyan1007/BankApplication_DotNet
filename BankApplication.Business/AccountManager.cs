using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using BankApplication.CommonLayer;
using BankApplication.DataAccess;
using Transaction = BankApplication.CommonLayer.Transaction;
using TransactionStatus = BankApplication.CommonLayer.TransactionStatus;

namespace BankApplication.Business
{
    /// <summary>
    /// The AccountManager class provides methods to manage accounts, including creation, withdrawal, deposit, transfer, and external transfer.
    /// </summary>
    public class AccountManager
    {
        /// <summary>
        /// Creates a new account with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the account holder.</param>
        /// <param name="pin">The PIN for the account.</param>
        /// <param name="balance">The initial balance for the account.</param>
        /// <param name="privilegeType">The privilege type of the account (e.g., Regular, Premium).</param>
        /// <param name="accountType">The type of account (e.g., Savings, Current).</param>
        /// <returns>The created IAccount object.</returns>
        /// <exception cref="MinBalanceNeedsToBeMaintainedException">Thrown when the initial balance is below the required minimum balance.</exception>
        public IAccount CreateAccount(string name, int pin, double balance, PrivilegeType privilegeType, AccountType accountType)
        {
            IPolicy policy = PolicyFactory.Instance.CreatePolicy(accountType.ToString(), privilegeType.ToString());
            IAccount account = AccountFactory.CreateAccount(name, pin, balance, privilegeType, accountType);
            account.Policy = policy;

            if (balance < policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            account.Open();
            IAccountRepository accountRepository = new BankDbRepository();
            accountRepository.Create(account);
            return account;
        }

        /// <summary>
        /// Withdraws the specified amount from the given account.
        /// </summary>
        /// <param name="fromAccountNumber">The account number from which to withdraw.</param>
        /// <param name="amount">The amount to withdraw.</param>
        /// <param name="pin">The PIN of the account.</param>
        /// <returns>True if the withdrawal was successful; otherwise, false.</returns>
        /// <exception cref="MinBalanceNeedsToBeMaintainedException">Thrown when the withdrawal would result in a balance below the required minimum.</exception>
        public bool Withdraw(string fromAccountNumber, double amount, int pin)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            ITransactionRepository transactionRepository = new TransactionRepository();

            IAccount fromAccount = accountRepository.GetAccountById(fromAccountNumber);

            if (fromAccount == null)
            {
                Console.WriteLine("Account not found or could not be fetched.");
                return false;
            }

            ValidateAccount(fromAccount, pin);

            if (fromAccount.Policy == null)
            {
                Console.WriteLine("Policy not associated with the account.");
                return false;
            }

            if (fromAccount.Balance - amount < fromAccount.Policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            fromAccount.Balance -= amount;

            Transaction transaction = new Transaction(fromAccount, amount)
            {
                TransactionType = TransactionType.WITHDRAW
            };

            TransactionLog.LogTransactions(fromAccount.AccNo, TransactionType.WITHDRAW, transaction);
            accountRepository.Update(fromAccount.AccNo, fromAccount.Balance);
            transactionRepository.Create(transaction);

            return true;
        }

        /// <summary>
        /// Deposits the specified amount into the given account.
        /// </summary>
        /// <param name="toAccountNumber">The account number into which to deposit.</param>
        /// <param name="amount">The amount to deposit.</param>
        /// <returns>True if the deposit was successful; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown when the account is not found.</exception>
        public bool Deposit(string toAccountNumber, double amount)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            ITransactionRepository transactionRepository = new TransactionRepository();
            IAccount toAccount = accountRepository.GetAccountById(toAccountNumber);

            if (toAccount == null)
            {
                throw new Exception("Account not found");
            }

            ValidateAccount(toAccount, toAccount.Pin);
            toAccount.Balance += amount;

            Transaction transaction = new Transaction(toAccount, amount)
            {
                TransactionType = TransactionType.DEPOSIT
            };

            TransactionLog.LogTransactions(toAccount.AccNo, TransactionType.DEPOSIT, transaction);
            accountRepository.Update(toAccount.AccNo, toAccount.Balance);
            transactionRepository.Create(transaction);

            return true;
        }

        /// <summary>
        /// Transfers funds between two accounts within the same bank.
        /// </summary>
        /// <param name="fromAccountNo">The account number from which to transfer funds.</param>
        /// <param name="toAccountNo">The account number to which to transfer funds.</param>
        /// <param name="amount">The amount to transfer.</param>
        /// <param name="pin">The PIN of the fromAccount.</param>
        /// <returns>True if the transfer was successful; otherwise, false.</returns>
        /// <exception cref="AccountDoesNotExistException">Thrown when one or both accounts do not exist.</exception>
        /// <exception cref="MinBalanceNeedsToBeMaintainedException">Thrown when the transfer would result in a balance below the required minimum.</exception>
        /// <exception cref="DailyLimitExceededException">Thrown when the transfer exceeds the daily limit.</exception>
        public bool TransferFunds(string fromAccountNo, string toAccountNo, double amount, int pin)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            IAccount fromAccount = accountRepository.GetAccountById(fromAccountNo);
            IAccount toAccount = accountRepository.GetAccountById(toAccountNo);
            ITransactionRepository transactionRepository = new TransactionRepository();

            if (fromAccount == null || toAccount == null)
            {
                throw new AccountDoesNotExistException($"The account with ID {fromAccountNo} or {toAccountNo} does not exist.");
            }

            Transfer transfer = new Transfer(fromAccount, toAccount, amount, pin);
            ValidateAccount(transfer.FromAccount, transfer.Pin);

            if (transfer.FromAccount.Balance - transfer.Amount < transfer.FromAccount.Policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            double dailyLimit = AccountPrivilegeManager.GetDailyLimit(transfer.FromAccount.PrivilegeType);
            double totalTransferredToday = GetTotalTransferredToday(transfer.FromAccount.AccNo);

            if (totalTransferredToday + transfer.Amount > dailyLimit)
            {
                throw new DailyLimitExceededException("Daily limit exceeded.");
            }

            transfer.FromAccount.Balance -= transfer.Amount;
            transfer.ToAccount.Balance += transfer.Amount;

            Transaction transaction = new Transaction(transfer.FromAccount, transfer.Amount);
            TransactionLog.LogTransactions(transfer.FromAccount.AccNo, TransactionType.TRANSFER, transaction);
            TransactionLog.LogTransactions(transfer.ToAccount.AccNo, TransactionType.DEPOSIT, transaction);

            bool WithDrawSuccessFull = Withdraw(transfer.FromAccount.AccNo, transfer.Amount, transfer.Pin);
            bool DepositSuccessFull = Deposit(transfer.ToAccount.AccNo, transfer.Amount);

            return WithDrawSuccessFull && DepositSuccessFull;
        }

        /// <summary>
        /// Performs an external transfer to another bank.
        /// </summary>
        /// <param name="transfer">The ExternalTransfer object containing the transfer details.</param>
        /// <returns>True if the external transfer was successful; otherwise, false.</returns>
        /// <exception cref="MinBalanceNeedsToBeMaintainedException">Thrown when the transfer would result in a balance below the required minimum.</exception>
        /// <exception cref="DailyLimitExceededException">Thrown when the transfer exceeds the daily limit.</exception>
        public bool ExternalTransferFunds(ExternalTransfer transfer)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            ValidateAccount(transfer.FromAccount, transfer.FromAccPin);

            if (transfer.FromAccount.Balance - transfer.Amount < transfer.FromAccount.Policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            double dailyLimit = AccountPrivilegeManager.GetDailyLimit(transfer.FromAccount.PrivilegeType);
            double totalTransferredToday = GetTotalTransferredToday(transfer.FromAccount.AccNo);

            if (totalTransferredToday + transfer.Amount > dailyLimit)
            {
                throw new DailyLimitExceededException("Daily limit exceeded.");
            }

           
            accountRepository.BankTableDataUpdate(transfer.ToExternalAccount.AccNo, transfer.ToExternalAccount.BankCode, transfer.Amount);


            bool withdrawSuccess = Withdraw(transfer.FromAccount.AccNo, transfer.Amount, transfer.FromAccPin);

            if (withdrawSuccess == false)
            {
                Console.WriteLine("Withdraw failed in External Transfer of Funds Method");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the account by checking its existence, activity status, and PIN.
        /// </summary>
        /// <param name="account">The account to validate.</param>
        /// <param name="pin">The PIN to validate.</param>
        /// <exception cref="AccountDoesNotExistException">Thrown when the account does not exist.</exception>
        /// <exception cref="AccountNotActiveException">Thrown when the account is not active.</exception>
        /// <exception cref="PinNotMatchedException">Thrown when the PIN does not match.</exception>
        private void ValidateAccount(IAccount account, int pin)
        {
            if (account == null)
            {
                throw new AccountDoesNotExistException("Account does not exist.");
            }

            if (!account.IsActive)
            {
                throw new InactiveAccountException("Account is not active.");
            }

            if (account.Pin != pin)
            {
                throw new InvalidPinException("Pin does not match.");
            }
        }

        /// <summary>
        /// Gets the total amount transferred from the specified account on the current day.
        /// </summary>
        /// <param name="fromAccountNumber">The account number to check.</param>
        /// <returns>The total transferred amount for the day.</returns>
        private double GetTotalTransferredToday(string accNo)
        {
            // Assuming there's a method to get transactions by account number and date
            ITransactionRepository transactionRepository = new TransactionRepository();
            double totalTransferredtoday = transactionRepository.GetTotalTransferredToday(accNo);

            return totalTransferredtoday;
        }
    }
}
