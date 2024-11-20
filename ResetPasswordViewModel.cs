using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ClothingCo;

namespace ClothingCo
{
#nullable enable
    public class ResetPasswordViewModel : INotifyPropertyChanged
    {
        public readonly string connectionString = "Data Source=NAIOMI\\SQLEXPRESS;Initial Catalog=SHOPPINGMART;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public ObservableCollection<SecurityQuestion> SecurityQuestions { get; set; }
        public SecurityQuestion SelectedSecurityQuestion { get; set; }

        public string Email { get; set; }
        public string SecurityAnswer { get; set; }

        public int QuestionID { get; set; }
        public string QuestionText { get; set; }

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand ResetPasswordCommand { get; }

        public class SecurityQuestion
        {
            public int SecurityQuestionID { get; set; }
            public string Question { get; set; }
        }

        public ResetPasswordViewModel()
        {
            ResetPasswordCommand = new Command(async () => await ResetPassword());
            SecurityQuestions = new ObservableCollection<SecurityQuestion>
            {
                new SecurityQuestion { SecurityQuestionID = 1, Question = "What is your mother's maiden name?" },
                new SecurityQuestion { SecurityQuestionID = 2, Question = "What was the name of your first pet?" },
                new SecurityQuestion { SecurityQuestionID = 3, Question = "What was the name of your elementary school?" }
            };
        }

        public async Task LoadSecurityQuestions(string email)
        {
            Email = email;
            await GetQuestionsFromDatabaseAsync();
        }

        public async Task GetQuestionsFromDatabaseAsync()
        {
            try
            {
                string query = @"SELECT SecurityQuestionID 
                                FROM Customers 
                                WHERE Email = @Email";

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);

                        var reader = await command.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            QuestionID = reader.GetInt32(0);
                            OnPropertyChanged(nameof(QuestionID));

                            var question = SecurityQuestions.FirstOrDefault(q => q.SecurityQuestionID == QuestionID);
                            if (question != null)
                            {
                                QuestionText = question.Question;
                                OnPropertyChanged(nameof(QuestionText));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No security question found for this email.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching security question: {ex.Message}");
            }
        }

        public async Task<bool> VerifySecurityQuestionAndAnswerAsync()
        {
            string query = "SELECT SecurityAnswer FROM Customers WHERE Email = @Email AND SecurityQuestionID = @SecurityQuestionID";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@SecurityQuestionID", QuestionID);

                    var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        string answer = reader.GetString(0);
                        if (answer.Equals(SecurityAnswer, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdatePasswordAsync()
        {
            string query = "UPDATE Customers SET Password = @Password WHERE Email = @Email";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Password", NewPassword);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task ResetPassword()
        {
            if (NewPassword != ConfirmPassword)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            bool isVerified = await VerifySecurityQuestionAndAnswerAsync();
            if (isVerified)
            {
                bool isUpdated = await UpdatePasswordAsync();
                if (isUpdated)
                {
                    Console.WriteLine("Password updated successfully.");
                }
                else
                {
                    Console.WriteLine("Password update failed.");
                }
            }
            else
            {
                Console.WriteLine("Security question and answer do not match.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
