namespace LADCI.AI.Invoicing.Domain.ValueObjects;

public sealed record CustomerId
{
    private CustomerId()
    {
        Value = Guid.Empty;
    }

    public CustomerId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Customer id cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; private set; }
}
