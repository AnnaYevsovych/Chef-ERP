using ChefApp.Models;

namespace ChefApp.Forms
{
    public class ProductEditForm : Form
    {
        public Product Result { get; private set; }
        private readonly Product _existing;

        private TextBox txtName, txtUnit;
        private NumericUpDown nudPrice, nudQuantity;
        private DateTimePicker dtpExpiry;
        private Button btnOk, btnCancel;

        public ProductEditForm(Product existing)
        {
            _existing = existing;
            InitializeComponent();

            if (_existing != null)
            {
                this.Text = "Редагувати продукт";
                txtName.Text = _existing.Name;
                txtUnit.Text = _existing.Unit;
                nudPrice.Value = _existing.Price;
                nudQuantity.Value = (decimal)_existing.Quantity;
                dtpExpiry.Value = _existing.ExpiryDate == default ? DateTime.Today.AddMonths(1) : _existing.ExpiryDate;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Новий продукт";
            this.Size = new Size(360, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int labelW = 110, ctrlX = 130, ctrlW = 190, rowH = 36, startY = 20;

            // Назва
            AddLabel("Назва:", ctrlX, startY);
            txtName = new TextBox { Left = ctrlX, Top = startY, Width = ctrlW };
            startY += rowH;

            // Одиниця виміру
            AddLabel("Одиниця:", ctrlX, startY);
            txtUnit = new TextBox { Left = ctrlX, Top = startY, Width = ctrlW };
            txtUnit.Text = "кг";
            startY += rowH;

            // Ціна
            AddLabel("Ціна:", ctrlX, startY);
            nudPrice = new NumericUpDown
            {
                Left = ctrlX, Top = startY, Width = ctrlW,
                DecimalPlaces = 2, Minimum = 0, Maximum = 999999, Increment = 1
            };
            startY += rowH;

            // Кількість
            AddLabel("Кількість:", ctrlX, startY);
            nudQuantity = new NumericUpDown
            {
                Left = ctrlX, Top = startY, Width = ctrlW,
                DecimalPlaces = 3, Minimum = 0, Maximum = 999999, Increment = 0.5m
            };
            startY += rowH;

            // Термін придатності
            AddLabel("Термін придатності:", ctrlX, startY);
            dtpExpiry = new DateTimePicker
            {
                Left = ctrlX, Top = startY, Width = ctrlW,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(1)
            };
            startY += rowH + 10;

            // Кнопки
            btnOk = new Button { Text = "Зберегти", Width = 100, Height = 30, Left = ctrlX, Top = startY, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Скасувати", Width = 100, Height = 30, Left = ctrlX + 110, Top = startY, DialogResult = DialogResult.Cancel };

            btnOk.Click += BtnOk_Click;

            this.Controls.AddRange(new Control[]
            {
                txtName, txtUnit, nudPrice, nudQuantity, dtpExpiry, btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void AddLabel(string text, int ctrlX, int top)
        {
            var lbl = new Label
            {
                Text = text,
                Left = 12,
                Top = top + 3,
                Width = ctrlX - 16,
                AutoSize = false
            };
            this.Controls.Add(lbl);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введіть назву продукту.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUnit.Text))
            {
                MessageBox.Show("Введіть одиницю виміру.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Result = new Product
            {
                Id = _existing?.Id ?? 0,
                Name = txtName.Text.Trim(),
                Unit = txtUnit.Text.Trim(),
                Price = nudPrice.Value,
                Quantity = (double)nudQuantity.Value,
                ExpiryDate = dtpExpiry.Value.Date
            };
        }
    }
}
