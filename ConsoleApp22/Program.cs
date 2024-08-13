using NLog;
using BankApplication.Business;
using BankApplication.CommonLayer;
using Microsoft.Extensions.Logging;
namespace ConsoleApp22
{
    // BANK PRATIAN APP PROJECT
    public class Program
    {
        //Initialising the Logger
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            Logger.Info("Application started.");
            

            AccountManager accountManager = new AccountManager();  //Creating an Account Manager
            Console.WriteLine("----------------------------------------------------------------------------------");
            Console.WriteLine("                          WELCOME TO BANK OF PRATIAN!!");
            Console.WriteLine("----------------------------------------------------------------------------------");

            while (true) 
            {
                Console.WriteLine("\n                            Bank Application Menu:");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Transfer Funds");
                Console.WriteLine("5. External Transfer");
                Console.WriteLine("6. Exit");
                Console.WriteLine("----------------------------------------------------------------------------------");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Select Account Type (0: SAVINGS, 1: CURRENT): ");
                        AccountType accType = (AccountType)int.Parse(Console.ReadLine());

                        Console.Write("Enter Name: ");
                        string name = Console.ReadLine();

                        Console.Write("Set Your PIN: ");
                        int pin = int.Parse(Console.ReadLine());

                        Console.WriteLine("Select Privilege Type (0: REGULAR, 1: GOLD, 2: PREMIUM): ");
                        PrivilegeType privilegeType = (PrivilegeType)int.Parse(Console.ReadLine());

                        Console.Write("Enter Initial Balance: ");
                        double balance = double.Parse(Console.ReadLine());
                        try
                        {
                            IAccount newAccount = accountManager.CreateAccount(name, pin, balance, privilegeType, accType);
                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Console.WriteLine($"Account created successfully! Account Number: {newAccount.AccNo}");
                            Console.WriteLine("----------------------------------------------------------------------------------");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Logger.Error(ex, "Error during Creating Account:");
                        }
                        break;



                    case "2":
                        //Deposit method
                        Console.Write("Enter Account Number: ");
                        string accNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter Amount to Deposit: ");
                        double depositAmount = double.Parse(Console.ReadLine());

                        try
                        {
                            bool isDepositSuccess = accountManager.Deposit(accNo, depositAmount);
                            if (isDepositSuccess)
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Deposit successful!");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                            else
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Error in depositing.");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("----------------------------------------------------------------------------------");
                            Logger.Error(ex, "Error during Deposit");
                        }
                        break;

                    case "3":
                        Console.Write("Enter Account Number: ");
                        string withdrawAccNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter PIN: ");
                        int withdrawPin =int.Parse( Console.ReadLine());
                        Console.Write("Enter Amount to Withdraw: ");
                        double withdrawAmount = double.Parse(Console.ReadLine());
                        try
                        {
                            bool withdrawSuccessful= accountManager.Withdraw(withdrawAccNo, withdrawAmount, withdrawPin);
                            if (withdrawSuccessful)
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Withdrawal successful!");

                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                            else
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Withdrawl failed");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                               
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex} : Error during the withdrawl process");
                            Logger.Error(ex, "Error during the withdrawl process !!");

                        }
                        break ;

                    case "4":
                        //Transfer from one account to another account
                        Console.Write("Enter From Account Number: ");
                        string fromAccNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter To Account Number: ");
                        string toAccNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter PIN: ");
                        int transferPin =int.Parse( Console.ReadLine());
                        Console.Write("Enter Amount to Transfer: ");
                        double transferAmount = double.Parse(Console.ReadLine());

                        try
                        {
                            bool transferCompleted = accountManager.TransferFunds(fromAccNo, toAccNo, transferAmount, transferPin);
                            if (transferCompleted)
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Transfer successful!");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                            else 
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("Transfer failed");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                            Logger.Error(ex, "Error during the transfer funds process !!");

                        }
                        break;
                    // Case for External Transfer of Funds
                    case "5":
                        // External Transfer to another bank
                        Console.Write("Enter From Account Number: ");
                        string extFromAccNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter Bank Code of External Bank: ");
                        string bankCode = Console.ReadLine().ToUpper();
                        Console.WriteLine("Enter the Bank Name of the External Bank");
                        string bankName = Console.ReadLine().ToUpper();
                        Console.Write("Enter To External Account Number: ");
                        string extToAccNo = Console.ReadLine().ToUpper();
                        Console.Write("Enter PIN: ");
                        int extTransferPin = int.Parse(Console.ReadLine());
                        Console.Write("Enter Amount to Transfer: ");
                        double extTransferAmount = double.Parse(Console.ReadLine());

                        try
                        {
                            // Creating ExternalTransfer object
                            ExternalTransfer externalTransfer = new ExternalTransfer
                            {
                                FromAccount = accountManager.getAccount(extFromAccNo),
                                FromAccPin = extTransferPin,
                                ToExternalAccount = new ExternalAccount
                                {
                                    AccNo = extToAccNo,
                                    BankCode = bankCode,
                                    BankName = bankName,
                                },
                                Amount = extTransferAmount
                            };

                            bool externalTransferCompleted = accountManager.ExternalTransferFunds(externalTransfer);
                            if (externalTransferCompleted)
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("External Transfer successful!");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                            else
                            {
                                Console.WriteLine("----------------------------------------------------------------------------------");
                                Console.WriteLine("External Transfer failed");
                                Console.WriteLine("----------------------------------------------------------------------------------");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Logger.Error(ex, "Error during the external transfer funds process !!");
                        }
                        break;

                    case "6":
                        Console.WriteLine("----------------------------------------------------------------------------------");
                        Console.WriteLine("Exiting application.");
                        Console.WriteLine("----------------------------------------------------------------------------------");
                        return;

                    default:
                        Console.WriteLine("----------------------------------------------------------------------------------");
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine("----------------------------------------------------------------------------------");
                        break;

                }
            }

           

        }
    }
}
