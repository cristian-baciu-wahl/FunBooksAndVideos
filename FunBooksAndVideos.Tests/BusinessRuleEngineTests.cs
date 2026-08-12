using FunBooksAndVideos.Application.Engines;
using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace FunBooksAndVideos.Tests;

public class BusinessRuleEngineTests
{
    private readonly Mock<ILogger<BusinessRuleEngine>> _logger = new();

    [Fact]
    public void ExecuteRules_ExecutesOnlyRulesForRequestedStage()
    {
        var preRule = new Mock<IBusinessRule>();
        preRule.SetupGet(x => x.RuleId).Returns("PRE");
        preRule.SetupGet(x => x.Priority).Returns(10);
        preRule.SetupGet(x => x.Stage).Returns(RuleExecutionStage.PreProcessing);
        preRule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>())).Returns(true);

        var postRule = new Mock<IBusinessRule>();
        postRule.SetupGet(x => x.RuleId).Returns("POST");
        postRule.SetupGet(x => x.Priority).Returns(20);
        postRule.SetupGet(x => x.Stage).Returns(RuleExecutionStage.PostProcessing);
        postRule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>())).Returns(true);

        var engine = CreateEngine(preRule.Object, postRule.Object);
        var order = new PurchaseOrder(4567890);

        engine.ExecuteRules(order, RuleExecutionStage.PreProcessing);

        preRule.Verify(x => x.Apply(order), Times.Once);
        postRule.Verify(x => x.Apply(It.IsAny<PurchaseOrder>()), Times.Never);
    }

    [Fact]
    public void ExecuteRules_ExecutesRulesInPriorityOrder()
    {
        var executionOrder = new List<string>();

        var lowPriority = CreateRule(
            "LOW",
            RuleExecutionStage.PreProcessing,
            20,
            executionOrder);

        var highPriority = CreateRule(
            "HIGH",
            RuleExecutionStage.PreProcessing,
            10,
            executionOrder);

        var engine = CreateEngine(lowPriority.Object, highPriority.Object);

        engine.ExecuteRules(
            new PurchaseOrder(4567890),
            RuleExecutionStage.PreProcessing);

        Assert.Equal(
            ["HIGH", "LOW"],
            executionOrder);
    }

    [Fact]
    public void ExecuteRules_DoesNotExecuteRule_WhenShouldApplyReturnsFalse()
    {
        var rule = new Mock<IBusinessRule>();

        rule.SetupGet(x => x.RuleId).Returns("TEST");
        rule.SetupGet(x => x.Priority).Returns(10);
        rule.SetupGet(x => x.Stage).Returns(RuleExecutionStage.PreProcessing);
        rule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>()))
            .Returns(false);

        var engine = CreateEngine(rule.Object);

        engine.ExecuteRules(
            new PurchaseOrder(4567890),
            RuleExecutionStage.PreProcessing);

        rule.Verify(
            x => x.Apply(It.IsAny<PurchaseOrder>()),
            Times.Never);
    }

    [Fact]
    public void ExecuteRules_ThrowsAggregateException_WhenRuleFails()
    {
        var rule = new Mock<IBusinessRule>();

        rule.SetupGet(x => x.RuleId).Returns("FAILING_RULE");
        rule.SetupGet(x => x.Priority).Returns(10);
        rule.SetupGet(x => x.Stage).Returns(RuleExecutionStage.PreProcessing);
        rule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>()))
            .Returns(true);

        rule.Setup(x => x.Apply(It.IsAny<PurchaseOrder>()))
            .Throws(new InvalidOperationException("Something failed"));

        var engine = CreateEngine(rule.Object);

        var exception = Assert.Throws<AggregateException>(() =>
            engine.ExecuteRules(
                new PurchaseOrder(4567890),
                RuleExecutionStage.PreProcessing));

        Assert.Contains("FAILING_RULE", exception.Message);
        Assert.Contains("Something failed", exception.Message);
    }

    [Fact]
    public void ExecuteRules_ContinuesExecutingRemainingRules_WhenOneFails()
    {
        var executionOrder = new List<string>();

        var failingRule = new Mock<IBusinessRule>();
        failingRule.SetupGet(x => x.RuleId).Returns("FAIL");
        failingRule.SetupGet(x => x.Priority).Returns(10);
        failingRule.SetupGet(x => x.Stage).Returns(RuleExecutionStage.PreProcessing);
        failingRule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>()))
            .Returns(true);

        failingRule.Setup(x => x.Apply(It.IsAny<PurchaseOrder>()))
            .Callback(() => executionOrder.Add("FAIL"))
            .Throws(new InvalidOperationException("Failure"));

        var successfulRule = CreateRule(
            "SUCCESS",
            RuleExecutionStage.PreProcessing,
            20,
            executionOrder);

        var engine = CreateEngine(
            failingRule.Object,
            successfulRule.Object);

        Assert.Throws<AggregateException>(() =>
            engine.ExecuteRules(
                new PurchaseOrder(4567890),
                RuleExecutionStage.PreProcessing));

        Assert.Equal(
            ["FAIL", "SUCCESS"],
            executionOrder);
    }

    [Fact]
    public void AddRule_Throws_WhenRuleWithSameIdAlreadyExists()
    {
        var rule = CreateRule(
            "RULE",
            RuleExecutionStage.PreProcessing,
            10);

        var engine = CreateEngine(rule.Object);

        var duplicate = CreateRule(
            "RULE",
            RuleExecutionStage.PostProcessing,
            20);

        Assert.Throws<InvalidOperationException>(() =>
            engine.AddRule(duplicate.Object));
    }

    [Fact]
    public void AddRule_Throws_WhenRuleIsNull()
    {
        var engine = CreateEngine();

        Assert.Throws<ArgumentNullException>(() =>
            engine.AddRule(null!));
    }

    [Fact]
    public void RemoveRule_RemovesExistingRule()
    {
        var rule = CreateRule(
            "RULE",
            RuleExecutionStage.PreProcessing,
            10);

        var engine = CreateEngine(rule.Object);

        engine.RemoveRule("RULE");

        engine.ExecuteRules(
            new PurchaseOrder(4567890),
            RuleExecutionStage.PreProcessing);

        rule.Verify(
            x => x.Apply(It.IsAny<PurchaseOrder>()),
            Times.Never);
    }

    [Fact]
    public void ExecuteRules_Throws_WhenOrderIsNull()
    {
        var engine = CreateEngine();

        Assert.Throws<ArgumentNullException>(() =>
            engine.ExecuteRules(
                null!,
                RuleExecutionStage.PreProcessing));
    }

    private BusinessRuleEngine CreateEngine(
        params IBusinessRule[] rules)
    {
        return new BusinessRuleEngine(
            _logger.Object,
            rules);
    }

    private static Mock<IBusinessRule> CreateRule(
        string id,
        RuleExecutionStage stage,
        int priority,
        List<string>? executionOrder = null)
    {
        var rule = new Mock<IBusinessRule>();

        rule.SetupGet(x => x.RuleId).Returns(id);
        rule.SetupGet(x => x.Stage).Returns(stage);
        rule.SetupGet(x => x.Priority).Returns(priority);
        rule.Setup(x => x.ShouldApply(It.IsAny<PurchaseOrder>()))
            .Returns(true);

        if (executionOrder is not null)
        {
            rule.Setup(x => x.Apply(It.IsAny<PurchaseOrder>()))
                .Callback(() => executionOrder.Add(id));
        }

        return rule;
    }
}