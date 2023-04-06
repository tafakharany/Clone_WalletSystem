using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public User? Sender { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
