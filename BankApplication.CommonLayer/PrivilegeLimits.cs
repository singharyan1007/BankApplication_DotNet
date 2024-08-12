using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public static class PrivilegeLimits
    {
        public static double GetDailyLimit(PrivilegeType privilegeType)
        {
            switch (privilegeType)
            {
                case PrivilegeType.REGULAR:
                    return 100000.0;
                case PrivilegeType.GOLD:
                    return 200000.0;
                case PrivilegeType.PREMIUM:
                    return 300000.0;
                case PrivilegeType.PREMIUM_ELITE:
                    return 500000.0; // Example additional limit for a new privilege type
                default:
                    throw new InvalidPrivilegeTypeException("Invalid privilege type.");
            }
        }

    }
}
