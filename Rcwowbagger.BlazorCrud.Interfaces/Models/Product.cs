namespace Rcwowbagger.BlazorCrud.Interfaces.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}
