using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.Business
{
    public static class AccountFactory
    {
        
        public static IAccount CreateAccount(string name, int pin, double balance, PrivilegeType privilegeType, AccountType accType)
        {
            switch (accType)
            {
                case AccountType.SAVINGS:
                    return new Savings(name, pin, balance, privilegeType);
                case AccountType.CURRENT:
                    return new Current(name, pin, balance, privilegeType);
                default:
                    throw new InvalidAccountTypeException("Invalid account type");
            }
            

            
        }
    }
}
