using LADCI.AI.Invoicing.Application.Invoices.Commands;
using LADCI.AI.Invoicing.Application.Invoices.Queries;
using LADCI.AI.Invoicing.Domain.Aggregates;

namespace LADCI.AI.Invoicing.UnitTests.Application;

internal sealed class InvoiceApplicationFixture
{
    private InvoiceApplicationFixture(InMemoryInvoiceRepository repository)
    {
        Repository = repository;
        CommandService = new InvoiceApplicationService(repository);
        QueryService = new InvoiceQueryService(repository);
    }

    public InMemoryInvoiceRepository Repository { get; }

    public InvoiceApplicationService CommandService { get; }

    public InvoiceQueryService QueryService { get; }

    public static InvoiceApplicationFixture Create()
        => new(new InMemoryInvoiceRepository());

    public InvoiceApplicationFixture WithInvoices(params Invoice[] invoices)
    {
        Repository.Seed(invoices);
        return this;
    }
}
