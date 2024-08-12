using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.Business
{
    public class Transfer
    {
        public IAccount FromAccount { get; set; }
        public IAccount ToAccount { get; set; }
        public double Amount { get; set; }
        public int Pin { get; set; }


        

        public Transfer(IAccount fromAccount, IAccount toAccount, double amount, int pin)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Pin = pin;
        }
    }
}
