namespace ClientSiteProductApiConsuming.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Product> Products { get; set; } = null!;
    }
}
