
namespace BankApplication.CommonLayer
{
    [Serializable]
    public class MinBalanceNeedsToBeMaintainedException : Exception
    {
        public MinBalanceNeedsToBeMaintainedException()
        {
        }

        public MinBalanceNeedsToBeMaintainedException(string? message) : base(message)
        {
        }

        public MinBalanceNeedsToBeMaintainedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}