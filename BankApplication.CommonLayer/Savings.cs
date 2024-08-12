using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BankApplication.CommonLayer;
namespace BankApplication.CommonLayer
{
    public class Savings:Account
    {
        public Savings() { }
        public Savings(string Name, int Pin, double Balance, PrivilegeType PrivilegeType)
        {
            this.Name = Name;
            this.AccNo = "SAV"+IdGenerator.GenerateId();
            this.Pin = Pin;
            this.IsActive = false;
            this.DateOfOpening = DateTime.Now;
            this.Balance = Balance;
            this.PrivilegeType = PrivilegeType;

           
        }

        

        public override string GetAccType()
        {
            return "SAVINGS";
        }
        public override bool Open()
        {
            IsActive = true;
            Console.WriteLine($"Account {AccNo} activated.");
            return true;
        }
        public override bool Close() 
        {
            if (this.Balance != 0)
            {
                this.Balance = 0;
            }
            IsActive = false;
            return true;
            
        }

    }
}
