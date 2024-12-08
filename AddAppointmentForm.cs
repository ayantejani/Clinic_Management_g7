using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public partial class AddAppointmentForm : Form
    {
        private string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        public AddAppointmentForm()
        {
            InitializeComponent();
            LoadPatientNames();
            LoadDoctorNames();
        }

        // Load patient names into the comboBoxPatientName dropdown
        private void LoadPatientNames()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT PatientID, Name FROM Patient;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayText = $"{reader["Name"]} (ID: {reader["PatientID"]})";
                            comboBoxPatientName.Items.Add(new ComboBoxItem(displayText, reader["PatientID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading patient names: " + ex.Message);
            }
        }

        // Load doctor names into the comboBoxDoctorName dropdown
        private void LoadDoctorNames()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DoctorID, Name FROM Doctor;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string displayText = $"{reader["Name"]} (ID: {reader["DoctorID"]})";
                            comboBoxDoctorName.Items.Add(new ComboBoxItem(displayText, reader["DoctorID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading doctor names: " + ex.Message);
            }
        }

        // Event handler for the Submit button
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxPatientName.SelectedItem == null || comboBoxDoctorName.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(textBoxReason.Text))
                {
                    MessageBox.Show("All fields are required.");
                    return;
                }

                var selectedPatient = (ComboBoxItem)comboBoxPatientName.SelectedItem;
                var selectedDoctor = (ComboBoxItem)comboBoxDoctorName.SelectedItem;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Appointment (AppointmentDate, Time, PatientID, DoctorID, Reason) " +
                                   "VALUES (@Date, @Time, @PatientID, @DoctorID, @Reason);";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Date", datePickerAppointmentDate.Value.Date);
                        cmd.Parameters.AddWithValue("@Time", timePickerAppointmentTime.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@PatientID", selectedPatient.Value);
                        cmd.Parameters.AddWithValue("@DoctorID", selectedDoctor.Value);
                        cmd.Parameters.AddWithValue("@Reason", textBoxReason.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Appointment added successfully!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the appointment: " + ex.Message);
            }
        }
    }

    // Helper class to store both display text and value in a ComboBox
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public ComboBoxItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
