using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public class InvalidAccountTypeException:ApplicationException
    {
        public InvalidAccountTypeException()
        {
            
            
        }

        public InvalidAccountTypeException(string? message) : base(message)
        {
        }

        public InvalidAccountTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
