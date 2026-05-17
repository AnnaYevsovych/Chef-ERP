using ChefApp.Forms;

namespace ChefApp.Forms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Шеф-кухар — Система управління рецептурами";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "Шеф-кухар",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Left = 30,
                Top = 30
            };

            var lblSubtitle = new Label
            {
                Text = "Система управління рецептурами та складом",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.Gray,
                AutoSize = true,
                Left = 30,
                Top = 75
            };

            int btnW = 420, btnH = 48, btnX = 30, gap = 10;

            var btnProducts = CreateNavButton("Склад продуктів", btnX, 120, btnW, btnH, Color.FromArgb(52, 152, 219));
            var btnRecipes  = CreateNavButton("База рецептур", btnX, 120 + (btnH + gap), btnW, btnH, Color.FromArgb(46, 204, 113));
            var btnMenu     = CreateNavButton("Меню на день", btnX, 120 + (btnH + gap) * 2, btnW, btnH, Color.FromArgb(155, 89, 182));
            var btnInvoices = CreateNavButton("Видаткові накладні", btnX, 120 + (btnH + gap) * 3, btnW, btnH, Color.FromArgb(230, 126, 34));

            btnProducts.Click += (s, e) => new ProductForm().ShowDialog();
            btnRecipes.Click  += (s, e) => new RecipeForm().ShowDialog();
            btnMenu.Click     += (s, e) => new MenuForm().ShowDialog();
            btnInvoices.Click += (s, e) => new InvoiceForm().ShowDialog();

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubtitle,
                btnProducts, btnRecipes, btnMenu, btnInvoices
            });
        }

        private Button CreateNavButton(string text, int x, int y, int w, int h, Color color)
        {
            return new Button
            {
                Text = text,
                Left = x, Top = y, Width = w, Height = h,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };
        }
    }
}
