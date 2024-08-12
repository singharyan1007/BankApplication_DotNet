using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public class AccountDoesNotExistException : ApplicationException
    {
        public AccountDoesNotExistException()
        {
        }

        public AccountDoesNotExistException(string? message) : base(message)
        {
        }

        public AccountDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
