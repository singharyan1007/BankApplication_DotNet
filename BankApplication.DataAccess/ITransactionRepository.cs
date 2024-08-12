using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.DataAccess
{
    public  interface ITransactionRepository
    {
        void Create(Transaction transaction);
        void Update();
        List<Transaction> GetAll();
        Transaction GetTransactionById(int transId);

    }
}
