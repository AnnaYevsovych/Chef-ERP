namespace ChefApp.Models
{
    // Рядок видаткової накладної
    public class InvoiceLine
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }       // Кількість до списання
        public decimal PricePerUnit { get; set; }  // Ціна на момент списання
        public decimal TotalPrice => (decimal)Quantity * PricePerUnit;
    }

    // Видаткова накладна на склад
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MenuId { get; set; }            // До якого меню прив'язана
        public int Persons { get; set; }
        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
        public string Notes { get; set; }

        // Загальна сума накладної
        public decimal TotalAmount => Lines.Sum(l => l.TotalPrice);
    }
}
