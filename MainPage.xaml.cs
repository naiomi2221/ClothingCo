using System;
using System.Diagnostics;
using ClothingCo;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace ClothingCo
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            string email = usernameTxt.Text;
            string password = passwordTxt.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            try
            {
                var customerId = await DatabaseHelper.ValidateUserAsync(email, password);
                if (customerId > 0)
                {
                    UserSession.Current = new AppUser { CustomerId = customerId };
                    UserSession.SetCustomerID(customerId);
                    await Navigation.PushAsync(new ShoppingPage());
                }
                else
                {
                    await DisplayAlert("Error", "Invalid username or password.", "OK");
                }
            }
            catch (SqlException sqlEx)
            {

                await DisplayAlert("Database Error", $"An error occurred while accessing the database: {sqlEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        public async void ResetClicked(object sender, EventArgs e)
        {
            string email = usernameTxt.Text;
            await Navigation.PushAsync(new ResetPassword(email));
        }

        private async void CreateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateUser());
        }
    }
}
            
         

          
            
        


