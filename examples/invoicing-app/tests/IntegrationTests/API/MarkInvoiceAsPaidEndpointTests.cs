using System.Net;
using FluentAssertions;

namespace LADCI.AI.Invoicing.IntegrationTests.API;

public sealed class MarkInvoiceAsPaidEndpointTests : IClassFixture<InvoicesApiFactory>
{
    private readonly HttpClient _client;

    public MarkInvoiceAsPaidEndpointTests(InvoicesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_InvoicePayment_Should_Mark_Invoice_As_Paid_And_Return_Ok()
    {
        var createResponse = await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest());
        var createdInvoice = await createResponse.Content.ReadInvoiceAsync();

        var response = await _client.PostAsync($"/api/invoices/{createdInvoice!.InvoiceId}/payments", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoice = await response.Content.ReadInvoiceAsync();
        invoice.Should().NotBeNull();
        invoice!.InvoiceId.Should().Be(createdInvoice.InvoiceId);
        invoice.Status.Should().Be("Paid");
    }

    [Fact]
    public async Task Post_InvoicePayment_Should_Return_NotFound_When_Invoice_Does_Not_Exist()
    {
        var response = await _client.PostAsync($"/api/invoices/{Guid.NewGuid()}/payments", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
