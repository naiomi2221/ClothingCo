using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class SecurityQuestionsViewModel : INotifyPropertyChanged
{
    private const string connectionString = "Data Source=NAIOMI\\SQLEXPRESS;Initial Catalog=SHOPPINGMART;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

    private ObservableCollection<SecurityQuestion> securityQuestions = new ObservableCollection<SecurityQuestion>();
    private SecurityQuestion selectedQuestion;

    public ObservableCollection<SecurityQuestion> SecurityQuestions
    {
        get => securityQuestions;
        set => SetProperty(ref securityQuestions, value);
    }

    public SecurityQuestion SelectedQuestion
    {
        get => selectedQuestion;
        set => SetProperty(ref selectedQuestion, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Constructor to initialize any commands or default values
    public SecurityQuestionsViewModel()
    {
        // Initialize if needed
    }

    // Method to load security questions from the database
    public async Task LoadSecurityQuestions()
    {
        var questions = await GetQuestionsFromDatabaseAsync();
        if (questions != null && questions.Count > 0)
        {
            SecurityQuestions = new ObservableCollection<SecurityQuestion>(questions);
        }
        else
        {
            Console.WriteLine("No security questions found.");
        }
    }

    // Fetch questions from the database
    private async Task<List<SecurityQuestion>> GetQuestionsFromDatabaseAsync()
    {
        var questions = new List<SecurityQuestion>();

        try
        {
            string query = "SELECT SecurityQuestionID, Question FROM SecurityQuestions";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        questions.Add(new SecurityQuestion
                        {
                            SecurityQuestionID = reader.GetInt32(0),
                            Question = reader.GetString(1)
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching security questions: {ex.Message}");
        }

        return questions;
    }
    public class SecurityQuestion
    {
        public int SecurityQuestionID { get; set; }
        public string Question { get; set; }
    }
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
