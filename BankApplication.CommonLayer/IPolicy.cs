using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public interface IPolicy
    {
        double GetMinBalance();
        double GetRateOfInterest();
    }


    public class Policy : IPolicy
    {
        public double MinBalance { get; private set; }
        public double RateOfInterest { get; private set; }

        public Policy(double minBalance, double rateOfInterest)
        {
            MinBalance = minBalance;
            RateOfInterest = rateOfInterest;
        }

        public double GetMinBalance()
        {
            return MinBalance;
        }

        public double GetRateOfInterest()
        {
            return RateOfInterest;
        }
    }
}
