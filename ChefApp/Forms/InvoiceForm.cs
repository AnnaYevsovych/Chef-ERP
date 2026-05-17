using ChefApp.Models;
using ChefApp.Services;

namespace ChefApp.Forms
{
    public class InvoiceForm : Form
    {
        private readonly InvoiceService _invoiceService = new InvoiceService();

        private DataGridView dgvInvoices;
        private DataGridView dgvLines;
        private Button btnDelete, btnClose;
        private Label lblTotal;

        public InvoiceForm()
        {
            InitializeComponent();
            LoadInvoices();
        }

        private void InitializeComponent()
        {
            this.Text = "Видаткові накладні";
            this.Size = new Size(900, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(700, 450);

            // --- Верхня таблиця: список накладних ---
            var lblInvoices = new Label
            {
                Text = "Список накладних:",
                Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(6, 4, 0, 0)
            };

            dgvInvoices = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 200,
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
            dgvInvoices.SelectionChanged += (s, e) => LoadLines();

            // --- Роздільник ---
            var lblLines = new Label
            {
                Text = "Склад накладної (рядки):",
                Dock = DockStyle.Top, Height = 24,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(6, 4, 0, 0)
            };

            // --- Нижня таблиця: рядки накладної ---
            dgvLines = new DataGridView
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

            // --- Панель кнопок ---
            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 46 };

            lblTotal = new Label
            {
                Left = 10, Top = 14, AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            btnDelete = new Button
            {
                Text = "Видалити накладну", Width = 160, Height = 32,
                Left = 550, Top = 7
            };
            btnDelete.Click += (s, e) => DeleteInvoice();

            btnClose = new Button
            {
                Text = "Закрити", Width = 100, Height = 32,
                Left = 720, Top = 7
            };
            btnClose.Click += (s, e) => this.Close();

            panelBottom.Controls.AddRange(new Control[] { lblTotal, btnDelete, btnClose });

            this.Controls.Add(dgvLines);
            this.Controls.Add(lblLines);
            this.Controls.Add(dgvInvoices);
            this.Controls.Add(lblInvoices);
            this.Controls.Add(panelBottom);
        }

        private void LoadInvoices()
        {
            var invoices = _invoiceService.GetAll();

            dgvInvoices.DataSource = null;
            dgvInvoices.Columns.Clear();

            var source = invoices.Select(inv => new
            {
                inv.Id,
                Дата     = inv.Date.ToString("dd.MM.yyyy HH:mm"),
                Примітка = inv.Notes,
                Персон   = inv.Persons,
                Позицій  = inv.Lines.Count,
                Сума     = inv.TotalAmount.ToString("F2") + " грн"
            }).ToList();

            dgvInvoices.DataSource = source;

            if (dgvInvoices.Columns.Contains("Id"))
                dgvInvoices.Columns["Id"].Visible = false;

            dgvLines.DataSource = null;
            dgvLines.Columns.Clear();
            lblTotal.Text = "";
        }

        private void LoadLines()
        {
            if (dgvInvoices.CurrentRow == null) return;
            var id = (int)dgvInvoices.CurrentRow.Cells["Id"].Value;
            var invoice = _invoiceService.GetById(id);
            if (invoice == null) return;

            dgvLines.DataSource = null;
            dgvLines.Columns.Clear();

            var source = invoice.Lines.Select(l => new
            {
                Продукт  = l.ProductName,
                Одиниця  = l.Unit,
                Кількість = Math.Round(l.Quantity, 3),
                Ціна     = l.PricePerUnit.ToString("F2"),
                Сума     = l.TotalPrice.ToString("F2") + " грн"
            }).ToList();

            dgvLines.DataSource = source;
            lblTotal.Text = $"Загальна сума: {invoice.TotalAmount:F2} грн";
        }

        private void DeleteInvoice()
        {
            if (dgvInvoices.CurrentRow == null) return;
            var id = (int)dgvInvoices.CurrentRow.Cells["Id"].Value;

            if (MessageBox.Show("Видалити накладну? Залишки складу не будуть відновлені.",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _invoiceService.Delete(id);
                LoadInvoices();
            }
        }
    }
}
