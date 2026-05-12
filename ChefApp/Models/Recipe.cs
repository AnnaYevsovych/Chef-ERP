namespace ChefApp.Models
{
    // Інгредієнт у рецепті (розкладка)
    public class RecipeIngredient
    {
        public int ProductId { get; set; }      // Посилання на продукт зі складу
        public string ProductName { get; set; } // Назва (для зручності відображення)
        public double AmountPerPerson { get; set; } // Кількість на 1 персону
        public string Unit { get; set; }        // Одиниця виміру
    }

    // Рецепт страви
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }           // Назва страви
        public string Category { get; set; }       // Категорія: Перша, Друга, Салат, Десерт...
        public string Description { get; set; }    // Опис / спосіб приготування
        public int CookingTimeMinutes { get; set; } // Час приготування (хвилини)
        public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        public override string ToString() => $"{Name} ({Category})";
    }
}
