using Microsoft.Maui.Controls;

namespace ClothingCo
{
    public partial class MyAccountPage : ContentPage
    {
        private readonly OrderHistoryViewModel _viewModel;

        public MyAccountPage()
        {
            InitializeComponent();
            _viewModel = new OrderHistoryViewModel(new OrderService());
            _viewModel.OnOrderCanceled += OrderCanceled; 
            BindingContext = _viewModel;
        }

        private async void OnMyAccountButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string email = EmailEntry.Text;

                if (!string.IsNullOrEmpty(email))
                {
                    Console.WriteLine($"Retrieved email: {email}");

                    await _viewModel.LoadOrderHistoryAsync(email);

                    if (_viewModel.Orders.Count == 0)
                    {
                        await DisplayAlert("No Order History", "No order history found for this email.", "OK");
                    }
                    else
                    {
                        Console.WriteLine("Order history loaded successfully.");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "User email is not set. Please log in.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error has occurred.", "OK");
            }
        }
        private async void CancelOrder(object sender, EventArgs args) 
        { 
            try 
            { if (sender is Button button && button.CommandParameter is int orderId) 
                { 
                    await _viewModel.CancelOrder(orderId); 
                } 
            } 
            catch (Exception ex) 
            { 
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK"); 
                Console.WriteLine(ex); 
            }
        }
        private async void OrderCanceled(int orderId) 
        {
            await DisplayAlert("Order Canceled", $"Order {orderId} has been canceled successfully.", "OK");
        }

    }
}