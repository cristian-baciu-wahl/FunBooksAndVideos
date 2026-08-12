using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

/// <summary>
/// Rule engine that orchestrates the execution of business rules
/// </summary>
public interface IBusinessRuleEngine
{
    /// <summary>
    /// Executes all applicable business rules for a purchase order
    /// </summary>
    void ExecuteRules(PurchaseOrder order, RuleExecutionStage stage);

    /// <summary>
    /// Adds a rule to the engine
    /// </summary>
    void AddRule(IBusinessRule rule);

    /// <summary>
    /// Removes a rule from the engine
    /// </summary>
    void RemoveRule(string ruleId);
}

