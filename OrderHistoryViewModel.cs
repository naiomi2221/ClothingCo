using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingCo
{
    public class OrderHistoryViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;
        private ObservableCollection<Order> _orders;
        private bool _hasOrders;

        public ObservableCollection<Order> Orders { get; set; }

        public bool HasOrders
        {
            get => _hasOrders;
            set
            {
                _hasOrders = value;
                OnPropertyChanged(nameof(HasOrders));
            }
        }

        public OrderHistoryViewModel(OrderService orderService)
        {
            _orderService = orderService ?? new OrderService();
            Orders = new ObservableCollection<Order>();
        }

        public async Task LoadOrderHistory(string email)
        {
            try
            {
                var orders = await _orderService.GetOrderHistoryAsync(email);
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order history: {ex.Message}");
            }
        }

        public async Task<bool> SubmitOrderAsync(Order order)
        {
            try
            {
                string connectionString = DatabaseHelper.GetConnectionString();

                string orderQuery = @"
                    INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, OrderStatus)
                    VALUES (@CustomerID, @OrderDate, @TotalAmount, @OrderStatus);";

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(orderQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        command.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while submitting the order: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                string query = "UPDATE Orders SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";

                using (var connection = new SqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderStatus", newStatus);
                        command.Parameters.AddWithValue("@OrderID", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return false;
            }
        }
        public async Task LoadOrderHistoryAsync(string email)
        {
            try
            {
                var orders = await _orderService.GetOrderHistoryAsync(email);
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }

                HasOrders = Orders.Count > 0;
                Console.WriteLine($"Loaded {Orders.Count} orders for email: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order history: {ex.Message}");
            }
        }
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            return await UpdateOrderStatusAsync(orderId, "Canceled");
        }
        public Action<string, string, string> DisplayAlertAction { get; set; }
        public async Task CancelOrder(int orderId)
        {
            var result = await CancelOrderAsync(orderId);
            if (result)
            {
                var order = Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order != null)
                {
                    Orders.Remove(order);
                    order.OrderStatus = "Canceled";
                    DisplayAlertAction?.Invoke("Success", "Order was successfully cancelled.", "OK");
                }
            }
            else
            {
                Console.WriteLine("Failed to cancel the order.");
            }
        }
        public event Action<int> OnOrderCanceled;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
