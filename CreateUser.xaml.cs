using Microsoft.Maui.Controls;

namespace ClothingCo;

public partial class CreateUser : ContentPage
{
    public CreateUser()
    {
        InitializeComponent();

        var viewModel = new SecurityQuestionsViewModel();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var viewModel = (SecurityQuestionsViewModel)BindingContext;
        await viewModel.LoadSecurityQuestions();  
    }
    private async void Insert_Clicked(object sender, EventArgs e)
    {
        if (Password.Text != ConfirmPassword.Text)
        {
            PasswordMatchLabel.Text = "Passwords do not match.";
            PasswordMatchLabel.IsVisible = true;  
            return;
        }
        else
        {
            PasswordMatchLabel.IsVisible = false; 
        }

        if (string.IsNullOrWhiteSpace(Password.Text))
        {
            await DisplayAlert("Error", "Password is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Name.Text) ||
            string.IsNullOrWhiteSpace(Address.Text) ||
            string.IsNullOrWhiteSpace(City.Text) ||
            string.IsNullOrWhiteSpace(State.Text) ||
            string.IsNullOrWhiteSpace(Zip.Text) ||
            string.IsNullOrWhiteSpace(Phone.Text) ||
            string.IsNullOrWhiteSpace(emailEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }
        var viewModel = (SecurityQuestionsViewModel)BindingContext;
        var selectedQuestion = viewModel.SelectedQuestion;  

        if (selectedQuestion == null)
        {
            await DisplayAlert("Error", "Please select a security question.", "OK");
            return;
        }
        var appUser = new AppUser
        {
            Name = Name.Text,
            Address = Address.Text,
            City = City.Text,
            State = State.Text,
            Zip = Zip.Text,
            Phone = Phone.Text,
            Email = emailEntry.Text,
            SecurityQuestionID = selectedQuestion.SecurityQuestionID,
            SecurityAnswer = SecurityAnswer.Text,
            Password = Password.Text, 
        };
        try
        {
            bool success = await DatabaseHelper.SubmitToDatabase(appUser);

            if (success)
            {
                await DisplayAlert("Success", "Your information has been submitted.", "OK");
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                await DisplayAlert("Error", "There was an issue submitting your information. Please try again.", "OK");
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error: {ex.Message}"); 
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); 
        }
    }
}