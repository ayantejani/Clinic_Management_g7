using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public partial class AddAppointmentForm : Form
    {
        private string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";

        public AddAppointmentForm()
        {
            InitializeComponent();
            LoadPatientNames();
            LoadDoctorNames();
        }

        // Load Patient Names
        private void LoadPatientNames()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT PatientID, Name FROM Patient;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        System.Data.DataTable patientTable = new System.Data.DataTable();
                        adapter.Fill(patientTable);

                        // Add an empty row for default selection
                        DataRow emptyRow = patientTable.NewRow();
                        emptyRow["PatientID"] = DBNull.Value; // Set empty ID
                        emptyRow["Name"] = "-- Select Patient --";
                        patientTable.Rows.InsertAt(emptyRow, 0);

                        comboBoxPatientName.DataSource = patientTable;
                        comboBoxPatientName.DisplayMember = "Name";
                        comboBoxPatientName.ValueMember = "PatientID";
                        comboBoxPatientName.SelectedIndex = 0; // Set default selection
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading patient names: " + ex.Message);
            }
        }

        private void LoadDoctorNames()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DoctorID, Name FROM Doctor;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        System.Data.DataTable doctorTable = new System.Data.DataTable();
                        adapter.Fill(doctorTable);

                        // Add an empty row for default selection
                        DataRow emptyRow = doctorTable.NewRow();
                        emptyRow["DoctorID"] = DBNull.Value; // Set empty ID
                        emptyRow["Name"] = "-- Select Doctor --";
                        doctorTable.Rows.InsertAt(emptyRow, 0);

                        comboBoxDoctorName.DataSource = doctorTable;
                        comboBoxDoctorName.DisplayMember = "Name";
                        comboBoxDoctorName.ValueMember = "DoctorID";
                        comboBoxDoctorName.SelectedIndex = 0; // Set default selection
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading doctor names: " + ex.Message);
            }
        }


        // Submit Button Click
        private void ButtonSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate that a patient and a doctor are selected
                if (comboBoxPatientName.SelectedValue == null || comboBoxDoctorName.SelectedValue == null)
                {
                    MessageBox.Show("Please select both a patient and a doctor.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Appointment (PatientID, DoctorID, AppointmentDate, Time, Reason) " +
                                   "VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", comboBoxPatientName.SelectedValue);
                        cmd.Parameters.AddWithValue("@DoctorID", comboBoxDoctorName.SelectedValue);
                        cmd.Parameters.AddWithValue("@AppointmentDate", datePickerAppointmentDate.Value.Date);
                        cmd.Parameters.AddWithValue("@AppointmentTime", timePickerAppointmentTime.Value.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Reason", textBoxReason.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Appointment added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                this.Close(); // Close the form after adding the appointment
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cancel Button Click
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
