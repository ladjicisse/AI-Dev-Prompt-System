namespace LADCI.AI.Invoicing.Domain.ValueObjects;

public sealed record LineItem
{
    private LineItem()
    {
        Description = string.Empty;
        Subtotal = null!;
    }

    public LineItem(string description, int quantity, Money subtotal)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        Subtotal = subtotal ?? throw new ArgumentNullException(nameof(subtotal));
        Description = description;
        Quantity = quantity;
    }

    public string Description { get; private set; }

    public int Quantity { get; private set; }

    public Money Subtotal { get; private set; }
}
