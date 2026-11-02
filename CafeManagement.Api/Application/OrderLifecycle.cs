using CafeManagement.Api.Domain;

namespace CafeManagement.Api.Application;

internal static class OrderLifecycle
{
    internal static bool TryTransition(Order order, OrderStatus next, string? rejectionReason = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        var allowed = (order.Status, next) switch
        {
            (OrderStatus.Submitted, OrderStatus.Preparing) => true,
            (OrderStatus.Submitted, OrderStatus.Accepted) => true,
            (OrderStatus.Accepted, OrderStatus.Preparing) => true,
            (OrderStatus.Accepted, OrderStatus.Rejected) => true,
            (OrderStatus.Preparing, OrderStatus.Ready) => true,
            (OrderStatus.Ready, OrderStatus.Completed) => order.Type != OrderType.Delivery,
            (OrderStatus.Ready, OrderStatus.OutForDelivery) => order.Type == OrderType.Delivery,
            (OrderStatus.OutForDelivery, OrderStatus.Completed) => order.Type == OrderType.Delivery,
            (OrderStatus.Submitted, OrderStatus.Rejected) => true,
            _ => false
        };

        if (!allowed)
        {
            return false;
        }

        var reason = rejectionReason?.Trim();
        if (next == OrderStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500)
            {
                return false;
            }
        }
        else if (rejectionReason is not null)
        {
            return false;
        }

        order.Status = next;
        order.RejectionReason = reason;
        return true;
    }
}
