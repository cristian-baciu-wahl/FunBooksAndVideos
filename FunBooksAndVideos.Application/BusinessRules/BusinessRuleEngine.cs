using FunBooksAndVideos.Domain.PurchaseOrders;
using Microsoft.Extensions.Logging;

namespace FunBooksAndVideos.Application.BusinessRules;

public class BusinessRuleEngine : IBusinessRuleEngine
{
    private readonly List<IBusinessRule> _rules = [];
    private readonly ILogger<BusinessRuleEngine> _logger;

    public BusinessRuleEngine(
        ILogger<BusinessRuleEngine> logger, 
        IEnumerable<IBusinessRule> rules)
    {
        _logger = logger;
        _rules.AddRange(rules);
    }
    public void AddRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (_rules.Any(r => r.RuleId == rule.RuleId))
            throw new InvalidOperationException($"Rule with ID {rule.RuleId} already exists");

        _rules.Add(rule);
        _logger.LogInformation($"Added rule: {rule.RuleId} with priority {rule.Priority}");
    }

    public void RemoveRule(string ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.RuleId == ruleId);
        if (rule != null)
        {
            _rules.Remove(rule);
            _logger.LogInformation($"Removed rule: {ruleId}");
        }
    }

    public async Task ExecuteRulesAsync(PurchaseOrder order, RuleExecutionStage stage, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        // Lower numbers indicate higher priority
        var rules = _rules
            .Where(rule => rule.Stage == stage && rule.ShouldApply(order))
            .OrderBy(rule => rule.Priority)
            .ToList();

        _logger.LogInformation($"Executing {rules.Count} rules for order {order.Id}");

        var errors = new List<string>();
        foreach (var rule in rules)
        {
            try
            {
                await rule.ApplyAsync(order, cancellationToken);
                _logger.LogInformation($"Successfully applied rule: {rule.RuleId} to order {order.Id}");
            }
            catch (ArgumentException ex)
            {
                errors.Add($"Rule {rule.RuleId} failed: {ex.Message}");
                _logger.LogError(ex, $"Error applying rule {rule.RuleId} to order {order.Id}");
            }
            catch (InvalidOperationException ex)
            {
                errors.Add($"Rule {rule.RuleId} failed: {ex.Message}");
                _logger.LogError(ex, $"Error applying rule {rule.RuleId} to order {order.Id}");
            }
            catch (Exception ex)
            {
                errors.Add($"Rule {rule.RuleId} failed: {ex.Message}");
                _logger.LogError(ex, $"Unexpected error applying rule {rule.RuleId} to order {order.Id}");
            }
        }

        if (errors.Any())
        {
            throw new AggregateException($"Failed to process order: {string.Join("; ", errors)}");
        }
    }
}