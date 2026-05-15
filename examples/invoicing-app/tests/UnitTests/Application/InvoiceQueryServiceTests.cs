using FluentAssertions;
using LADCI.AI.Invoicing.Application.Invoices.Queries;

namespace LADCI.AI.Invoicing.UnitTests.Application;

public sealed class InvoiceQueryServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Only_Invoices_For_The_Requested_Customer()
    {
        var customerId = Guid.NewGuid();
        var fixture = InvoiceApplicationFixture.Create().WithInvoices(
            TestData.CreateInvoiceAggregate(customerId: customerId, totalAmount: 100m),
            TestData.CreateInvoiceAggregate(customerId: customerId, totalAmount: 250m),
            TestData.CreateInvoiceAggregate(customerId: Guid.NewGuid(), totalAmount: 999m));

        var result = await fixture.QueryService.GetAllAsync(new GetInvoicesDto(customerId));

        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.CustomerId == customerId);
        result.Select(x => x.TotalAmount).Should().BeEquivalentTo([100m, 250m]);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_An_Empty_Collection_When_User_Has_No_Invoices()
    {
        var fixture = InvoiceApplicationFixture.Create();

        var result = await fixture.QueryService.GetAllAsync(new GetInvoicesDto(Guid.NewGuid()));

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDetailsAsync_Should_Return_Invoice_Details_For_The_Requested_User_And_Invoice()
    {
        var invoice = TestData.CreateInvoiceAggregate(totalAmount: 175m);
        var fixture = InvoiceApplicationFixture.Create().WithInvoices(invoice);

        var result = await fixture.QueryService.GetDetailsAsync(new GetInvoiceDetailsDto(invoice.CustomerId.Value, invoice.Id.Value));

        result.InvoiceId.Should().Be(invoice.Id.Value);
        result.CustomerId.Should().Be(invoice.CustomerId.Value);
        result.TotalAmount.Should().Be(175m);
        result.Currency.Should().Be("EUR");
        result.LineItems.Should().HaveCount(invoice.LineItems.Count);
    }

    [Fact]
    public async Task GetDetailsAsync_Should_Throw_When_Invoice_Does_Not_Belong_To_The_User()
    {
        var invoice = TestData.CreateInvoiceAggregate();
        var fixture = InvoiceApplicationFixture.Create().WithInvoices(invoice);

        var act = async () => await fixture.QueryService.GetDetailsAsync(new GetInvoiceDetailsDto(Guid.NewGuid(), invoice.Id.Value));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invoice was not found.");
    }

    [Fact]
    public async Task GetDetailsAsync_Should_Throw_When_Invoice_Does_Not_Exist()
    {
        var fixture = InvoiceApplicationFixture.Create();

        var act = async () => await fixture.QueryService.GetDetailsAsync(new GetInvoiceDetailsDto(Guid.NewGuid(), Guid.NewGuid()));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invoice was not found.");
    }
}
