
namespace BankApplication.CommonLayer
{
    [Serializable]
    internal class InvalidPrivilegeTypeException : Exception
    {
        public InvalidPrivilegeTypeException()
        {
        }

        public InvalidPrivilegeTypeException(string? message) : base(message)
        {
        }

        public InvalidPrivilegeTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}