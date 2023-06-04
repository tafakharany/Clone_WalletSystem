namespace Wallet.Domain.Views;

public partial class TransactionsSentFromUsersToEachOther
{
    public int? Id { get; set; }

    public int? RecipientId { get; set; }

    public int? SenderId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    public string? Recipientname { get; set; }

    public string? Sendername { get; set; }

    public decimal? Amountsenttorecipient { get; set; }

    public long? Rownumber { get; set; }

    public decimal? Totalamountsenttorecipient { get; set; }
}
