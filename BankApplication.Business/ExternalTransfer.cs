using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BankApplication.CommonLayer;
using Transaction = BankApplication.CommonLayer.Transaction;
using TransactionStatus = BankApplication.CommonLayer.TransactionStatus;
namespace BankApplication.Business

{
    public class ExternalTransfer : Transaction
    {
        public ExternalAccount ToExternalAccount { get; set; }
        public int FromAccPin { get; set; }
        public TransactionStatus Status { get; set; }
        public ExternalTransfer(IAccount fromAccount, ExternalAccount toExternalAccount, double amount, int fromAccPin) : base(fromAccount, amount)
        {
            
            this.ToExternalAccount = toExternalAccount;
            this.FromAccPin = fromAccPin;
            this.Status = TransactionStatus.OPEN;
        }
    }
}
