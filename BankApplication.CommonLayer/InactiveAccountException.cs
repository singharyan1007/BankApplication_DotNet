using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public class InactiveAccountException : ApplicationException
    {
        public InactiveAccountException()
        {
        }

        public InactiveAccountException(string? message) : base(message)
        {
        }

        public InactiveAccountException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
