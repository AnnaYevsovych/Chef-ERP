using ChefApp.Models;

namespace ChefApp.Services
{
    public class ProductService
    {
        private const string FileName = "products.json";
        private List<Product> _products;

        public ProductService()
        {
            _products = JsonDataService.Load<Product>(FileName);
        }

        public List<Product> GetAll() => _products;

        public Product GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

        public void Add(Product product)
        {
            product.Id = JsonDataService.GetNextId(_products, p => p.Id);
            _products.Add(product);
            Save();
        }

        public void Update(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index >= 0)
            {
                _products[index] = product;
                Save();
            }
        }

        public void Delete(int id)
        {
            _products.RemoveAll(p => p.Id == id);
            Save();
        }

        // Зменшити кількість продукту на складі
        public bool Deduct(int productId, double amount)
        {
            var product = GetById(productId);
            if (product == null || product.Quantity < amount) return false;
            product.Quantity -= amount;
            Save();
            return true;
        }

        // Продукти з простроченим терміном
        public List<Product> GetExpired() => _products.Where(p => p.IsExpired).ToList();

        // Продукти зі строком, що спливає
        public List<Product> GetExpiringSoon() => _products.Where(p => p.IsExpiringSoon).ToList();

        private void Save() => JsonDataService.Save(FileName, _products);
    }
}
