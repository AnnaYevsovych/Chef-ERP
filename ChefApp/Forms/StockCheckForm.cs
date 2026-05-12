using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class StockCheckForm : Form
    {
        private readonly DailyMenu _menu;
        private readonly RecipeService _recipeService;
        private readonly ProductService _productService;
        private readonly InvoiceService _invoiceService = new InvoiceService();

        private DataGridView dgvCheck;
        private Button btnCreateInvoice, btnClose;
        private Label lblSummary;

        // Розраховані потреби: productId -> (назва, одиниця, потрібно, є на складі)
        private List<StockCheckLine> _lines = new();

        public StockCheckForm(DailyMenu menu, RecipeService recipeService, ProductService productService)
        {
            _menu = menu;
            _recipeService = recipeService;
            _productService = productService;
            InitializeComponent();
            Calculate();
        }

        private void InitializeComponent()
        {
            this.Text = $"Перевірка запасів — {_menu.Date:dd.MM.yyyy}, {_menu.Persons} осіб";
            this.Size = new Size(780, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(650, 400);

            lblSummary = new Label
            {
                Dock = DockStyle.Top, Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            dgvCheck = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f)
            };

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 46 };
            btnCreateInvoice = new Button
            {
                Text = "Сформувати накладну та списати зі складу",
                Width = 300, Height = 32, Left = 10, Top = 7,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCreateInvoice.FlatAppearance.BorderSize = 0;
            btnCreateInvoice.Click += (s, e) => CreateInvoice();

            btnClose = new Button { Text = "Закрити", Width = 100, Height = 32, Left = 320, Top = 7 };
            btnClose.Click += (s, e) => this.Close();

            panelBottom.Controls.AddRange(new Control[] { btnCreateInvoice, btnClose });

            this.Controls.Add(dgvCheck);
            this.Controls.Add(lblSummary);
            this.Controls.Add(panelBottom);
        }

        private void Calculate()
        {
            // Порахувати потребу по всіх стравах
            var needed = new Dictionary<int, (string Name, string Unit, double Amount)>();

            foreach (var item in _menu.Items)
            {
                var recipe = _recipeService.GetById(item.RecipeId);
                if (recipe == null) continue;

                foreach (var ing in recipe.Ingredients)
                {
                    double total = ing.AmountPerPerson * _menu.Persons;
                    if (needed.ContainsKey(ing.ProductId))
                    {
                        var ex = needed[ing.ProductId];
                        needed[ing.ProductId] = (ex.Name, ex.Unit, ex.Amount + total);
                    }
                    else
                    {
                        needed[ing.ProductId] = (ing.ProductName, ing.Unit, total);
                    }
                }
            }

            // Порівняти з залишками
            _lines.Clear();
            foreach (var kv in needed)
            {
                var product = _productService.GetById(kv.Key);
                double inStock = product?.Quantity ?? 0;
                decimal price = product?.Price ?? 0;

                _lines.Add(new StockCheckLine
                {
                    ProductId   = kv.Key,
                    ProductName = kv.Value.Name,
                    Unit        = kv.Value.Unit,
                    Needed      = kv.Value.Amount,
                    InStock     = inStock,
                    Price       = price,
                    Enough      = inStock >= kv.Value.Amount
                });
            }

            RefreshGrid();

            int shortCount = _lines.Count(l => !l.Enough);
            if (shortCount == 0)
                lblSummary.Text = $"Всі продукти в наявності. Можна формувати накладну.";
            else
                lblSummary.Text = $"Нестача по {shortCount} позиціям (виділено червоним).";

            lblSummary.ForeColor = shortCount == 0 ? Color.DarkGreen : Color.DarkRed;
            btnCreateInvoice.Enabled = shortCount == 0;
        }

        private void RefreshGrid()
        {
            dgvCheck.DataSource = null;
            dgvCheck.Columns.Clear();

            var source = _lines.Select(l => new
            {
                l.ProductId,
                Продукт    = l.ProductName,
                Одиниця    = l.Unit,
                Потрібно   = Math.Round(l.Needed, 3),
                НаСкладі   = Math.Round(l.InStock, 3),
                Різниця    = Math.Round(l.InStock - l.Needed, 3),
                Ціна       = l.Price,
                Сума       = Math.Round((decimal)l.Needed * l.Price, 2)
            }).ToList();

            dgvCheck.DataSource = source;

            if (dgvCheck.Columns.Contains("ProductId"))
                dgvCheck.Columns["ProductId"].Visible = false;

            // Підсвітити нестачу
            foreach (DataGridViewRow row in dgvCheck.Rows)
            {
                var productId = (int)row.Cells["ProductId"].Value;
                var line = _lines.FirstOrDefault(l => l.ProductId == productId);
                if (line != null && !line.Enough)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
            }
        }

        private void CreateInvoice()
        {
            if (MessageBox.Show(
                $"Сформувати видаткову накладну та списати продукти зі складу?\nЦю дію не можна скасувати.",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var invoice = new Invoice
            {
                Date    = DateTime.Now,
                MenuId  = _menu.Id,
                Persons = _menu.Persons,
                Notes   = $"Меню на {_menu.Date:dd.MM.yyyy}, {_menu.Persons} осіб",
                Lines   = _lines.Select(l => new InvoiceLine
                {
                    ProductId    = l.ProductId,
                    ProductName  = l.ProductName,
                    Unit         = l.Unit,
                    Quantity     = l.Needed,
                    PricePerUnit = l.Price
                }).ToList()
            };

            // Списати зі складу
            foreach (var line in _lines)
                _productService.Deduct(line.ProductId, line.Needed);

            // Зберегти накладну
            _invoiceService.Add(invoice);

            // Позначити меню як підтверджене
            _menu.IsConfirmed = true;

            MessageBox.Show(
                $"Накладну №{invoice.Id} сформовано!\nЗагальна сума: {invoice.TotalAmount:F2} грн",
                "Накладну створено", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    // Допоміжний клас для рядка перевірки
    public class StockCheckLine
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public double Needed { get; set; }
        public double InStock { get; set; }
        public decimal Price { get; set; }
        public bool Enough { get; set; }
    }
}
