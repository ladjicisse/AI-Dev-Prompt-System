namespace LADCI.AI.Invoicing.Domain.ValueObjects;

public sealed record InvoiceId
{
    private InvoiceId()
    {
        Value = Guid.Empty;
    }

    public InvoiceId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Invoice id cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; private set; }
}
