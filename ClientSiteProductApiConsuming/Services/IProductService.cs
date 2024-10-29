using ClientSiteProductApiConsuming.Models;

namespace ClientSiteProductApiConsuming.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync(string token);
        Task<bool> AddProductAsync(Product product, IFormFile imageFile, string token);
        Task <bool> UpdateProductAsync(Product product, IFormFile productImage, string token);
        Task <bool> DeleteProductAsync(int id, string token);
    }
}
