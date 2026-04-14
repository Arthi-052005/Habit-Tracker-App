using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using System.Windows.Forms.DataVisualization.Charting;

namespace Habit_Tracker
{
    public class Dashboard : Form
    {
        string currentUser;

        Panel sidebar;
        Panel contentPanel;

        TextBox txtHabit;
        Button btnAdd;
        Button btnDelete;
        Button btnComplete;
        ListBox habitList;
        Chart habitChart;

        Label lblQuote;
        Label lblAnalyticsTitle;

        public Dashboard(string username)
        {
            currentUser = username;
            DesignUI();
        }

        private void DesignUI()
        {
            this.Text = "Habit Tracker - Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(40, 40, 80);

            // Sidebar
            sidebar = new Panel()
            {
                Width = 220,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(20, 20, 50)
            };

            Label lblTitle = new Label()
            {
                Text = "Habit Tracker",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };

            Button btnHabits = CreateSidebarButton("🏠 My Habits", 100);
            Button btnAnalytics = CreateSidebarButton("📊 Analytics", 160);
            Button btnLogout = CreateSidebarButton("🚪 Logout", 220);

            btnHabits.Click += ShowHabits;
            btnAnalytics.Click += ShowAnalytics;
            btnLogout.Click += BtnLogout_Click;

            sidebar.Controls.Add(lblTitle);
            sidebar.Controls.Add(btnHabits);
            sidebar.Controls.Add(btnAnalytics);
            sidebar.Controls.Add(btnLogout);

            // Content Panel
            contentPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(55, 55, 110)
            };

            Label welcome = new Label()
            {
                Text = $"Welcome, {currentUser} 🎯",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(260, 20),
                AutoSize = true
            };

            lblQuote = new Label()
            {
                Text = "“Small daily improvements lead to big results.”",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                Location = new Point(260, 60),
                AutoSize = true
            };

            txtHabit = new TextBox()
            {
                Location = new Point(260, 100),
                Width = 300
            };

            btnAdd = new Button()
            {
                Text = "Add Habit",
                Location = new Point(580, 100),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            habitList = new ListBox()
            {
                Location = new Point(260, 150),
                Size = new Size(500, 250)
            };

            btnDelete = new Button()
            {
                Text = "Delete",
                Location = new Point(260, 420),
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnComplete = new Button()
            {
                Text = "Mark Complete",
                Location = new Point(400, 420),
                BackColor = Color.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Analytics Title
            lblAnalyticsTitle = new Label()
            {
                Text = "📊 Habit Analytics",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Visible = false
            };

            // Chart
            habitChart = new Chart()
            {
                Size = new Size(500, 320),
                Visible = false
            };

            ChartArea area = new ChartArea();
            area.BackColor = Color.White;
            habitChart.ChartAreas.Add(area);

            Series series = new Series()
            {
                ChartType = SeriesChartType.Column
            };

            habitChart.Series.Add(series);

            // Events
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnComplete.Click += BtnComplete_Click;

            // Add controls
            contentPanel.Controls.Add(welcome);
            contentPanel.Controls.Add(lblQuote);
            contentPanel.Controls.Add(txtHabit);
            contentPanel.Controls.Add(btnAdd);
            contentPanel.Controls.Add(habitList);
            contentPanel.Controls.Add(btnDelete);
            contentPanel.Controls.Add(btnComplete);
            contentPanel.Controls.Add(habitChart);
            contentPanel.Controls.Add(lblAnalyticsTitle);

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebar);

            LoadHabits();
        }

        private Button CreateSidebarButton(string text, int top)
        {
            return new Button()
            {
                Text = text,
                Width = 170,
                Height = 45,
                Location = new Point(25, top),
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void ShowHabits(object sender, EventArgs e)
        {
            habitChart.Visible = false;
            lblAnalyticsTitle.Visible = false;

            txtHabit.Visible = true;
            btnAdd.Visible = true;
            habitList.Visible = true;
            btnDelete.Visible = true;
            btnComplete.Visible = true;
        }

        private void ShowAnalytics(object sender, EventArgs e)
        {
            txtHabit.Visible = false;
            btnAdd.Visible = false;
            habitList.Visible = false;
            btnDelete.Visible = false;
            btnComplete.Visible = false;

            habitChart.Visible = true;
            lblAnalyticsTitle.Visible = true;

            CenterAnalyticsUI();
            LoadChart();
        }

        private void CenterAnalyticsUI()
        {
            int sidebarWidth = sidebar.Width;
            int availableWidth = this.ClientSize.Width - sidebarWidth;

            habitChart.Left = sidebarWidth + (availableWidth - habitChart.Width) / 2;
            habitChart.Top = (this.ClientSize.Height - habitChart.Height) / 2;

            lblAnalyticsTitle.Left = habitChart.Left + (habitChart.Width - lblAnalyticsTitle.Width) / 2;
            lblAnalyticsTitle.Top = habitChart.Top - 50;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (habitChart.Visible)
                CenterAnalyticsUI();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }

        private void LoadHabits()
        {
            habitList.Items.Clear();

            var db = MongoDBConnection.GetDatabase();
            var collection = db.GetCollection<Habit>("Habits");

            var habits = collection.Find(h => h.Username == currentUser).ToList();

            foreach (var habit in habits)
            {
                string status = habit.IsCompleted ? "✅ Done" : "❌ Pending";

                habitList.Items.Add(
                    $"{habit.Title} | {habit.Date.ToShortDateString()} | {status} | 🔥 {habit.Streak}"
                );
            }
        }

        private void LoadChart()
        {
            var db = MongoDBConnection.GetDatabase();
            var collection = db.GetCollection<Habit>("Habits");

            var habits = collection.Find(h => h.Username == currentUser).ToList();

            int total = habits.Count;
            int completed = habits.Count(h => h.IsCompleted);
            int pending = total - completed;

            habitChart.Series[0].Points.Clear();

            habitChart.Series[0].Points.AddXY("Total", total);
            habitChart.Series[0].Points.AddXY("Completed", completed);
            habitChart.Series[0].Points.AddXY("Pending", pending);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHabit.Text))
            {
                MessageBox.Show("Enter a habit!");
                return;
            }

            var db = MongoDBConnection.GetDatabase();
            var collection = db.GetCollection<Habit>("Habits");

            var habit = new Habit
            {
                Title = txtHabit.Text,
                Username = currentUser,
                IsCompleted = false,
                Date = DateTime.Now,
                CompletedDate = DateTime.MinValue,
                Streak = 0
            };

            collection.InsertOne(habit);

            txtHabit.Clear();
            LoadHabits();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (habitList.SelectedItem == null)
            {
                MessageBox.Show("Select a habit!");
                return;
            }

            var selected = habitList.SelectedItem.ToString().Split('|')[0].Trim();

            var db = MongoDBConnection.GetDatabase();
            var collection = db.GetCollection<Habit>("Habits");

            collection.DeleteOne(h => h.Title == selected && h.Username == currentUser);

            LoadHabits();
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (habitList.SelectedItem == null)
            {
                MessageBox.Show("Select a habit!");
                return;
            }

            var selected = habitList.SelectedItem.ToString().Split('|')[0].Trim();

            var db = MongoDBConnection.GetDatabase();
            var collection = db.GetCollection<Habit>("Habits");

            var habit = collection.Find(h =>
                h.Title == selected &&
                h.Username == currentUser
            ).FirstOrDefault();

            // 🔴 FIXED NULL ERROR
            if (habit == null)
            {
                MessageBox.Show("Habit not found!");
                return;
            }

            int newStreak = habit.Streak;

            if (habit.CompletedDate.Date == DateTime.Now.Date.AddDays(-1))
                newStreak++;
            else
                newStreak = 1;

            var update = Builders<Habit>.Update
                .Set(h => h.IsCompleted, true)
                .Set(h => h.CompletedDate, DateTime.Now)
                .Set(h => h.Streak, newStreak);

            collection.UpdateOne(h => h.Id == habit.Id, update);

            LoadHabits();
        }
    }
}
