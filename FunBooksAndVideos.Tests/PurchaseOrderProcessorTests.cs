using FunBooksAndVideos.Application.Exceptions;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Models;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Domain;
using Moq;

namespace FunBooksAndVideos.Tests.Processors;

public class PurchaseOrderProcessorTests
{
    private const int CustomerId = 4567890;

    private readonly CancellationToken _cancellationToken =
        new CancellationTokenSource().Token;

    private readonly Mock<IPurchaseOrderRepository> _repo = new();
    private readonly Mock<IBusinessRuleEngine> _ruleEngine = new();

    private readonly PurchaseOrderProcessor _sut;

    public PurchaseOrderProcessorTests()
    {
        _sut = new PurchaseOrderProcessor(
            _ruleEngine.Object,
            _repo.Object);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.ProcessPurchaseOrderAsync(
                null!,
                _cancellationToken));
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WhenProductDoesNotExist_DoesNotSaveOrExecuteRules()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 999,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                999,
                _cancellationToken))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.ProcessPurchaseOrderAsync(
                request,
                _cancellationToken));

        _repo.Verify(
            x => x.SavePurchaseOrderAsync(
                It.IsAny<PurchaseOrder>(),
                _cancellationToken),
            Times.Never);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PreProcessing,
                _cancellationToken),
            Times.Never);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PostProcessing,
                _cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithDigitalProduct_CreatesCorrectOrder()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 2,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                2,
                _cancellationToken))
            .ReturnsAsync(CreateVideo());

        var order = await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        Assert.Equal(CustomerId, order.CustomerId);
        Assert.Single(order.ItemLines);

        var line = Assert.IsType<ProductOrderLine>(
            order.ItemLines[0]);

        Assert.IsType<Video>(line.Product);
        Assert.Equal(33.51m, line.UnitPrice);
        Assert.Equal(33.51m, order.TotalPrice);

        _repo.Verify(
            x => x.SavePurchaseOrderAsync(
                order,
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithPhysicalProduct_CreatesCorrectOrder()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        var order = await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        Assert.Equal(CustomerId, order.CustomerId);
        Assert.Single(order.ItemLines);

        var line = Assert.IsType<ProductOrderLine>(
            order.ItemLines[0]);

        Assert.IsType<Book>(line.Product);
        Assert.Equal(14.99m, line.UnitPrice);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(29.98m, order.TotalPrice);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithMembership_CreatesMembershipLine()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    MembershipType = MembershipType.BookClub.ToString(),
                    Quantity = 1
                }
            ]
        };

        var order = await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        Assert.Single(order.ItemLines);

        var line = Assert.IsType<MembershipOrderLine>(
            order.ItemLines[0]);

        Assert.Equal(
            MembershipType.BookClub,
            line.MembershipType);

        Assert.Equal(0m, order.TotalPrice);

        _repo.Verify(
            x => x.GetProductByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_WithMultipleItems_CreatesAllLines()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                },
                new PurchaseOrderItemRequest
                {
                    ProductId = 2,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        _repo
            .Setup(x => x.GetProductByIdAsync(
                2,
                _cancellationToken))
            .ReturnsAsync(CreateVideo());

        var order = await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        Assert.Equal(2, order.ItemLines.Count);
        Assert.Equal(63.49m, order.TotalPrice);

        _repo.Verify(
            x => x.GetProductByIdAsync(
                1,
                _cancellationToken),
            Times.Once);

        _repo.Verify(
            x => x.GetProductByIdAsync(
                2,
                _cancellationToken),
            Times.Once);

        _repo.Verify(
            x => x.SavePurchaseOrderAsync(
                order,
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_UsesCataloguePrice()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        var order = await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        var line = Assert.IsType<ProductOrderLine>(
            order.ItemLines.Single());

        Assert.Equal(14.99m, line.UnitPrice);
        Assert.Equal(29.98m, order.TotalPrice);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_ExecutesPreProcessingBeforeSaveAndPostProcessingAfterSave()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        var sequence = new MockSequence();

        _ruleEngine
            .InSequence(sequence)
            .Setup(x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PreProcessing,
                _cancellationToken));

        _repo
            .InSequence(sequence)
            .Setup(x => x.SavePurchaseOrderAsync(
                It.IsAny<PurchaseOrder>(),
                _cancellationToken));

        _ruleEngine
            .InSequence(sequence)
            .Setup(x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PostProcessing,
                _cancellationToken));

        await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PreProcessing,
                _cancellationToken),
            Times.Once);

        _repo.Verify(
            x => x.SavePurchaseOrderAsync(
                It.IsAny<PurchaseOrder>(),
                _cancellationToken),
            Times.Once);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PostProcessing,
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_PropagatesCancellationTokenToRepository()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        _repo.Verify(
            x => x.GetProductByIdAsync(
                1,
                _cancellationToken),
            Times.Once);

        _repo.Verify(
            x => x.SavePurchaseOrderAsync(
                It.IsAny<PurchaseOrder>(),
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPurchaseOrderAsync_PropagatesCancellationTokenToRuleEngine()
    {
        var request = new PurchaseOrderRequest
        {
            CustomerId = CustomerId,
            Items =
            [
                new PurchaseOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
            ]
        };

        _repo
            .Setup(x => x.GetProductByIdAsync(
                1,
                _cancellationToken))
            .ReturnsAsync(CreateBook());

        await _sut.ProcessPurchaseOrderAsync(
            request,
            _cancellationToken);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PreProcessing,
                _cancellationToken),
            Times.Once);

        _ruleEngine.Verify(
            x => x.ExecuteRulesAsync(
                It.IsAny<PurchaseOrder>(),
                RuleExecutionStage.PostProcessing,
                _cancellationToken),
            Times.Once);
    }

    private static Book CreateBook() =>
        new()
        {
            Id = 1,
            Name = "The Girl on the Train",
            Author = "Paula Hawkins",
            Isbn = "9781234567897",
            Price = 14.99m
        };

    private static Video CreateVideo() =>
        new()
        {
            Id = 2,
            Name = "Comprehensive First Aid Training",
            Director = "John Smith",
            Price = 33.51m
        };
}
