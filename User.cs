using Microsoft.Maui.Storage;

namespace ClothingCo
{
    public static class UserSession
    {
        public static AppUser Current { get; set; }

        public static void SetCustomerID(int customerId)
        {
            Preferences.Set("CustomerID", customerId);
        }

        public static int RetrieveCustomerIDFromPreferences()
        {
            if (Preferences.ContainsKey("CustomerID"))
            {
                return Preferences.Get("CustomerID", 0);
            }
            return 0; 
        }
    }

    public class AppUser
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int SecurityQuestionID { get; set; }
        public string SecurityAnswer { get; set; }
        public string Password { get; set; }
    }
}
