
namespace BankApplication.CommonLayer
{
    [Serializable]
    internal class InvalidTransactionTypeException : Exception
    {
        public InvalidTransactionTypeException()
        {
        }

        public InvalidTransactionTypeException(string? message) : base(message)
        {
        }

        public InvalidTransactionTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}