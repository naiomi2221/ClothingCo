using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ClothingCo;

public static class DatabaseHelper
{
    public static readonly string connectionString = "Data Source=NAIOMI\\SQLEXPRESS;Initial Catalog=SHOPPINGMART;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

    public static string GetConnectionString()
    {
        return connectionString;
    }

    public static async Task<bool> SubmitToDatabase(AppUser appUser)
        //hello
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Customers (Name, Address , City, State, Zip, Phone, " +
                    "Email, SecurityQuestionID, SecurityAnswer, Password) VALUES " +
                    "(@Name, @Address, @City, @State, @Zip, @Phone, @Email, @SecurityQuestionID, @SecurityAnswer, @Password)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", appUser.Name);
                command.Parameters.AddWithValue("@Address", appUser.Address);
                command.Parameters.AddWithValue("@City", appUser.City);
                command.Parameters.AddWithValue("@State", appUser.State);
                command.Parameters.AddWithValue("@Zip", appUser.Zip);
                command.Parameters.AddWithValue("@Phone", appUser.Phone);
                command.Parameters.AddWithValue("@Email", appUser.Email);
                command.Parameters.AddWithValue("@SecurityQuestionID", appUser.SecurityQuestionID);
                command.Parameters.AddWithValue("@SecurityAnswer", appUser.SecurityAnswer);
                command.Parameters.AddWithValue("@Password", appUser.Password);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user: {ex.Message}");
            return false;
        }
    }
    public static async Task<bool> VerifyPassword(string enteredPassword, string storedPassword)
    {
        return enteredPassword == storedPassword;  
    }

    public static async Task<int> ValidateUserAsync(string email, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT CustomerId, Password FROM Customers WHERE Email = @Email";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader.GetString(reader.GetOrdinal("Password"));

                        if (await VerifyPassword(password, storedPassword))
                        {
                            return reader.GetInt32(reader.GetOrdinal("CustomerId"));
                        }
                    }
                }
            }
        }

        return 0;
    }
}

