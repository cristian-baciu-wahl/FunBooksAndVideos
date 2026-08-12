using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

/// <summary>
/// Represents a business rule that can be applied to a purchase order.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the priority of the business rule. Rules with lower priority values are applied first.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the unique identifier of the business rule.
    /// </summary>
    string RuleId { get; }

    /// <summary>
    /// Gets the stage at which the business rule should be applied (pre-processing or post-processing).
    /// </summary>
    RuleExecutionStage Stage { get; }

    bool ShouldApply(PurchaseOrder order);

    void Apply(PurchaseOrder order);
}
