namespace ChefApp.Models
{
    // Позиція в меню на день
    public class MenuItem
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string Category { get; set; }
        public int Persons { get; set; }       // На скільки персон
        public DateTime MenuDate { get; set; } // Дата меню
    }

    // Меню на день
    public class DailyMenu
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Persons { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
        public bool IsConfirmed { get; set; } = false; // Чи підтверджено (накладну сформовано)
    }
}
