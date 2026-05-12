using ChefApp.Models;

namespace ChefApp.Services
{
    public class MenuService
    {
        private const string FileName = "menu.json";
        private List<DailyMenu> _menus;

        public MenuService()
        {
            _menus = JsonDataService.Load<DailyMenu>(FileName);
        }

        public List<DailyMenu> GetAll() => _menus;

        public DailyMenu GetById(int id) => _menus.FirstOrDefault(m => m.Id == id);

        public DailyMenu GetByDate(DateTime date) =>
            _menus.FirstOrDefault(m => m.Date.Date == date.Date);

        public void Add(DailyMenu menu)
        {
            menu.Id = JsonDataService.GetNextId(_menus, m => m.Id);
            _menus.Add(menu);
            Save();
        }

        public void Update(DailyMenu menu)
        {
            var index = _menus.FindIndex(m => m.Id == menu.Id);
            if (index >= 0)
            {
                _menus[index] = menu;
                Save();
            }
        }

        public void Delete(int id)
        {
            _menus.RemoveAll(m => m.Id == id);
            Save();
        }

        private void Save() => JsonDataService.Save(FileName, _menus);
    }
}
