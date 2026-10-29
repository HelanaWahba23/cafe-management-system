namespace CafeManagement.Api.Contracts;

public sealed record ReportCount(string Label, long Count);
public sealed record DailyCashReport(DateOnly Date, long Count, string Total);
public sealed record AdminReport(DateOnly From, DateOnly To, string Currency, long TotalOrders,
    ReportCount[] Statuses, ReportCount[] Fulfillment, ReportCount[] Origins,
    long CashReceiptCount, string CashRevenue, long SandboxReceiptCount, string SandboxValue,
    DailyCashReport[] DailyCash);
