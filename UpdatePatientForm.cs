using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace g7_Clinic_Management
{
    public partial class UpdatePatientForm : Form
    {
        private string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";
        private int patientId;

        public UpdatePatientForm(int patientId)
        {
            InitializeComponent();
            this.patientId = patientId;
            LoadPatientData();
        }

        private void LoadPatientData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Patient WHERE PatientID = @PatientID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["Name"].ToString();
                                dtpDOB.Value = Convert.ToDateTime(reader["DateOfBirth"]);
                                txtPhone.Text = reader["PhoneNumber"].ToString();
                                txtAddress.Text = reader["Address"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                    string query = "UPDATE Patient SET Name = @Name, DateOfBirth = @DOB, PhoneNumber = @Phone, Address = @Address WHERE PatientID = @PatientID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@DOB", dob);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@PatientID", patientId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Your changes have been saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Call DisplayChanges to show the updated patient details in the terminal
                        DisplayUpdatedPatientDetails(connection);

                        // Set DialogResult to indicate that the form was closed successfully
                        this.DialogResult = DialogResult.OK; // This signals that the form is closing properly
                        this.Close(); // Close the form
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

        // Method to fetch and display the updated patient details
        private void DisplayUpdatedPatientDetails(MySqlConnection connection)
        {
            string query = "SELECT * FROM Patient WHERE PatientID = @PatientID"; // Fetch the updated patient
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Updated Patient Details ---");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Patient ID: {reader["PatientID"]}");
                                Console.WriteLine($"Name: {reader["Name"]}");
                                Console.WriteLine($"Date of Birth: {Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString()}");
                                Console.WriteLine($"Phone: {reader["PhoneNumber"]}");
                                Console.WriteLine($"Address: {reader["Address"]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No patient details found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching the updated patient details: " + ex.Message);
            }
        }
    }
}
