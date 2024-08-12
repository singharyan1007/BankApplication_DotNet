namespace BankApplication.Business
{
    public class ExternalAccount
    {
        public string AccNo { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }

        public ExternalAccount(string AccNo, string BankCode, string BankName)
        {
            this.AccNo = AccNo;
            this.BankCode = BankCode;
            this.BankName = BankName;
        }
    }
}