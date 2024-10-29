using ClientSiteProductApiConsuming.Models;
using System.Net.Http.Headers;


namespace ClientSiteProductApiConsuming.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("https://localhost:7269/api/Products");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        { 
            var product = await _httpClient.GetFromJsonAsync<Product>($"https://localhost:7269/api/Products/{id}");
            if (product == null)
            {
                throw new KeyNotFoundException("Product Not found");
            }
            return product;
        }


        public async Task<bool> AddProductAsync(Product product, IFormFile imageFile, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(product.Id.ToString()), "Id");
            formData.Add(new StringContent(product.Name), "Name");
            formData.Add(new StringContent(product.Description), "Description");
            formData.Add(new StringContent(product.Price.ToString()), "Price");

            if (imageFile != null)
            {
                using (var stream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(stream);
                    var imageContent = new ByteArrayContent(stream.ToArray());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    formData.Add(imageContent, "productImage", imageFile.FileName);
                }
            }
        
            var response = await _httpClient.PostAsync("https://localhost:7269/api/Products/", formData);
            return response != null && response.IsSuccessStatusCode;


        }


        // Other methods follow a similar pattern
        public async Task<bool> UpdateProductAsync(Product product, IFormFile imageFile, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(product.Id.ToString()), "Id");
            formData.Add(new StringContent(product.Name), "Name");
            formData.Add(new StringContent(product.Description), "Description");
            formData.Add(new StringContent(product.Price.ToString()), "Price");

            if (imageFile != null)
            {
                using (var stream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(stream);
                    var imageContent = new ByteArrayContent(stream.ToArray());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    formData.Add(imageContent, "productImage", imageFile.FileName);
                }
            }

            var response = await _httpClient.PutAsync($"https://localhost:7269/api/Products/{product.Id}", formData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
               // _logger.LogError($"Error updating product: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteProductAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"http://localhost:7269/api/Products/{id}");
           
            return response.IsSuccessStatusCode;
        }

       

    }
}
