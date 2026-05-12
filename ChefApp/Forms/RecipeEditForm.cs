using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class RecipeEditForm : Form
    {
        public Recipe Result { get; private set; }

        private readonly Recipe _existing;
        private readonly ProductService _productService;
        private readonly List<RecipeIngredient> _ingredients = new();

        private TextBox txtName, txtDescription;
        private ComboBox cmbCategory;
        private NumericUpDown nudTime;
        private DataGridView dgvIngredients;
        private Button btnAddIngredient, btnRemoveIngredient, btnOk, btnCancel;

        private static readonly string[] Categories =
            { "Перша страва", "Друга страва", "Салат", "Гарнір", "Десерт", "Напій", "Закуска" };

        public RecipeEditForm(Recipe existing, ProductService productService)
        {
            _existing = existing;
            _productService = productService;
            InitializeComponent();

            if (_existing != null)
            {
                this.Text = "Редагувати рецепт";
                txtName.Text = _existing.Name;
                cmbCategory.SelectedItem = _existing.Category;
                nudTime.Value = _existing.CookingTimeMinutes;
                txtDescription.Text = _existing.Description;
                _ingredients.AddRange(_existing.Ingredients);
                RefreshIngredientsGrid();
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Новий рецепт";
            this.Size = new Size(680, 560);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(580, 480);
            this.StartPosition = FormStartPosition.CenterParent;

            // --- Верхня панель з полями ---
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 160, Padding = new Padding(12) };

            int lw = 120, cx = 135, cw = 220, row = 0, rh = 34;

            Lbl(panelTop, "Назва страви:", 12, row * rh + 16);
            txtName = new TextBox { Left = cx, Top = row * rh + 12, Width = 490 };
            row++;

            Lbl(panelTop, "Категорія:", 12, row * rh + 16);
            cmbCategory = new ComboBox { Left = cx, Top = row * rh + 12, Width = cw, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory.Items.AddRange(Categories);
            cmbCategory.SelectedIndex = 0;
            row++;

            Lbl(panelTop, "Час (хвилин):", 12, row * rh + 16);
            nudTime = new NumericUpDown { Left = cx, Top = row * rh + 12, Width = 100, Minimum = 1, Maximum = 600, Value = 30 };
            row++;

            Lbl(panelTop, "Опис / рецепт:", 12, row * rh + 16);
            txtDescription = new TextBox
            {
                Left = cx, Top = row * rh + 12, Width = 490, Height = 50,
                Multiline = true, ScrollBars = ScrollBars.Vertical
            };

            panelTop.Controls.AddRange(new Control[] { txtName, cmbCategory, nudTime, txtDescription });

            // --- Панель інгредієнтів ---
            var lblIngr = new Label
            {
                Text = "Розкладка (інгредієнти на 1 особу):",
                Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(12, 4, 0, 0)
            };

            dgvIngredients = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f)
            };

            // --- Панель кнопок інгредієнтів ---
            var panelIngr = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            btnAddIngredient    = new Button { Text = "Додати інгредієнт", Width = 160, Height = 30, Left = 12, Top = 5 };
            btnRemoveIngredient = new Button { Text = "Видалити", Width = 100, Height = 30, Left = 182, Top = 5 };
            btnAddIngredient.Click    += (s, e) => AddIngredient();
            btnRemoveIngredient.Click += (s, e) => RemoveIngredient();
            panelIngr.Controls.AddRange(new Control[] { btnAddIngredient, btnRemoveIngredient });

            // --- Кнопки OK / Cancel ---
            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 46 };
            btnOk     = new Button { Text = "Зберегти", Width = 110, Height = 32, Left = 12, Top = 7, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Скасувати", Width = 110, Height = 32, Left = 132, Top = 7, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;
            panelBottom.Controls.AddRange(new Control[] { btnOk, btnCancel });

            // --- Зібрати layout ---
            var panelCenter = new Panel { Dock = DockStyle.Fill };
            panelCenter.Controls.Add(dgvIngredients);
            panelCenter.Controls.Add(lblIngr);
            panelCenter.Controls.Add(panelIngr);

            this.Controls.Add(panelCenter);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelBottom);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void Lbl(Panel p, string text, int x, int y)
        {
            p.Controls.Add(new Label { Text = text, Left = x, Top = y, AutoSize = true });
        }

        private void RefreshIngredientsGrid()
        {
            dgvIngredients.DataSource = null;
            dgvIngredients.Columns.Clear();

            var source = _ingredients.Select((ing, idx) => new
            {
                Index = idx,
                Продукт = ing.ProductName,
                Кількість = ing.AmountPerPerson,
                Одиниця = ing.Unit
            }).ToList();

            dgvIngredients.DataSource = source;
            if (dgvIngredients.Columns.Contains("Index"))
                dgvIngredients.Columns["Index"].Visible = false;
        }

        private void AddIngredient()
        {
            var products = _productService.GetAll();
            if (products.Count == 0)
            {
                MessageBox.Show("Спочатку додайте продукти на склад.", "Немає продуктів",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var form = new IngredientEditForm(products);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ingredients.Add(form.Result);
                RefreshIngredientsGrid();
            }
        }

        private void RemoveIngredient()
        {
            if (dgvIngredients.CurrentRow == null) return;
            var idx = (int)dgvIngredients.CurrentRow.Cells["Index"].Value;
            _ingredients.RemoveAt(idx);
            RefreshIngredientsGrid();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введіть назву страви.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Result = new Recipe
            {
                Id = _existing?.Id ?? 0,
                Name = txtName.Text.Trim(),
                Category = cmbCategory.SelectedItem?.ToString() ?? "Друга страва",
                CookingTimeMinutes = (int)nudTime.Value,
                Description = txtDescription.Text.Trim(),
                Ingredients = new List<RecipeIngredient>(_ingredients)
            };
        }
    }
}
