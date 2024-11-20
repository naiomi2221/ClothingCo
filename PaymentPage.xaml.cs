using Microsoft.IdentityModel.Tokens;

namespace ClothingCo;

public partial class PaymentPage : ContentPage
{
    private Order order;
    private OrderService orderService;
    private OrderHistoryViewModel orderHistoryViewModel;

    public PaymentPage()
    {
        InitializeComponent();
        orderService = new OrderService();
        orderHistoryViewModel = new OrderHistoryViewModel(orderService);
    }
    private int RetrieveCustomerIDFromSessionOrContext()
    {
        return UserSession.RetrieveCustomerIDFromPreferences();
    }

    private async void MakePayment_Clicked(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("Starting payment process...");
            bool success = await ProcessPaymentAsync();

            if (success)
            {
                Console.WriteLine("Payment successful. Submitting order...");

                if (orderHistoryViewModel == null)
                {
                    Console.WriteLine("orderHistoryViewModel is null");
                    await DisplayAlert("Error", "Order history view model is not initialized.", "OK");
                    return;
                }
                int customerID = RetrieveCustomerIDFromSessionOrContext();
                    

                decimal totalAmount = CartManager.Instance.CartItems.Sum(item => item.PriceDecimal * item.Quantity);

                if (order == null)
                {
                    order = new Order
                    {
                        CustomerID = customerID,
                        OrderStatus = "Pending",
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount
                    };
                }
   
                    bool isOrderSubmitted = await orderHistoryViewModel.SubmitOrderAsync(order);

                    if (isOrderSubmitted)
                    {
                        await DisplayAlert("Success", "Your order has been submitted successfully.", "OK");
                    await Navigation.PushAsync(new ShoppingPage());
                    {

                    }
                    }
                    else
                    {
                        await DisplayAlert("Error", "An unexpected error occurred", "OK");
                    }
                
            }
            else
            {
                await DisplayAlert("Payment Failed", "There was an issue processing your payment. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            Console.WriteLine(ex);
        }
    }
    private int GetNextOrderID()
    {
        return 123;
    }
    private async Task<bool> ProcessPaymentAsync()
    {
        await Task.Delay(2000); 
        return true; 
    }
}
