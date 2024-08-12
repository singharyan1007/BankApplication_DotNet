
namespace BankApplication.CommonLayer
{
    [Serializable]
    public class InvalidPolicyTypeException : Exception
    {
        public InvalidPolicyTypeException()
        {
        }

        public InvalidPolicyTypeException(string? message) : base(message)
        {
        }

        public InvalidPolicyTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}