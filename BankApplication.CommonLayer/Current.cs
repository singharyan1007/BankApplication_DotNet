using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BankApplication.CommonLayer;
namespace BankApplication.CommonLayer
{
    public class Current : Account
    {
        public Current() { }
        public Current(string name, int pin, double balance, PrivilegeType privilegeType)
        {
            Name = name;
            AccNo = "CUR"+IdGenerator.GenerateId();
            Pin = pin;
            IsActive = false;
            DateOfOpening = DateTime.Now;
            Balance = balance;
            PrivilegeType = privilegeType;


           
        }

       

         public override string GetAccType()
        {
            return "CURRENT";
             
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

