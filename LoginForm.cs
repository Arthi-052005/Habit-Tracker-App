using System;
using System.Drawing;
using System.Windows.Forms;
using MongoDB.Driver;

namespace Habit_Tracker
{
    public class LoginForm : Form
    {
        TextBox txtUsername;
        TextBox txtPassword;
        Button btnLogin;
        Button btnRegister;

        public LoginForm()
        {
            // ❌ DO NOT use InitializeComponent()
            DesignUI();
        }

        private void DesignUI()
        {
            this.Text = "Habit Tracker - Login";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 60);

            Label lblTitle = new Label()
            {
                Text = "Habit Tracker",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(90, 20)
            };

            Label lblUser = new Label()
            {
                Text = "Username",
                ForeColor = Color.White,
                Location = new Point(50, 90)
            };

            txtUsername = new TextBox()
            {
                Location = new Point(50, 110),
                Width = 280
            };

            Label lblPass = new Label()
            {
                Text = "Password",
                ForeColor = Color.White,
                Location = new Point(50, 150)
            };

            txtPassword = new TextBox()
            {
                Location = new Point(50, 170),
                Width = 280,
                PasswordChar = '*'
            };

            btnLogin = new Button()
            {
                Text = "Login",
                Location = new Point(50, 220),
                Width = 130,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnRegister = new Button()
            {
                Text = "Register",
                Location = new Point(200, 220),
                Width = 130,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                var db = MongoDBConnection.GetDatabase();
                var collection = db.GetCollection<User>("Users");

                var user = new User
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Text
                };

                collection.InsertOne(user);

                MessageBox.Show("Registered Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var db = MongoDBConnection.GetDatabase();
                var collection = db.GetCollection<User>("Users");

                var user = collection.Find(u =>
                    u.Username == txtUsername.Text &&
                    u.Password == txtPassword.Text
                ).FirstOrDefault();

                if (user != null)
                {
                    this.Hide();
                    new Dashboard(txtUsername.Text).Show();
                }
                else
                {
                    MessageBox.Show("Invalid Credentials!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}