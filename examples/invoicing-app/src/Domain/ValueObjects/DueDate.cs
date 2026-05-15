namespace LADCI.AI.Invoicing.Domain.ValueObjects;

public sealed record DueDate
{
    private DueDate()
    {
        Value = default;
    }

    public DueDate(DateOnly value)
    {
        Value = value;
    }

    public DateOnly Value { get; private set; }
}
