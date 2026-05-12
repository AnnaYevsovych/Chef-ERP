using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class ProductForm : Form
    {
        private readonly ProductService _service = new ProductService();

        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private Label lblWarning;

        public ProductForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Склад продуктів";
            this.Size = new Size(860, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(700, 400);

            // --- Таблиця ---
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

            // --- Панель кнопок ---
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 50 };

            btnAdd = new Button { Text = "Додати", Width = 100, Height = 32, Left = 10, Top = 9 };
            btnEdit = new Button { Text = "Редагувати", Width = 110, Height = 32, Left = 120, Top = 9 };
            btnDelete = new Button { Text = "Видалити", Width = 100, Height = 32, Left = 240, Top = 9 };
            btnRefresh = new Button { Text = "Оновити", Width = 100, Height = 32, Left = 350, Top = 9 };

            btnAdd.Click += (s, e) => AddProduct();
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRefresh.Click += (s, e) => LoadData();

            // --- Попередження про прострочені ---
            lblWarning = new Label
            {
                AutoSize = false,
                Height = 24,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Padding = new Padding(8, 0, 0, 0),
                Visible = false
            };

            panel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            this.Controls.Add(dgv);
            this.Controls.Add(lblWarning);
            this.Controls.Add(panel);
        }

        private void LoadData()
        {
            var products = _service.GetAll();

            dgv.DataSource = null;
            dgv.Columns.Clear();

            var source = products.Select(p => new
            {
                p.Id,
                Назва = p.Name,
                Одиниця = p.Unit,
                Ціна = p.Price,
                Кількість = p.Quantity,
                Термін = p.ExpiryDate.ToString("dd.MM.yyyy"),
                Статус = p.IsExpired ? "ПРОСТРОЧЕНО" : p.IsExpiringSoon ? "Спливає" : "OK"
            }).ToList();

            dgv.DataSource = source;

            // Приховати Id
            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;

            // Кольорування рядків
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var status = row.Cells["Статус"].Value?.ToString();
                if (status == "ПРОСТРОЧЕНО")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                else if (status == "Спливає")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
            }

            // Попередження
            var expired = _service.GetExpired();
            var expiringSoon = _service.GetExpiringSoon();

            if (expired.Count > 0)
            {
                lblWarning.Text = $"  Увага! Прострочені продукти: {string.Join(", ", expired.Select(p => p.Name))}";
                lblWarning.Visible = true;
            }
            else if (expiringSoon.Count > 0)
            {
                lblWarning.Text = $"  Спливає термін: {string.Join(", ", expiringSoon.Select(p => p.Name))}";
                lblWarning.ForeColor = Color.DarkOrange;
                lblWarning.Visible = true;
            }
            else
            {
                lblWarning.Visible = false;
            }
        }

        private void AddProduct()
        {
            using var form = new ProductEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _service.Add(form.Result);
                LoadData();
            }
        }

        private void EditSelected()
        {
            if (dgv.CurrentRow == null) return;
            var id = (int)dgv.CurrentRow.Cells["Id"].Value;
            var product = _service.GetById(id);
            if (product == null) return;

            using var form = new ProductEditForm(product);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _service.Update(form.Result);
                LoadData();
            }
        }

        private void DeleteSelected()
        {
            if (dgv.CurrentRow == null) return;
            var id = (int)dgv.CurrentRow.Cells["Id"].Value;
            var name = dgv.CurrentRow.Cells["Назва"].Value?.ToString();

            if (MessageBox.Show($"Видалити продукт \"{name}\"?", "Підтвердження",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _service.Delete(id);
                LoadData();
            }
        }
    }
}
