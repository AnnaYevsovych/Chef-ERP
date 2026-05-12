using ChefApp.Models;

namespace ChefApp.Forms
{
    public class IngredientEditForm : Form
    {
        public RecipeIngredient Result { get; private set; }

        private readonly List<Product> _products;
        private ComboBox cmbProduct;
        private NumericUpDown nudAmount;
        private Label lblUnit;
        private Button btnOk, btnCancel;

        public IngredientEditForm(List<Product> products)
        {
            _products = products;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Додати інгредієнт";
            this.Size = new Size(340, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int cx = 130, cw = 170, rh = 36, y = 20;

            // Продукт
            AddLabel("Продукт:", y);
            cmbProduct = new ComboBox
            {
                Left = cx, Top = y, Width = cw,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var p in _products)
                cmbProduct.Items.Add(p);
            cmbProduct.SelectedIndex = 0;
            cmbProduct.SelectedIndexChanged += (s, e) => UpdateUnit();
            y += rh;

            // Кількість на 1 особу
            AddLabel("На 1 особу:", y);
            nudAmount = new NumericUpDown
            {
                Left = cx, Top = y, Width = 100,
                DecimalPlaces = 3, Minimum = 0.001m, Maximum = 9999, Value = 0.1m, Increment = 0.05m
            };
            lblUnit = new Label
            {
                Left = cx + 108, Top = y + 3,
                AutoSize = true,
                ForeColor = Color.Gray
            };
            y += rh;

            // Кнопки
            btnOk = new Button
            {
                Text = "Додати", Width = 100, Height = 30,
                Left = cx, Top = y + 8, DialogResult = DialogResult.OK
            };
            btnCancel = new Button
            {
                Text = "Скасувати", Width = 100, Height = 30,
                Left = cx + 110, Top = y + 8, DialogResult = DialogResult.Cancel
            };
            btnOk.Click += BtnOk_Click;

            this.Controls.AddRange(new Control[] { cmbProduct, nudAmount, lblUnit, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            UpdateUnit();
        }

        private void AddLabel(string text, int y)
        {
            this.Controls.Add(new Label { Text = text, Left = 12, Top = y + 3, AutoSize = true });
        }

        private void UpdateUnit()
        {
            if (cmbProduct.SelectedItem is Product p)
                lblUnit.Text = p.Unit;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem is not Product product)
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            Result = new RecipeIngredient
            {
                ProductId = product.Id,
                ProductName = product.Name,
                AmountPerPerson = (double)nudAmount.Value,
                Unit = product.Unit
            };
        }
    }
}
