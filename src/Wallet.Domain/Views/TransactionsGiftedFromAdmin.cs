using System;
using System.Collections.Generic;

namespace Wallet.Domain.Views;

public partial class TransactionsGiftedFromAdmin
{
    public int? Id { get; set; }

    public int? RecipientId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    public string? Name { get; set; }
}
