using ClothingCo;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;

namespace ClothingCo
{
    public partial class ResetPassword : ContentPage
    {
        private readonly ResetPasswordViewModel _viewModel;

        public ResetPassword(string email)
        {
            InitializeComponent();
            _viewModel = new ResetPasswordViewModel();
            BindingContext = _viewModel;

            if (!string.IsNullOrEmpty(email))
            {
                _viewModel.LoadSecurityQuestions(email);
            }
            else
            {
                DisplayAlert("Error", "Email cannot be empty", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void GetQuestion_Clicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    // Await the LoadSecurityQuestions method
                    await _viewModel.LoadSecurityQuestions(email);

                    if (!string.IsNullOrEmpty(_viewModel.QuestionText))
                    {
                        SecurityQuestionLabel.Text = _viewModel.QuestionText;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No security question found for this email.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter your email", "OK");
            }
        }


        private async void Submit_Clicked(object sender, EventArgs e)
        {
            _viewModel.SecurityAnswer = SecurityAnswerEntry.Text;
            _viewModel.NewPassword = NewPasswordEntry.Text;
            _viewModel.ConfirmPassword = ConfirmPasswordEntry.Text;

            // Debugging output
            Console.WriteLine($"SecurityAnswer: {_viewModel.SecurityAnswer}");
            Console.WriteLine($"NewPassword: {_viewModel.NewPassword}");
            Console.WriteLine($"ConfirmPassword: {_viewModel.ConfirmPassword}");

            if (string.IsNullOrWhiteSpace(_viewModel.SecurityAnswer) ||
                string.IsNullOrWhiteSpace(_viewModel.NewPassword) ||
                string.IsNullOrWhiteSpace(_viewModel.ConfirmPassword))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (_viewModel.NewPassword != _viewModel.ConfirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            bool isVerified = await _viewModel.VerifySecurityQuestionAndAnswerAsync();
            if (!isVerified)
            {
                await DisplayAlert("Error", "Invalid security answer", "OK");
                return;
            }

            bool isPasswordUpdated = await _viewModel.UpdatePasswordAsync();
            if (isPasswordUpdated)
            {
                await DisplayAlert("Success", "Your password has been reset", "OK");
                await Navigation.PushAsync(new MainPage()); 
            }
            else
            {
                await DisplayAlert("Error", "There was an error resetting your password", "OK");
            }
        }
    }
}

