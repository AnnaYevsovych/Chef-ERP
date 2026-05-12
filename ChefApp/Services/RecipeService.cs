using ChefApp.Models;

namespace ChefApp.Services
{
    public class RecipeService
    {
        private const string FileName = "recipes.json";
        private List<Recipe> _recipes;

        public RecipeService()
        {
            _recipes = JsonDataService.Load<Recipe>(FileName);
        }

        public List<Recipe> GetAll() => _recipes;

        public Recipe GetById(int id) => _recipes.FirstOrDefault(r => r.Id == id);

        public List<Recipe> GetByCategory(string category) =>
            _recipes.Where(r => r.Category == category).ToList();

        public void Add(Recipe recipe)
        {
            recipe.Id = JsonDataService.GetNextId(_recipes, r => r.Id);
            _recipes.Add(recipe);
            Save();
        }

        public void Update(Recipe recipe)
        {
            var index = _recipes.FindIndex(r => r.Id == recipe.Id);
            if (index >= 0)
            {
                _recipes[index] = recipe;
                Save();
            }
        }

        public void Delete(int id)
        {
            _recipes.RemoveAll(r => r.Id == id);
            Save();
        }

        public List<string> GetCategories() =>
            _recipes.Select(r => r.Category).Distinct().OrderBy(c => c).ToList();

        private void Save() => JsonDataService.Save(FileName, _recipes);
    }
}
