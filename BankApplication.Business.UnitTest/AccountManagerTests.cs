using System.Security.Cryptography.X509Certificates;
using BankApplication.CommonLayer;
namespace BankApplication.Business.UnitTest
{
    [TestClass]
    public class AccountManagerTests
    {
        public AccountManager _accountManager=null;


        [TestInitialize]
        public void Setup()
        {
           
            _accountManager = new AccountManager();

        }

        [TestCleanup]
        public void Cleanup() 
        {
            _accountManager = null;
        }

        [TestMethod]
        public void CreateAccount_WithValidInput_ShouldCreateSavingsAccount()
        {
            //Arrange
            string name = "Aryan Singh";
        int pin = 1234;
        double balance = 100000.0;
        PrivilegeType privilege = PrivilegeType.PREMIUM;
        AccountType accountType = AccountType.SAVINGS;



            //Act
            IAccount account = _accountManager.CreateAccount(name, pin, balance, privilege, accountType);

            //Assert
            Assert.IsNotNull(account);
            Assert.AreEqual(name, account.Name);
            Assert.AreEqual(pin, account.Pin);
            Assert.AreEqual(balance, account.Balance);
            Assert.AreEqual(privilege, account.PrivilegeType);
            Assert.AreEqual("SAVINGS", account.GetAccType());

        }

        [TestMethod]
        [ExpectedException(typeof(MinBalanceNeedsToBeMaintainedException))]
        public void CreateAccount_ShouldThrowMinBalanceNeedsToBeMaintainedException() 
        {
            //Arrange
            string name = "Aryan Singh";
            int pin = 1234;
            double balance = 1000.0; //Less than the min balance that needs to be maintained for Regular Savings Account
            PrivilegeType privilege = PrivilegeType.PREMIUM;
            AccountType accountType = AccountType.SAVINGS;

            //Act
            _accountManager.CreateAccount(name,pin,balance,privilege,accountType);
        }
    }
}