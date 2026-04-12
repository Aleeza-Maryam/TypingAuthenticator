using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TypingAuthGUI
{
    public partial class LoginForm : Form
    {
        private Label lblTitle, lblUsername, lblType, lblStatus;
        private TextBox txtUsername, txtTyping;
        private Button btnLogin, btnGoRegister;
        private Panel headerPanel;

        // Typing tracking
        private DateTime lastKeyTime;
        private System.Collections.Generic.List<double> pauses =
            new System.Collections.Generic.List<double>();
        private int errorCount = 0;
        private DateTime startTime;
        private bool typingStarted = false;

        private static readonly HttpClient client = new HttpClient();
        private const string PARAGRAPH = "the quick brown fox jumps over the lazy dog";

        public LoginForm()
        {
            InitializeComponent();
            BuildUI();
        }

        void BuildUI()
        {
            this.Text = "Typing Authenticator — Login";
            this.Size = new Size(550, 500);
            this.BackColor = Color.FromArgb(18, 18, 30);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 30, 50)
            };

            lblTitle = new Label
            {
                Text = "🔐 TYPING AUTHENTICATOR",
                ForeColor = Color.FromArgb(100, 200, 255),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            // Username
            lblUsername = new Label
            {
                Text = "Username:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 100),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(40, 125),
                Size = new Size(460, 35),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Typing box
            lblType = new Label
            {
                Text = $"Type this: \"{PARAGRAPH}\"",
                ForeColor = Color.FromArgb(150, 150, 200),
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 180),
                Size = new Size(460, 40),
                AutoSize = false
            };

            txtTyping = new TextBox
            {
                Location = new Point(40, 225),
                Size = new Size(460, 40),
                Font = new Font("Consolas", 11),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtTyping.KeyDown += TxtTyping_KeyDown;
            txtTyping.KeyPress += TxtTyping_KeyPress;

            // Status label
            lblStatus = new Label
            {
                Text = "Type the paragraph above to authenticate",
                ForeColor = Color.FromArgb(150, 150, 200),
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 280),
                Size = new Size(460, 30),
                AutoSize = false
            };

            // Login button
            btnLogin = new Button
            {
                Text = "AUTHENTICATE",
                Location = new Point(40, 330),
                Size = new Size(220, 45),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Register button
            btnGoRegister = new Button
            {
                Text = "NEW USER? REGISTER",
                Location = new Point(280, 330),
                Size = new Size(220, 45),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.FromArgb(100, 200, 255),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGoRegister.FlatAppearance.BorderSize = 1;
            btnGoRegister.Click += (s, e) => {
                new RegisterForm().Show();
                this.Hide();
            };

            this.Controls.AddRange(new Control[] {
                headerPanel, lblUsername, txtUsername,
                lblType, txtTyping, lblStatus,
                btnLogin, btnGoRegister
            });
        }

        private void TxtTyping_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!typingStarted)
            {
                typingStarted = true;
                startTime = DateTime.Now;
                lastKeyTime = DateTime.Now;
                return;
            }

            double pause = (DateTime.Now - lastKeyTime).TotalSeconds;
            pauses.Add(pause);
            lastKeyTime = DateTime.Now;
        }

        private void TxtTyping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                errorCount++;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "⚠ Please enter username!";
                return;
            }

            if (pauses.Count < 5)
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "⚠ Please type the paragraph first!";
                return;
            }

            lblStatus.ForeColor = Color.Yellow;
            lblStatus.Text = "🔄 Analyzing typing pattern...";
            btnLogin.Enabled = false;

            double totalTime = (DateTime.Now - startTime).TotalSeconds;
            double avgPause = 0;
            double maxPause = 0;
            double minPause = 0;
            double variance = 0;

            if (pauses.Count > 0)
            {
                avgPause = 0;
                foreach (var p in pauses) avgPause += p;
                avgPause /= pauses.Count;

                maxPause = pauses[0];
                minPause = pauses[0];
                foreach (var p in pauses)
                {
                    if (p > maxPause) maxPause = p;
                    if (p < minPause) minPause = p;
                }

                foreach (var p in pauses)
                    variance += Math.Pow(p - avgPause, 2);
                variance /= pauses.Count;
            }

            var payload = new
            {
                claimed_username = txtUsername.Text,
                total_time = Math.Round(totalTime, 2),
                avg_pause = Math.Round(avgPause, 4),
                max_pause = Math.Round(maxPause, 4),
                min_pause = Math.Round(minPause, 4),
                typing_speed = totalTime > 0 ?
                    Math.Round(txtTyping.Text.Length / totalTime, 2) : 0,
                error_count = errorCount,
                error_rate = Math.Round((double)errorCount / PARAGRAPH.Length, 4),
                rhythm_variance = Math.Round(variance, 6)
            };

            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(
                    "http://localhost:5000/predict", content);
                var result = JsonConvert.DeserializeObject<dynamic>(
                    await response.Content.ReadAsStringAsync());

                bool isAuth = (bool)result.is_authentic;
                double confidence = (double)result.confidence;

                if (isAuth)
                {
                    lblStatus.ForeColor = Color.LightGreen;
                    lblStatus.Text = $"✅ Access Granted! Confidence: {confidence}%";

                    System.Threading.Thread.Sleep(1000);
                    new DashboardForm(txtUsername.Text, confidence).Show();
                    this.Hide();
                }
                else
                {
                    lblStatus.ForeColor = Color.OrangeRed;
                    lblStatus.Text = $"❌ Impostor Detected! Confidence: {confidence}%";
                    txtTyping.Clear();
                    pauses.Clear();
                    errorCount = 0;
                    typingStarted = false;
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "❌ Python API not running! Start predict.py first.";
            }

            btnLogin.Enabled = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}