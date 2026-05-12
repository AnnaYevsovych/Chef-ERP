using System.Text.Json;

namespace ChefApp.Services
{
    // Універсальний сервіс збереження/завантаження JSON
    public static class JsonDataService
    {
        private static readonly string DataFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data");

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,           // Красиве форматування JSON
            PropertyNameCaseInsensitive = true
        };

        // Повертає шлях до файлу по імені
        private static string GetFilePath(string fileName)
        {
            Directory.CreateDirectory(DataFolder); // Створює папку якщо немає
            return Path.Combine(DataFolder, fileName);
        }

        // Завантажити список об'єктів з JSON файлу
        public static List<T> Load<T>(string fileName)
        {
            var path = GetFilePath(fileName);
            if (!File.Exists(path)) return new List<T>();

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        // Зберегти список об'єктів у JSON файл
        public static void Save<T>(string fileName, List<T> data)
        {
            var path = GetFilePath(fileName);
            var json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(path, json);
        }

        // Отримати наступний вільний ID для нового запису
        public static int GetNextId<T>(List<T> list, Func<T, int> idSelector)
        {
            if (list == null || list.Count == 0) return 1;
            return list.Max(idSelector) + 1;
        }
    }
}
