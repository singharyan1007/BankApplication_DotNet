using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public class InvalidPinException : ApplicationException
    {
        public InvalidPinException()
        {
        }

        public InvalidPinException(string? message) : base(message)
        {
        }

        public InvalidPinException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
