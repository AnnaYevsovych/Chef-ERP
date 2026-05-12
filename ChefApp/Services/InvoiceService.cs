using ChefApp.Models;

namespace ChefApp.Services
{
    public class InvoiceService
    {
        private const string FileName = "invoices.json";
        private List<Invoice> _invoices;

        public InvoiceService()
        {
            _invoices = JsonDataService.Load<Invoice>(FileName);
        }

        public List<Invoice> GetAll() => _invoices;

        public Invoice GetById(int id) => _invoices.FirstOrDefault(i => i.Id == id);

        public void Add(Invoice invoice)
        {
            invoice.Id = JsonDataService.GetNextId(_invoices, i => i.Id);
            _invoices.Add(invoice);
            Save();
        }

        public void Delete(int id)
        {
            _invoices.RemoveAll(i => i.Id == id);
            Save();
        }

        private void Save() => JsonDataService.Save(FileName, _invoices);
    }
}
