using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.DataAccess

{
    public interface IAccountRepository
    {
        void Create(IAccount account);
        void Update(string accountNumber, double amount);

       
        List<IAccount> GetAll();

        IAccount GetAccountById(string accountNumber);

        long GetAccountCount();
        double GetTotalBankWorth();

        Dictionary<string, long> GetAccountCountByType();



    }
}
