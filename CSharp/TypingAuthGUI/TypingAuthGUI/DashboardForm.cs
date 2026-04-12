using System;
using System.Drawing;
using System.Windows.Forms;

namespace TypingAuthGUI
{
    public partial class DashboardForm : Form
    {
        private string username;
        private double confidence;

        public DashboardForm(string username, double confidence)
        {
            this.username = username;
            this.confidence = confidence;
            InitializeComponent();
            BuildUI();
        }

        void BuildUI()
        {
            this.Text = "Dashboard";
            this.Size = new Size(550, 500);
            this.BackColor = Color.FromArgb(18, 18, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 30, 50)
            };
            var lblTitle = new Label
            {
                Text = "🏠 DASHBOARD",
                ForeColor = Color.FromArgb(100, 255, 150),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            // Welcome card
            var card = new Panel
            {
                Location = new Point(40, 100),
                Size = new Size(460, 200),
                BackColor = Color.FromArgb(30, 30, 50)
            };

            var lblWelcome = new Label
            {
                Text = $"✅ Welcome, {username}!",
                ForeColor = Color.LightGreen,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 25),
                AutoSize = true
            };

            var lblConf = new Label
            {
                Text = $"Authentication Confidence: {confidence}%",
                ForeColor = Color.FromArgb(100, 200, 255),
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 70),
                AutoSize = true
            };

            var confBar = new ProgressBar
            {
                Location = new Point(20, 100),
                Size = new Size(420, 20),
                Maximum = 100,
                Value = (int)confidence,
                Style = ProgressBarStyle.Continuous
            };

            var lblTime = new Label
            {
                Text = $"Login Time: {DateTime.Now:dd MMM yyyy — hh:mm tt}",
                ForeColor = Color.FromArgb(150, 150, 200),
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 135),
                AutoSize = true
            };

            var lblSecurity = new Label
            {
                Text = "🔒 Identity verified via typing biometrics",
                ForeColor = Color.FromArgb(100, 255, 150),
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 165),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] {
                lblWelcome, lblConf, confBar, lblTime, lblSecurity
            });

            // Logout button
            var btnLogout = new Button
            {
                Text = "LOGOUT",
                Location = new Point(40, 330),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => {
                new LoginForm().Show();
                this.Close();
            };

            this.Controls.AddRange(new Control[] {
                headerPanel, card, btnLogout
            });
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DashboardForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "DashboardForm";
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.ResumeLayout(false);

        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {

        }
    }
}