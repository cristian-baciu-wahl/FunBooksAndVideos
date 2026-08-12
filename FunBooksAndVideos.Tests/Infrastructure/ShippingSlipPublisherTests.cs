using FunBooksAndVideos.Infrastructure.Fullfilment;

namespace FunBooksAndVideos.Tests.Infrastructure;

public class ShippingSlipPublisherTests
{
    [Fact]
    public async Task PublishAsync_AddsShippingSlip()
    {
        var sut = new InMemoryShippingSlipPublisher();

        await sut.PublishAsync(123, 4567890, CancellationToken.None);

        var slip = Assert.Single(sut.PublishedSlips);

        Assert.Equal(123, slip.PurchaseOrderId);
        Assert.Equal(4567890, slip.CustomerId);
    }

    [Fact]
    public async Task PublishAsync_CanPublishMultipleShippingSlips()
    {
        var sut = new InMemoryShippingSlipPublisher();

        await sut.PublishAsync(123, 4567890, CancellationToken.None);
        await sut.PublishAsync(124, 4567891, CancellationToken.None);

        Assert.Equal(2, sut.PublishedSlips.Count);

        Assert.Contains(
            sut.PublishedSlips,
            slip => slip.PurchaseOrderId == 123 &&
                    slip.CustomerId == 4567890);

        Assert.Contains(
            sut.PublishedSlips,
            slip => slip.PurchaseOrderId == 124 &&
                    slip.CustomerId == 4567891);
    }

    [Fact]
    public async Task PublishAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var sut = new InMemoryShippingSlipPublisher();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => sut.PublishAsync(
                123,
                4567890,
                cancellationTokenSource.Token));

        Assert.Empty(sut.PublishedSlips);
    }
}