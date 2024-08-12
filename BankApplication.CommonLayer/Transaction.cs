using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace BankApplication.CommonLayer
{
    public class Transaction
    {
        public int TransID { get; set; }
        public IAccount FromAccount { get; set; }
        public DateTime TranDate { get; set; }
        public double Amount { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }

        public Transaction(IAccount fromAccount, double amount)
        {
            this.TransID = IdGenerator.GenerateId();
            this.FromAccount = fromAccount;
            this.TranDate = DateTime.Now;
            this.Amount = amount;
            this.TransactionStatus = TransactionStatus.CLOSE;
        }
    }
}
