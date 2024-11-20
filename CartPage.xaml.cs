using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using ClothingCo;
using Microsoft.Identity.Client;

namespace ClothingCo
{
    public partial class CartPage : ContentPage
    {
        public ObservableCollection<Product> CartItems { get; set; }

        public CartPage(List<Product> cartItems)
        {
            InitializeComponent();
            CartItems = new ObservableCollection<Product>(cartItems);
            BindingContext = this;
        }
        public string TotalPrice
        {
            get
            {
                decimal total = CartItems.Sum(item => item.PriceDecimal * item.Quantity);
                return $"${total:0.00}";
            }
         }
        
        private void RemoveItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var productToRemove = button?.CommandParameter as Product;

            if (productToRemove != null)
            {
                CartItems.Remove(productToRemove);
                OnPropertyChanged("Product");
                OnPropertyChanged("TotalPrice");
               
            }
        }
        private async void GoToCheckout(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaymentPage());
        }
    }
}
        










        
       

