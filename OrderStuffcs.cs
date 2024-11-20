using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingCo
{
    

    public class Customer
    {
        public int CustomerID { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public string Name { get; set; }
    }
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }

        public bool CanCancel => OrderStatus == "Pending";

        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
    public class OrderService
    {
        public async Task<List<Order>> GetOrderHistoryAsync(string email)
        {
            var orders = new List<Order>();
            string connectionString = DatabaseHelper.GetConnectionString();

            string query = @"
    SELECT Orders.OrderID, Orders.CustomerID, Orders.OrderDate, Orders.TotalAmount, Orders.OrderStatus
    FROM Orders
    INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID
    WHERE Customers.Email = @Email";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = new Order
                            {
                                OrderID = reader.GetInt32(0),
                                CustomerID = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2), // Ensure this maps correctly
                                TotalAmount = reader.GetDecimal(3),
                                OrderStatus = reader.GetString(4)
                            };
                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }


        public static async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            var orderItems = new List<OrderItem>();
            string connectionString = DatabaseHelper.GetConnectionString();
            string query = @"
                 SELECT oi. OrderItemID, oi.ProductID, oi.Quantity, oi.Price, p.ProductName
                 FROM OrderItems oi
                 INNER JOIN Products p ON oi.ProductID = p.ProductID";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);

                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var orderItem = new OrderItem
                        {
                            OrderItemID = reader.GetInt32(0),
                            ProductID = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2),
                            UnitPrice = reader.GetDecimal(3),
                            
                        };

                        orderItems.Add(orderItem);
                    }
                }
            }

            return orderItems;
        }

    }
}
