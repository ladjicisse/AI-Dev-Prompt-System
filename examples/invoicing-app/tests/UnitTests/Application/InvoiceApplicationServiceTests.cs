using FluentAssertions;
using LADCI.AI.Invoicing.Application.Invoices.Commands;

namespace LADCI.AI.Invoicing.UnitTests.Application;

public sealed class InvoiceApplicationServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_And_Persist_Invoice()
    {
        var fixture = InvoiceApplicationFixture.Create();
        var request = TestData.CreateInvoiceRequest();

        var result = await fixture.CommandService.CreateAsync(request);

        result.InvoiceId.Should().Be(request.InvoiceId);
        result.CustomerId.Should().Be(request.CustomerId);
        result.Status.Should().Be("Draft");
        result.TotalAmount.Should().Be(request.TotalAmount);
        result.Currency.Should().Be(request.Currency);
        result.LineItems.Should().HaveCount(2);

        fixture.Repository.StoredInvoices.Should().ContainSingle(x => x.Id.Value == request.InvoiceId);
    }

    [Fact]
    public async Task CreateAsync_Should_Map_Line_Items_To_Result_Dto()
    {
        var fixture = InvoiceApplicationFixture.Create();
        var request = TestData.CreateInvoiceRequest();

        var result = await fixture.CommandService.CreateAsync(request);

        result.LineItems.Should().BeEquivalentTo(
            request.LineItems.Select(x => new
            {
                x.Description,
                x.Quantity,
                Subtotal = x.Subtotal,
                Currency = request.Currency
            }));
    }

    [Fact]
    public async Task MarkAsPaidAsync_Should_Load_Invoice_Mark_It_As_Paid_And_Persist_Update()
    {
        var invoice = TestData.CreateInvoiceAggregate();
        var fixture = InvoiceApplicationFixture.Create().WithInvoices(invoice);

        var result = await fixture.CommandService.MarkAsPaidAsync(new MarkInvoiceAsPaidDto(invoice.Id.Value));

        result.InvoiceId.Should().Be(invoice.Id.Value);
        result.Status.Should().Be("Paid");
        fixture.Repository.UpdateCalls.Should().Be(1);
        fixture.Repository.StoredInvoices.Single().Status.ToString().Should().Be("Paid");
    }

    [Fact]
    public async Task MarkAsPaidAsync_Should_Throw_When_Invoice_Does_Not_Exist()
    {
        var fixture = InvoiceApplicationFixture.Create();

        var act = async () => await fixture.CommandService.MarkAsPaidAsync(new MarkInvoiceAsPaidDto(Guid.NewGuid()));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invoice was not found.");
    }
}
