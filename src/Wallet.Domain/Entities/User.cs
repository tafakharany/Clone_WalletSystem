using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Entities;

[Table(name: "User")]
public class User: IdentityUser<int>
{
    public string MobileNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Balance { get; set; }
    public virtual List<Transaction>? SentTransactions { get; set; }
    public virtual List<Transaction>? ReceivedTransactions { get; set; }
}
