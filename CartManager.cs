using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingCo
{
    public class CartManager
    {
        private static CartManager _instance;

        public static CartManager Instance => _instance ??= new CartManager();

        public ObservableCollection<CartItem> CartItems { get; private set; }
        private CartManager()
        {
            CartItems = new ObservableCollection<CartItem>();
        }
        public decimal CalculateTotalAmount()
        {
            return CartItems.Sum(item => item.PriceDecimal * item.Quantity);
        }
        public class CartItem
        {
            public string Name { get; set; }

            public decimal PriceDecimal { get; set; }

            public int Quantity { get; set; }
        }
    }

}
