using System;
using System.Drawing;
using System.Net.Http;
using System.Windows.Forms;

namespace TypingAuthGUI
{
    public partial class RegisterForm : Form
    {
        private Label lblTitle, lblInfo, lblStatus, lblType;
        private TextBox txtUsername, txtTyping;
        private Button btnRecord, btnTrain, btnBack;
        private ProgressBar progressBar;
        private Panel headerPanel;

        private int sessionCount = 0;
        private const int TOTAL_SESSIONS = 8;
        private const string PARAGRAPH = "the quick brown fox jumps over the lazy dog";

        private System.Collections.Generic.List<double> pauses =
            new System.Collections.Generic.List<double>();
        private DateTime lastKeyTime;
        private bool typingStarted = false;
        private DateTime startTime;
        private int errorCount = 0;

        public RegisterForm()
        {
            InitializeComponent();
            BuildUI();
        }

        void BuildUI()
        {
            this.Text = "Register New User";
            this.Size = new Size(550, 550);
            this.BackColor = Color.FromArgb(18, 18, 30);
            this.StartPosition = FormStartPosition.CenterScreen;

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 30, 50)
            };

            lblTitle = new Label
            {
                Text = "📝 REGISTER NEW USER",
                ForeColor = Color.FromArgb(100, 255, 150),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            var lblUser = new Label
            {
                Text = "Choose Username:",
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

            lblType = new Label
            {
                Text = $"Type this paragraph ({TOTAL_SESSIONS} times):\n\"{PARAGRAPH}\"",
                ForeColor = Color.FromArgb(150, 150, 200),
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 180),
                Size = new Size(460, 45),
                AutoSize = false
            };

            txtTyping = new TextBox
            {
                Location = new Point(40, 230),
                Size = new Size(460, 40),
                Font = new Font("Consolas", 11),
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtTyping.KeyPress += TxtTyping_KeyPress;
            txtTyping.KeyDown += TxtTyping_KeyDown;

            progressBar = new ProgressBar
            {
                Location = new Point(40, 290),
                Size = new Size(460, 20),
                Maximum = TOTAL_SESSIONS,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            lblInfo = new Label
            {
                Text = $"Session: 0 / {TOTAL_SESSIONS}",
                ForeColor = Color.FromArgb(150, 200, 255),
                Font = new Font("Segoe UI", 10),
                Location = new Point(40, 320),
                AutoSize = true
            };

            lblStatus = new Label
            {
                Text = "Enter username and start typing!",
                ForeColor = Color.FromArgb(150, 150, 200),
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 350),
                Size = new Size(460, 30),
                AutoSize = false
            };

            btnRecord = new Button
            {
                Text = "SAVE SESSION",
                Location = new Point(40, 400),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRecord.FlatAppearance.BorderSize = 0;
            btnRecord.Click += BtnRecord_Click;

            btnTrain = new Button
            {
                Text = "TRAIN MODEL",
                Location = new Point(260, 400),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(150, 100, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnTrain.FlatAppearance.BorderSize = 0;
            btnTrain.Click += BtnTrain_Click;

            btnBack = new Button
            {
                Text = "← Back to Login",
                Location = new Point(40, 460),
                Size = new Size(200, 35),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 200, 255),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.Click += (s, e) => {
                new LoginForm().Show();
                this.Close();
            };

            this.Controls.AddRange(new Control[] {
                headerPanel, lblUser, txtUsername,
                lblType, txtTyping, progressBar,
                lblInfo, lblStatus, btnRecord,
                btnTrain, btnBack
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
            pauses.Add((DateTime.Now - lastKeyTime).TotalSeconds);
            lastKeyTime = DateTime.Now;
        }

        private void TxtTyping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back) errorCount++;
        }

        private void BtnRecord_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "⚠ Enter username first!";
                return;
            }

            if (pauses.Count < 5)
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "⚠ Type the paragraph first!";
                return;
            }

            // Data save karo CSV mein
            SaveToCSV();
            sessionCount++;

            progressBar.Value = sessionCount;
            lblInfo.Text = $"Session: {sessionCount} / {TOTAL_SESSIONS}";

            // Reset
            txtTyping.Clear();
            pauses.Clear();
            errorCount = 0;
            typingStarted = false;

            if (sessionCount >= TOTAL_SESSIONS)
            {
                lblStatus.ForeColor = Color.LightGreen;
                lblStatus.Text = $"✅ All {TOTAL_SESSIONS} sessions done! Train model now.";
                btnRecord.Enabled = false;
                btnTrain.Enabled = true;
            }
            else
            {
                lblStatus.ForeColor = Color.Yellow;
                lblStatus.Text = $"✅ Session {sessionCount} saved! {TOTAL_SESSIONS - sessionCount} more to go.";
            }
        }

        private void SaveToCSV()
        {
            double totalTime = (DateTime.Now - startTime).TotalSeconds;
            double avgPause = 0;
            foreach (var p in pauses) avgPause += p;
            avgPause = pauses.Count > 0 ? avgPause / pauses.Count : 0;

            double maxP = pauses.Count > 0 ? pauses[0] : 0;
            double minP = pauses.Count > 0 ? pauses[0] : 0;
            foreach (var p in pauses)
            {
                if (p > maxP) maxP = p;
                if (p < minP) minP = p;
            }

            double variance = 0;
            foreach (var p in pauses)
                variance += Math.Pow(p - avgPause, 2);
            variance = pauses.Count > 0 ? variance / pauses.Count : 0;

            string dataPath = @"E:\C#\TypingAuthenticator\Python\data\typing_data.csv";
            bool fileExists = System.IO.File.Exists(dataPath);

            using (var sw = new System.IO.StreamWriter(dataPath, true))
            {
                if (!fileExists)
                    sw.WriteLine("username,total_time,avg_pause,max_pause,min_pause,typing_speed,error_count,error_rate,rhythm_variance");

                sw.WriteLine($"{txtUsername.Text}," +
                    $"{Math.Round(totalTime, 2)}," +
                    $"{Math.Round(avgPause, 4)}," +
                    $"{Math.Round(maxP, 4)}," +
                    $"{Math.Round(minP, 4)}," +
                    $"{(totalTime > 0 && txtTyping.Text != null ? Math.Round(txtTyping.Text.Length / totalTime, 2) : 0)}," +
                    $"{errorCount}," +
                    $"{Math.Round((double)errorCount / PARAGRAPH.Length, 4)}," +
                    $"{Math.Round(variance, 6)}");
            }
        }

        private void BtnTrain_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.Yellow;
            lblStatus.Text = "🔄 Training model...";

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = @"E:\C#\TypingAuthenticator\Python\train_model.py";
            process.StartInfo.UseShellExecute = true;
            process.Start();
            process.WaitForExit();

            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Text = "✅ Model trained! Go back to Login.";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}