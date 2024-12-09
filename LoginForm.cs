using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace g7_Clinic_Management
{
    public partial class LoginForm : Form
    {
        private readonly string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (ValidateLogin(username, password))
            {
                this.DialogResult = DialogResult.OK; // Successful login
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Credentials. Please Contact System Administrator.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        object dbPasswordHash = cmd.ExecuteScalar();

                        if (dbPasswordHash != null)
                        {
                            // Hash the input password and compare
                            string inputPasswordHash = ComputeSha256Hash(password);
                            return dbPasswordHash.ToString() == inputPasswordHash;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
