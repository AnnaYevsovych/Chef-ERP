using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class RecipeForm : Form
    {
        private readonly RecipeService _recipeService = new RecipeService();
        private readonly ProductService _productService = new ProductService();

        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete;
        private ComboBox cmbCategory;
        private Label lblFilter;

        public RecipeForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "База рецептур";
            this.Size = new Size(860, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(700, 400);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 44 };
            lblFilter = new Label { Text = "Категорія:", Left = 10, Top = 13, AutoSize = true };
            cmbCategory = new ComboBox
            {
                Left = 90,
                Top = 9,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.SelectedIndexChanged += OnCategoryChanged;

            panelTop.Controls.AddRange(new Control[] { lblFilter, cmbCategory });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f)
            };
            dgv.DoubleClick += (s, e) => EditSelected();

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnAdd = new Button { Text = "Додати рецепт", Width = 130, Height = 32, Left = 10, Top = 9 };
            btnEdit = new Button { Text = "Редагувати", Width = 110, Height = 32, Left = 150, Top = 9 };
            btnDelete = new Button { Text = "Видалити", Width = 100, Height = 32, Left = 270, Top = 9 };

            btnAdd.Click += (s, e) => AddRecipe();
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();

            panelBottom.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            this.Controls.Add(dgv);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelBottom);
        }

        private void OnCategoryChanged(object sender, EventArgs e) => LoadData();

        private void LoadData()
        {
            // Відписатись на час оновлення щоб не було рекурсії
            cmbCategory.SelectedIndexChanged -= OnCategoryChanged;

            var selected = cmbCategory.SelectedItem?.ToString();

            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Всі категорії");
            foreach (var cat in _recipeService.GetCategories())
                cmbCategory.Items.Add(cat);

            if (selected != null && cmbCategory.Items.Contains(selected))
                cmbCategory.SelectedItem = selected;
            else
                cmbCategory.SelectedIndex = 0;

            // Підписатись знову
            cmbCategory.SelectedIndexChanged += OnCategoryChanged;

            var recipes = _recipeService.GetAll();
            if (cmbCategory.SelectedIndex > 0)
                recipes = recipes.Where(r => r.Category == cmbCategory.SelectedItem.ToString()).ToList();

            dgv.DataSource = null;
            dgv.Columns.Clear();

            var source = recipes.Select(r => new
            {
                r.Id,
                Назва = r.Name,
                Категорія = r.Category,
                Час = r.CookingTimeMinutes + " хв",
                Інгредієнтів = r.Ingredients.Count
            }).ToList();

            dgv.DataSource = source;

            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;
        }

        private void AddRecipe()
        {
            using var form = new RecipeEditForm(null, _productService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _recipeService.Add(form.Result);
                LoadData();
            }
        }

        private void EditSelected()
        {
            if (dgv.CurrentRow == null) return;
            var id = (int)dgv.CurrentRow.Cells["Id"].Value;
            var recipe = _recipeService.GetById(id);
            if (recipe == null) return;

            using var form = new RecipeEditForm(recipe, _productService);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _recipeService.Update(form.Result);
                LoadData();
            }
        }

        private void DeleteSelected()
        {
            if (dgv.CurrentRow == null) return;
            var id = (int)dgv.CurrentRow.Cells["Id"].Value;
            var name = dgv.CurrentRow.Cells["Назва"].Value?.ToString();

            if (MessageBox.Show($"Видалити рецепт \"{name}\"?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _recipeService.Delete(id);
                LoadData();
            }
        }
    }
}