using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.CommonLayer
{
    public static class AccountPrivilegeManager
    {
        public static double GetDailyLimit(PrivilegeType privilegeType)
        {
            return PrivilegeLimits.GetDailyLimit(privilegeType);
        }
    }
}
