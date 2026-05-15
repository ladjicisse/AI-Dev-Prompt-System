using System.Net;
using FluentAssertions;
using LADCI.AI.Invoicing.API.Contracts;

namespace LADCI.AI.Invoicing.IntegrationTests.API;

public sealed class CreateInvoiceEndpointTests : IClassFixture<InvoicesApiFactory>
{
    private readonly HttpClient _client;

    public CreateInvoiceEndpointTests(InvoicesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Invoices_Should_Create_Invoice_And_Return_Created()
    {
        var request = InvoiceApiRequests.ValidCreateInvoiceRequest();

        var response = await _client.CreateInvoiceAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var invoice = await response.Content.ReadInvoiceAsync();
        invoice.Should().NotBeNull();
        invoice!.CustomerId.Should().Be(request.CustomerId);
        invoice.Status.Should().Be("Draft");
        invoice.TotalAmount.Should().Be(request.TotalAmount);
        invoice.Currency.Should().Be(request.Currency);
        invoice.LineItems.Should().HaveCount(request.LineItems.Count);
    }

    [Fact]
    public async Task Post_Invoices_Should_Return_BadRequest_When_LineItems_Are_Missing()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(),
            new DateOnly(2026, 4, 28),
            new DateOnly(2026, 5, 28),
            [],
            100m,
            "EUR");

        var response = await _client.CreateInvoiceAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Invoices_Should_Return_BadRequest_When_Currency_Is_Empty()
    {
        var request = InvoiceApiRequests.ValidCreateInvoiceRequest(currency: string.Empty);

        var response = await _client.CreateInvoiceAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
