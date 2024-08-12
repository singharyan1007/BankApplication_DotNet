using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace ConsoleApp22
{
    public class CitiBankService : IExternalBankService
    {
        public bool Deposit(string accId, double amt)
        {
            try
            {
                Console.WriteLine($"Depositing {amt} to CitiBank account number {accId} ");
                return true;
            }
            catch(Exception ex) { Console.WriteLine($"error in depositing to bank account {accId}",ex); return false; }
           

        }
        

    }
}
