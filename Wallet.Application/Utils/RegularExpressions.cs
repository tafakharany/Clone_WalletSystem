using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Utils
{
    public class RegularExpressions
    {
        public const string Email = @"^*([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string MobileNumber = @"^(00201|201|\\+201|01)(0|1|2|5)([0-9]{8})$";
        public const string Password = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$";
    }
}
