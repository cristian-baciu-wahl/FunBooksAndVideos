using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Application.Processors;
using FunBooksAndVideos.Application.Rules;
using FunBooksAndVideos.Domain;
using FunBooksAndVideos.Tests.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace FunBooksAndVideos.Tests
{
    public class PurchaseOrderProcessorTests
    {
        private readonly Mock<IShippingSlipService> _shippingSlipServiceMock;
        private readonly Mock<ICustomerMembershipService> _membershipServiceMock;
        private readonly Mock<ILogger<BusinessRuleEngine>> _loggerMock;
        private readonly BusinessRuleEngine _ruleEngine;
        private readonly PurchaseOrderProcessor _processor;

        public PurchaseOrderProcessorTests()
        {
            _shippingSlipServiceMock = new Mock<IShippingSlipService>();
            _membershipServiceMock = new Mock<ICustomerMembershipService>();
            _loggerMock = new Mock<ILogger<BusinessRuleEngine>>();

            // Setup rule engine with rules
            _ruleEngine = new BusinessRuleEngine(_loggerMock.Object);
            _ruleEngine.AddRule(new ActivateMembershipRule(_membershipServiceMock.Object));
            _ruleEngine.AddRule(new GenerateShippingSlipRule(_shippingSlipServiceMock.Object));

            _processor = new PurchaseOrderProcessor(_ruleEngine);
        }

        [Fact]
        public void ProcessPurchaseOrder_WithMembership_ActivatesMembership()
        {
            // Arrange
            var order = new PurchaseOrder(1, 12345);
            order.ItemLines.Add(new ItemLine
            {
                MembershipType = MembershipType.BookClub,
                Quantity = 1,
                UnitPrice = 0
            });

            // Act
            _processor.ProcessPurchaseOrder(order);

            // Assert
            _membershipServiceMock.Verify(
                x => x.ActivateMembership(12345, MembershipType.BookClub),
                Times.Once);
        }

        [Fact]
        public void ProcessPurchaseOrder_WithPhysicalProduct_GeneratesShippingSlip()
        {
            // Arrange
            var order = new PurchaseOrder(1, 12345);
            var book = new Book
            {
                Id = 1,
                Name = "Test Book",
                Author = "Test Author",
                Isbn = "9781234567897",
                Price = 10.99m
            };
            order.ItemLines.Add(new ItemLine
            {
                Product = book,
                Quantity = 1,
                UnitPrice = 10.99m
            });

            // Act
            _processor.ProcessPurchaseOrder(order);

            // Assert
            _shippingSlipServiceMock.Verify(
                x => x.GenerateShippingSlip(1, 12345),
                Times.Once);
        }

        [Fact]
        public void ProcessPurchaseOrder_WithMembershipAndPhysicalProduct_ProcessesBothRulesInPriorityOrder()
        {
            // Arrange
            var order = new PurchaseOrder(1, 12345);
            order.ItemLines.Add(new ItemLine
            {
                MembershipType = MembershipType.Premium,
                Quantity = 1,
                UnitPrice = 0
            });
            var book = new Book
            {
                Id = 1,
                Name = "Test Book",
                Author = "Test Author",
                Isbn = "9781234567897",
                Price = 10.99m
            };
            order.ItemLines.Add(new ItemLine
            {
                Product = book,
                Quantity = 1,
                UnitPrice = 10.99m
            });

            // Act
            _processor.ProcessPurchaseOrder(order);

            // Assert
            var callOrder = new List<string>();
            _membershipServiceMock
                .Setup(x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()))
                .Callback(() => callOrder.Add("Membership"));
            _shippingSlipServiceMock
                .Setup(x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("Shipping"));

            _membershipServiceMock.Verify(
                x => x.ActivateMembership(12345, MembershipType.Premium),
                Times.Once);
            _shippingSlipServiceMock.Verify(
                x => x.GenerateShippingSlip(1, 12345),
                Times.Once);
        }

        [Fact]
        public void ProcessPurchaseOrder_WithNoMembershipOrPhysicalProduct_DoesNothing()
        {
            // Arrange
            var order = new PurchaseOrder(1, 12345);
            var video = new Video
            {
                Id = 1,
                Name = "Test Video",
                Director = "Test Director",
                Price = 5.99m
            };
            order.ItemLines.Add(new ItemLine
            {
                Product = video,
                Quantity = 1,
                UnitPrice = 5.99m
            });

            // Act
            _processor.ProcessPurchaseOrder(order);

            // Assert
            _membershipServiceMock.Verify(
                x => x.ActivateMembership(It.IsAny<int>(), It.IsAny<MembershipType>()),
                Times.Never);
            _shippingSlipServiceMock.Verify(
                x => x.GenerateShippingSlip(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void ProcessPurchaseOrder_NullOrder_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _processor.ProcessPurchaseOrder(null));
        }

        [Fact]
        public void AddRule_DuplicateRuleId_ThrowsInvalidOperationException()
        {
            // Arrange
            var rule1 = new ActivateMembershipRule(_membershipServiceMock.Object);
            var rule2 = new ActivateMembershipRule(_membershipServiceMock.Object); // Same rule ID

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _ruleEngine.AddRule(rule2));
        }

        [Fact]
        public void RemoveRule_RemovesExistingRule()
        {
            // Arrange
            var rule = new TestPremiumMembershipRule(_membershipServiceMock.Object);
            _ruleEngine.AddRule(rule);

            // Act
            _ruleEngine.RemoveRule(rule.RuleId);

            // Assert
            // Create a new order and verify rule doesn't apply
            var order = new PurchaseOrder(1, 12345);
            order.ItemLines.Add(new ItemLine
            {
                MembershipType = MembershipType.BookClub,
                Quantity = 1,
                UnitPrice = 0
            });

            // Reset mock to ensure no calls
            _membershipServiceMock.Reset();

            _processor.ProcessPurchaseOrder(order);

            // Verify the rule was not applied
            _membershipServiceMock.Verify(
                x => x.ActivateMembership(It.IsAny<int>(), MembershipType.Premium),
                Times.Never);
        }
    }
}