using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class MenuForm : Form
    {
        private readonly MenuService _menuService = new MenuService();
        private readonly RecipeService _recipeService = new RecipeService();
        private readonly ProductService _productService = new ProductService();

        private DateTimePicker dtpDate;
        private NumericUpDown nudPersons;
        private DataGridView dgvMenu;
        private Button btnAddDish, btnRemoveDish, btnCheckStock, btnSaveMenu;
        private Label lblPersons, lblDate, lblStatus;
        private ListBox lstDishes;

        private DailyMenu _currentMenu;

        public MenuForm()
        {
            InitializeComponent();
            LoadMenu(DateTime.Today);
        }

        private void InitializeComponent()
        {
            this.Text = "Меню на день";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(750, 500);

            // --- Верхня панель ---
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };

            lblDate = new Label { Text = "Дата:", Left = 10, Top = 16, AutoSize = true };
            dtpDate = new DateTimePicker
            {
                Left = 50, Top = 12, Width = 130,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            dtpDate.ValueChanged += (s, e) => LoadMenu(dtpDate.Value.Date);

            lblPersons = new Label { Text = "Персон:", Left = 200, Top = 16, AutoSize = true };
            nudPersons = new NumericUpDown
            {
                Left = 255, Top = 12, Width = 70,
                Minimum = 1, Maximum = 9999, Value = 10
            };
            nudPersons.ValueChanged += (s, e) => UpdatePersons();

            btnSaveMenu = new Button { Text = "Зберегти меню", Width = 130, Height = 30, Left = 350, Top = 10 };
            btnSaveMenu.Click += (s, e) => SaveMenu();

            btnCheckStock = new Button
            {
                Text = "Перевірити запаси",
                Width = 150, Height = 30, Left = 490, Top = 10,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCheckStock.FlatAppearance.BorderSize = 0;
            btnCheckStock.Click += (s, e) => CheckStock();

            panelTop.Controls.AddRange(new Control[] { lblDate, dtpDate, lblPersons, nudPersons, btnSaveMenu, btnCheckStock });

            // --- Ліва панель: список страв меню ---
            var panelLeft = new Panel { Dock = DockStyle.Fill };

            var lblMenu = new Label
            {
                Text = "Страви в меню:",
                Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(4, 4, 0, 0)
            };

            dgvMenu = new DataGridView
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

            var panelMenuButtons = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            btnRemoveDish = new Button { Text = "Видалити страву", Width = 140, Height = 30, Left = 10, Top = 5 };
            btnRemoveDish.Click += (s, e) => RemoveDish();
            panelMenuButtons.Controls.Add(btnRemoveDish);

            panelLeft.Controls.Add(dgvMenu);
            panelLeft.Controls.Add(lblMenu);
            panelLeft.Controls.Add(panelMenuButtons);

            // --- Права панель: список рецептів ---
            var panelRight = new Panel { Dock = DockStyle.Right, Width = 280 };
            panelRight.BorderStyle = BorderStyle.FixedSingle;

            var lblRecipes = new Label
            {
                Text = "Рецептури (додати до меню):",
                Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(4, 4, 0, 0)
            };

            lstDishes = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.None
            };

            btnAddDish = new Button
            {
                Text = "Додати до меню →",
                Dock = DockStyle.Bottom, Height = 36,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnAddDish.FlatAppearance.BorderSize = 0;
            btnAddDish.Click += (s, e) => AddDish();
            lstDishes.DoubleClick += (s, e) => AddDish();

            panelRight.Controls.Add(lstDishes);
            panelRight.Controls.Add(lblRecipes);
            panelRight.Controls.Add(btnAddDish);

            // --- Статусний рядок ---
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom, Height = 26,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.DimGray
            };

            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
            this.Controls.Add(panelTop);
            this.Controls.Add(lblStatus);

            // Заповнити список рецептів
            foreach (var r in _recipeService.GetAll())
                lstDishes.Items.Add(r);
        }

        private void LoadMenu(DateTime date)
        {
            _currentMenu = _menuService.GetByDate(date);
            if (_currentMenu == null)
            {
                _currentMenu = new DailyMenu
                {
                    Date = date,
                    Persons = (int)nudPersons.Value,
                    Items = new List<MenuItem>()
                };
            }
            nudPersons.Value = _currentMenu.Persons;
            RefreshMenuGrid();
        }

        private void UpdatePersons()
        {
            if (_currentMenu != null)
                _currentMenu.Persons = (int)nudPersons.Value;
        }

        private void RefreshMenuGrid()
        {
            dgvMenu.DataSource = null;
            dgvMenu.Columns.Clear();

            var source = _currentMenu.Items.Select((item, idx) => new
            {
                Index     = idx,
                Страва    = item.RecipeName,
                Категорія = item.Category
            }).ToList();

            dgvMenu.DataSource = source;

            if (dgvMenu.Columns.Contains("Index"))
                dgvMenu.Columns["Index"].Visible = false;

            lblStatus.Text = $"Страв у меню: {_currentMenu.Items.Count}  |  Персон: {_currentMenu.Persons}  |  Дата: {_currentMenu.Date:dd.MM.yyyy}";
        }

        private void AddDish()
        {
            if (lstDishes.SelectedItem is not Recipe recipe) return;

            // Перевірити чи вже є така страва
            if (_currentMenu.Items.Any(i => i.RecipeId == recipe.Id))
            {
                MessageBox.Show($"Страва \"{recipe.Name}\" вже є в меню.", "Увага",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _currentMenu.Items.Add(new MenuItem
            {
                RecipeId   = recipe.Id,
                RecipeName = recipe.Name,
                Category   = recipe.Category,
                Persons    = _currentMenu.Persons,
                MenuDate   = _currentMenu.Date
            });

            RefreshMenuGrid();
        }

        private void RemoveDish()
        {
            if (dgvMenu.CurrentRow == null) return;
            var idx = (int)dgvMenu.CurrentRow.Cells["Index"].Value;
            _currentMenu.Items.RemoveAt(idx);
            RefreshMenuGrid();
        }

        private void SaveMenu()
        {
            if (_currentMenu.Items.Count == 0)
            {
                MessageBox.Show("Додайте хоча б одну страву до меню.", "Порожнє меню",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentMenu.Persons = (int)nudPersons.Value;

            if (_currentMenu.Id == 0)
                _menuService.Add(_currentMenu);
            else
                _menuService.Update(_currentMenu);

            MessageBox.Show("Меню збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshMenuGrid();
        }

        private void CheckStock()
        {
            if (_currentMenu.Items.Count == 0)
            {
                MessageBox.Show("Спочатку додайте страви до меню.", "Порожнє меню",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Зберегти якщо ще не збережено
            if (_currentMenu.Id == 0) SaveMenu();

            using var form = new StockCheckForm(_currentMenu, _recipeService, _productService);
            form.ShowDialog();

            // Перезавантажити меню після можливого списання
            LoadMenu(dtpDate.Value.Date);
        }
    }
}
