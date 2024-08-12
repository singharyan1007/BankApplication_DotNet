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
        private static readonly ExternalBankServiceFactory _instance = new ExternalBankServiceFactory();
        private readonly IDictionary<string, IExternalBankService> _serviceBankPool;

        private ExternalBankServiceFactory()
        {
            _serviceBankPool = LoadBankServicesFromConfig(); // Load services first
        }

        public static ExternalBankServiceFactory Instance => _instance;

        public IExternalBankService GetExternalBankService(string bankCode)
        {
            if (_serviceBankPool.ContainsKey(bankCode))
            {
                return _serviceBankPool[bankCode];
            }
            else
            {
                throw new ArgumentException($"Unsupported bank code: {bankCode}");
            }
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

