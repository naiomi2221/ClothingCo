using Microsoft.Maui.Controls;using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;



namespace ClothingCo
{

    public partial class ShoppingPage : ContentPage
    {
        private List<Product> cartItems = new List<Product>();
        public List<Product> Products { get; set; }


        public ShoppingPage()
        {
            InitializeComponent();


            Products = new List<Product>
        {
                new Product { Name = "Shirt", Price = "$29.99",PriceDecimal = 29.99m, Image = "shirt.jpg" },
                new Product { Name = "Pullover", Price = "$49.99", PriceDecimal= 49.99m, Image = "pullover.jpg" },
                new Product { Name = "Joggers", Price = "$39.99", PriceDecimal= 39.99m,  Image = "joggers.jpg" },
                new Product { Name = "Socks", Price = "$19.99", PriceDecimal=19.99m,  Image = "socks.jpg" },
                new Product { Name = "Long Sleeve Shirt", Price = "$29.99", PriceDecimal=29.99m, Image = "longsleeveshirt.jpg" },
                new Product { Name = "Shorts", Price = "$24.99", PriceDecimal=24.99m, Image = "shorts.jpg" },
                new Product { Name = "Khakis", Price = "$44.99", PriceDecimal=44.99m, Image = "khakis.jpg" }
        };
            BindingContext = this;
        }

        private void AddToCart_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var product = button?.CommandParameter as Product;

            if (product != null)
            {
                var existingProduct = cartItems.FirstOrDefault(p => p.Name == product.Name);
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    cartItems.Add(product);
                }

                DisplayAlert("Added to Cart", $"{product.Name} added to cart.", "OK");
            }
        }
        private async void OnAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyAccountPage());
        }
        private async void OnCartClicked(object sender, EventArgs e)
        {

            var cartPage = new CartPage(cartItems); 
            await Navigation.PushAsync(cartPage);
        }
      
       private void DecreaseQuantity_Clicked(object sender, EventArgs e)
       {
            var button = sender as Button;
            var productToUpdate = button?.CommandParameter as Product;

            if(productToUpdate != null)
            {
                productToUpdate.Quantity--;
            }
       }
        private void IncreaseQuantity_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var productToUpdate = button?.CommandParameter as Product;

            if (productToUpdate != null)
            {
                productToUpdate.Quantity++;
                BindingContext = this;
            }
        }
        private void RemoveItem_Clicked(object sender, EventArgs e)
        {

        }
    }
    
    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Image { get; set; } 
        public decimal PriceDecimal { get; set; }
        public int Quantity {  get; set; }
    }

}
