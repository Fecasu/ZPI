using System.ComponentModel.DataAnnotations;

namespace WebStore.ViewModels.VM;

public class AddOrUpdateProductVm
{
    public int? Id { get; set; }
    [Required] public string Name { get; set; } = "";
    [Required] public string Description { get; set; } = "";
    [Required] public byte[] ImageBytes { get; set; } = [];
    public decimal Price { get; set; }
    public float Weight { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}

public record ProductVm(int Id, string Name, string Description, byte[] ImageBytes,
    decimal Price, float Weight, int CategoryId, int SupplierId, long Quantity);
