using System.ComponentModel.DataAnnotations;
namespace CafeManagement.Api.Contracts;
public sealed class CustomerProfileInput
{
 [Required,StringLength(120,MinimumLength=1)] public string DisplayName {get;set;}="";
 [Required,StringLength(40,MinimumLength=1)] public string Phone {get;set;}="";
 [StringLength(500)] public string? Address {get;set;}
 public string RowVersion {get;set;}="";
}
public sealed record CustomerOrderDetails(OrderTracking Order,string? DeliveryAddress,string PaymentState);
