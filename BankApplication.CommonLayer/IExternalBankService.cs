using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public interface IExternalBankService
    {
        bool Deposit(string accId, double amt);
    }
}
