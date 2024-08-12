using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.Business
{
    public  class TransactionLog
    {
        private static Dictionary<string, Dictionary<TransactionType, List<Transaction>>> transactionsLog = new Dictionary<string, Dictionary<TransactionType, List<Transaction>>>();

        //Get all transactions
        public static Dictionary<string, Dictionary<TransactionType, List<Transaction>>> GetTransactions()
        {
            return transactionsLog;
        }
        //Get all the transactions for a particular Account
        public static  Dictionary<TransactionType, List<Transaction>> GetTransactions(string accNo)
        {
            if (transactionsLog.ContainsKey(accNo))
            {
                return transactionsLog[accNo];
            }
            else
            {
                throw new TransactionNotFoundException("Transactions not found for the account.");
            }
        }
        //Get all the transactions for a particular transaction type for a particular Account
        public static List<Transaction> GetTransactions(string accNo, TransactionType type)
        {
            if (!transactionsLog.ContainsKey(accNo))
            {
                transactionsLog[accNo] = new Dictionary<TransactionType, List<Transaction>>();
            }

            if (!transactionsLog[accNo].ContainsKey(type))
            {
                transactionsLog[accNo][type] = new List<Transaction>(); // Create an empty list for the new type
            }

            return transactionsLog[accNo][type];
        }


        //Log all the transactions
        public static  void LogTransactions(string accNo, TransactionType type, Transaction transaction)
        {
            if (!transactionsLog.ContainsKey(accNo))
            {
                transactionsLog[accNo] = new Dictionary<TransactionType, List<Transaction>>();
            }

            if (!transactionsLog[accNo].ContainsKey(type))
            {
                transactionsLog[accNo][type] = new List<Transaction>();
            }

            transactionsLog[accNo][type].Add(transaction);
        }

      





    }
}
