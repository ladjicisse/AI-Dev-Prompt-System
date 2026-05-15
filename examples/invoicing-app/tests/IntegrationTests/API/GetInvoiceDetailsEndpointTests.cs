using System.Net;
using FluentAssertions;

namespace LADCI.AI.Invoicing.IntegrationTests.API;

public sealed class GetInvoiceDetailsEndpointTests : IClassFixture<InvoicesApiFactory>
{
    private readonly HttpClient _client;

    public GetInvoiceDetailsEndpointTests(InvoicesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_InvoiceDetails_Should_Return_The_Invoice_For_The_Owning_Customer()
    {
        var createResponse = await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest(totalAmount: 175m));
        var createdInvoice = await createResponse.Content.ReadInvoiceAsync();

        var response = await _client.GetAsync($"/api/customers/{createdInvoice!.CustomerId}/invoices/{createdInvoice.InvoiceId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoice = await response.Content.ReadInvoiceAsync();
        invoice.Should().NotBeNull();
        invoice!.InvoiceId.Should().Be(createdInvoice.InvoiceId);
        invoice.CustomerId.Should().Be(createdInvoice.CustomerId);
        invoice.TotalAmount.Should().Be(175m);
    }

    [Fact]
    public async Task Get_InvoiceDetails_Should_Return_NotFound_When_Invoice_Does_Not_Belong_To_The_Customer()
    {
        var createResponse = await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest());
        var createdInvoice = await createResponse.Content.ReadInvoiceAsync();

        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}/invoices/{createdInvoice!.InvoiceId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_InvoiceDetails_Should_Return_NotFound_When_Invoice_Does_Not_Exist()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}/invoices/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
