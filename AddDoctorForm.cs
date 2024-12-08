using System;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace g7_Clinic_Management
{
    public partial class AddDoctorForm : Form
    {
        private string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        public AddDoctorForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string specialty = cmbSpecialty.SelectedItem.ToString();
            string phoneNumber = txtPhoneNumber.Text;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Doctor (Name, Specialty, PhoneNumber) VALUES (@Name, @Specialty, @PhoneNumber);";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Specialty", specialty);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Doctor added successfully!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

