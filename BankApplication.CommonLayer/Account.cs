using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public interface IAccount
    {
       public string AccNo { get; set; }
       public string Name { get; set; }
        int Pin { get; set; }
        bool IsActive { get; set; }
        DateTime DateOfOpening { get; set; }
        double Balance { get; set; }
        PrivilegeType PrivilegeType { get; set; }
        public IPolicy Policy { get; set; }

        string GetAccType();
        bool Open();
        bool Close();
    }


    public abstract  class Account : IAccount
    {
        public string AccNo { get;  set; }
        public string Name { get; set; }
        public int Pin { get; set; }
        public bool IsActive { get;  set; }
        public DateTime DateOfOpening { get;  set; }
        public double Balance { get; set; }
        public PrivilegeType PrivilegeType { get; set; }
        public IPolicy Policy{ get; set; }

        public abstract string GetAccType();
        public abstract bool Open();
        public abstract bool Close();
    }
}
