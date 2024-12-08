using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace g7_Clinic_Management
{
    public partial class AddPatientForm : Form
    {
        private string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        public AddPatientForm()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            DateTime dob = dtpDOB.Value;
            string phone = txtPhone.Text;
            string address = txtAddress.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Patient (Name, DateOfBirth, PhoneNumber, Address) VALUES (@Name, @DOB, @Phone, @Address)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@DOB", dob);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);

                        cmd.ExecuteNonQuery();

                        // Display confirmation message
                        MessageBox.Show("Patient added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Call DisplayChanges to show the changes in terminal
                        Program.DisplayChanges("Patient added", $"Name: {name}, Date of Birth: {dob.ToShortDateString()}, Phone: {phone}, Address: {address}");

                        // Ask the user to press any key before closing the form (optional)
                        this.Close();  // Close the form
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the form when Exit is clicked
        }
    }
}
