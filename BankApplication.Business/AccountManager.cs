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
namespace BankApplication.Business
{
    public class AccountManager
    {
       

        public IAccount CreateAccount(string name, int pin, double balance, PrivilegeType privilegeType, AccountType accountType)
        {
            // Ensure AccountFactory.CreateAccount returns IAccount
            IPolicy policy = PolicyFactory.Instance.CreatePolicy(accountType.ToString(), privilegeType.ToString());
            IAccount account = AccountFactory.CreateAccount(name, pin, balance, privilegeType, accountType);
            
            
           
            account.Policy = policy;
            if (account.Policy != null)
            {
                Console.WriteLine($"{account.Policy} Policy is set");
            }
           



            if (balance < policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            account.Open();
            IAccountRepository accountRepository = new BankDbRepository();
            accountRepository.Create(account);
            return account;
        }

        public bool Withdraw(string fromAccountNumber, double amount, int pin)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            ITransactionRepository transactionRepository = new TransactionRepository();

            IAccount fromAccount = accountRepository.GetAccountById(fromAccountNumber);

            // Check if account is null
            if (fromAccount == null)
            {
                Console.WriteLine("Account not found or could not be fetched.");
                return false; // Alternatively, throw an AccountDoesNotExistException
            }

            // Validate account PIN
            ValidateAccount(fromAccount, pin);

            if (fromAccount.Policy == null)
            {
                Console.WriteLine("Policy not associated with the account.");
                return false;
            }

            // Check minimum balance requirement
            if (fromAccount.Balance - amount < fromAccount.Policy.GetMinBalance())
            {
                throw new MinBalanceNeedsToBeMaintainedException("Minimum balance needs to be maintained.");
            }

            // Update the account balance
            fromAccount.Balance -= amount;

            // Create a transaction record
            Transaction transaction = new Transaction(fromAccount, amount)
            {
                TransactionType = TransactionType.WITHDRAW // Set the transaction type
            };

            // Log the transaction
            TransactionLog.LogTransactions(fromAccount.AccNo, TransactionType.WITHDRAW, transaction);

            // Update the account balance in the database
            accountRepository.Update(fromAccount.AccNo, fromAccount.Balance);

            // Add the transaction record to the database
            transactionRepository.Create(transaction);

            return true;
        }

        public bool Deposit(string toAccountNumber, double amount)
        {
            //first take the input as the account number and then call the GetAccountById from the DAL and then pass is to the Transaction instance and then Update the Accounts table
            IAccountRepository accountRepository = new BankDbRepository();
            IAccount toAccount=accountRepository.GetAccountById(toAccountNumber);

            if (toAccount == null)
            {
                throw new Exception("Account not found");

            }


            ValidateAccount(toAccount, toAccount.Pin);  // Assuming IAccount has a Pin property
            toAccount.Balance += amount;
            Transaction transaction = new Transaction(toAccount, amount)
            {
                TransactionType = TransactionType.DEPOSIT
            };
            TransactionLog.LogTransactions(toAccount.AccNo, TransactionType.DEPOSIT, transaction);
           
            accountRepository.Update(toAccount.AccNo, toAccount.Balance);

            return true;
        }

        public bool TransferFunds(string fromAccountNo, string toAccountNo, double amount, int pin)
        {
            IAccountRepository accountRepository = new BankDbRepository();
            IAccount fromAccount = accountRepository.GetAccountById(fromAccountNo);
            IAccount toAccount = accountRepository.GetAccountById(toAccountNo);
            if (fromAccount == null)
            {
                throw new AccountDoesNotExistException($"The account with ID {fromAccountNo} does not exist.");
            }
            if (toAccount == null)
            {
                throw new AccountDoesNotExistException($"The account with ID {toAccountNo} does not exist.");
            }
            Transfer transfer = new Transfer(fromAccount, toAccount, amount, pin);

            //First take the the two accounts as input along with the money to be transferred. Then get the two accounts from the DAL layer. If not present return false and throw an AccountDoesNotExistException. If found, do the transfer after checking the minimum balance Policy and update both the accounts.
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

            //Make the update in the account for both the toAccount and the forAccount
            
            accountRepository.Update(transfer.ToAccount.AccNo, transfer.ToAccount.Balance);
            accountRepository.Update(transfer.FromAccount.AccNo, transfer.FromAccount.Balance);

            //Write the code to update the transaction table

            return true;
        }

        public bool ExternalTransferFunds(ExternalTransfer transfer)
        {
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

            transfer.FromAccount.Balance -= transfer.Amount;

            // Here you should add the actual code to transfer the amount to the external bank
            // Simulate external transfer completion

            //When the manager makes a transaction to an external account of a bank like CITI, the logic to transfer the code should be written her
            ExternalBankServiceFactory factory = ExternalBankServiceFactory.Instance;
            IExternalBankService citiBankService = factory.GetExternalBankService("CITI");
            bool depositSuccess = citiBankService.Deposit("170401", transfer.Amount);



            transfer.Status = TransactionStatus.CLOSE; // Set the status to CLOSE once the external transfer is complete

            TransactionLog.LogTransactions(transfer.FromAccount.AccNo, TransactionType.EXTERNALTRANSFER, transfer);

            return true;
        }

        private void ValidateAccount(IAccount account, int pin)
        {
            if (account == null)
            {
                throw new AccountDoesNotExistException("Account does not exist");
            }
            if (!account.IsActive)
            {
                throw new InactiveAccountException("Account is inactive");
            }
            if (account.Pin != pin)
            {
                throw new InvalidPinException("Enter a correct pin");
            }
        }

        private double GetTotalTransferredToday(string accNo)
        {
            var transactions = TransactionLog.GetTransactions(accNo, TransactionType.TRANSFER);
            double total = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.TranDate.Date == DateTime.Now.Date)
                {
                    total += transaction.Amount;
                }
            }

            return total;
        }
    }



}
