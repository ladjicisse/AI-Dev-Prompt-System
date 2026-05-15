using System.Net;
using FluentAssertions;

namespace LADCI.AI.Invoicing.IntegrationTests.API;

public sealed class GetInvoicesEndpointTests : IClassFixture<InvoicesApiFactory>
{
    private readonly HttpClient _client;

    public GetInvoicesEndpointTests(InvoicesApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_CustomerInvoices_Should_Return_All_Invoices_For_That_Customer()
    {
        var customerId = Guid.NewGuid();
        await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest(customerId, 100m));
        await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest(customerId, 250m));
        await _client.CreateInvoiceAsync(InvoiceApiRequests.ValidCreateInvoiceRequest(Guid.NewGuid(), 999m));

        var response = await _client.GetAsync($"/api/customers/{customerId}/invoices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoices = await response.Content.ReadInvoicesAsync();
        invoices.Should().NotBeNull();
        invoices!.Should().HaveCount(2);
        invoices.Should().OnlyContain(x => x.CustomerId == customerId);
    }

    [Fact]
    public async Task Get_CustomerInvoices_Should_Return_Empty_When_The_Customer_Has_No_Invoices()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}/invoices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoices = await response.Content.ReadInvoicesAsync();
        invoices.Should().NotBeNull();
        invoices!.Should().BeEmpty();
    }
}
