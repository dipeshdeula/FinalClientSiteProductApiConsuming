using System.ComponentModel.DataAnnotations;

namespace ClientSiteProductApiConsuming.Models
{
    public class Product
    {
        public int Id { get; set; }

        //[Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Price is required")]
        //[Range(0.01, double.MaxValue,ErrorMessage ="Price must be greater than zero")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        // Computed property to return ImageUrl without extension
        public string ImageUrlWithoutExtension
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                    return ImageUrl;

                return Path.GetFileNameWithoutExtension(ImageUrl);
            }
        }
    }
}
