namespace CyclingTourismAndFishing.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // Зв’язок із Item
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Quantity { get; set; }

        // Можна додати зв’язок із користувачем
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
