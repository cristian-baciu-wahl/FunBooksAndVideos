using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

/// <summary>
/// Strategy interface for business rules
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Determines if this rule should be applied to the purchase order
    /// </summary>
    bool ShouldApply(PurchaseOrder order);

    /// <summary>
    /// Executes the business rule
    /// </summary>
    void Apply(PurchaseOrder order);

    /// <summary>
    /// Priority of the rule (higher number = higher priority)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Rule identifier for logging and tracking
    /// </summary>
    string RuleId { get; }
}
