using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public class PolicyFactory
    {
        private static readonly Lazy<PolicyFactory> instance = new Lazy<PolicyFactory>(() => new PolicyFactory());

        private Dictionary<string, IPolicy> policies;

        private PolicyFactory()
        {
            LoadPolicies();
        }

        public static PolicyFactory Instance => instance.Value;

        private void LoadPolicies()
        {
            policies = new Dictionary<string, IPolicy>();

            var lines = System.IO.File.ReadAllLines("Policy.properties");
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var values = parts[1].Split(',');
                    if (values.Length == 2 &&
                        double.TryParse(values[0], out double minBalance) &&
                        double.TryParse(values[1], out double rateOfInterest))
                    {
                        policies[key] = new Policy(minBalance, rateOfInterest);
                    }
                }
            }
        }

        public IPolicy CreatePolicy(string accType, string privilege)
        {
            string key = $"{accType.ToUpper()}-{privilege.ToUpper()}";
            if (policies.TryGetValue(key, out IPolicy policy))
            {
                return policy;
            }
            else
            {
                throw new InvalidPolicyTypeException();
            }
        }
    }
}
