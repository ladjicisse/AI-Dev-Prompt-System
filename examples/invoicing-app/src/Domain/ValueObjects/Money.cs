namespace LADCI.AI.Invoicing.Domain.ValueObjects;

public sealed record Money
{
    private Money()
    {
        Amount = default;
        Currency = string.Empty;
    }

    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; }
}
