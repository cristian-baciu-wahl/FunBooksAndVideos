using FunBooksAndVideos.Domain;

namespace FunBooksAndVideos.Application.Interfaces;

public interface IBusinessRule
{
    bool ShouldApply(PurchaseOrder order);

    void Apply(PurchaseOrder order);

    int Priority { get; }

    string RuleId { get; }
}
