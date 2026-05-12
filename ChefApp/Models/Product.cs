namespace ChefApp.Models
{
    // Продукт на складі
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }       // Найменування
        public string Unit { get; set; }       // Одиниця виміру (кг, л, шт)
        public decimal Price { get; set; }     // Ціна за одиницю
        public double Quantity { get; set; }   // Кількість на складі
        public DateTime ExpiryDate { get; set; } // Термін придатності

        // Чи не прострочений продукт
        public bool IsExpired => ExpiryDate < DateTime.Today;

        // Чи закінчується термін (менше 3 днів)
        public bool IsExpiringSoon => !IsExpired && ExpiryDate <= DateTime.Today.AddDays(3);

        public override string ToString() => Name;
    }
}
