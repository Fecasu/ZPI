namespace WebStore.ViewModels.VM;

public class AddOrUpdateOrderVm
{
    public int? Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public long? TrackingNumber { get; set; }
    public List<OrderLineVm> Products { get; set; } = [];
}

public record OrderLineVm(int ProductId, int Quantity);
public record OrderVm(int Id, int CustomerId, DateTime OrderDate, DateTime? DeliveryDate,
    long? TrackingNumber, decimal TotalAmount, int? InvoiceId, IReadOnlyList<OrderLineVm> Products);

public class AddOrUpdateInvoiceVm
{
    public int? Id { get; set; }
    public string Number { get; set; } = "";
    public DateTime IssueDate { get; set; }
    public List<int> OrderIds { get; set; } = [];
}

public record InvoiceVm(int Id, string Number, DateTime IssueDate, decimal Amount, IReadOnlyList<int> OrderIds);
