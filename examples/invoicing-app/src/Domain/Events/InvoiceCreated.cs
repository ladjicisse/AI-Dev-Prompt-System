using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.Domain.Events;

public sealed record InvoiceCreated(InvoiceId InvoiceId);
