using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
namespace BankApplication.Business
{
    public class ExternalBankServiceFactory
    {
        private static readonly ExternalBankServiceFactory instance = new ExternalBankServiceFactory();
        private readonly Dictionary<string, IExternalBankService> serviceBank;

        private ExternalBankServiceFactory()
        {
            serviceBank = new Dictionary<string, IExternalBankService>();
            LoadServices();
        }

        public static ExternalBankServiceFactory Instance => instance;

        private void LoadServices()
        {
            var properties = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines("serviceBanks.properties");

            foreach (string line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    properties[parts[0]] = parts[1];
                }
            }
        }

        public IExternalBankService GetService(string bankCode)
        {
            if (serviceBank.TryGetValue(bankCode, out IExternalBankService service))
            {
                return service;
            }
            throw new ArgumentException("Invalid bank code.");
        }

        private IDictionary<string, IExternalBankService> LoadBankServicesFromConfig()
        {
            var serviceBankPool = new Dictionary<string, IExternalBankService>();

            // Replace this with actual configuration file reading logic (e.g., using System.Configuration)
            var appSettings = ConfigurationManager.AppSettings;
            foreach (var key in appSettings.Keys)
            {
                string bankCode = key.ToString();
                string className = appSettings[bankCode];

                
               
                    IExternalBankService bankObj = (IExternalBankService)Activator.CreateInstance(Type.GetType(className));
                serviceBankPool.Add(bankCode, bankObj);
                
                
            }

            return serviceBankPool;
        }
    }
}

