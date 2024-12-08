using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public partial class UpdateAppointmentForm : Form
    {
        private string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";
        private int appointmentId;

        // Declare fields to hold the current PatientID and DoctorID
        private int currentPatientId;
        private int currentDoctorId;

        public UpdateAppointmentForm(int appointmentId)
        {
            InitializeComponent();
            this.appointmentId = appointmentId;
            LoadAppointmentDetails();
            LoadPatientNames();
            LoadDoctorNames();
        }

        private void LoadAppointmentDetails()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT a.AppointmentDate, a.Time, a.Reason, a.PatientID, a.DoctorID
                        FROM Appointment a
                        WHERE a.AppointmentID = @AppointmentID;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                datePickerAppointmentDate.Value = Convert.ToDateTime(reader["AppointmentDate"]);
                                timePickerAppointmentTime.Value = DateTime.Today.Add((TimeSpan)reader["Time"]);
                                textBoxReason.Text = reader["Reason"].ToString();

                                // Assign PatientID and DoctorID for comboBox selection
                                currentPatientId = Convert.ToInt32(reader["PatientID"]);
                                currentDoctorId = Convert.ToInt32(reader["DoctorID"]);
                            }
                            else
                            {
                                MessageBox.Show("No appointment found for the given ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading appointment details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

                        comboBoxPatientName.DataSource = patientTable;
                        comboBoxPatientName.DisplayMember = "Name";
                        comboBoxPatientName.ValueMember = "PatientID";

                        // Set the selected value to match the currentPatientId
                        if (patientTable.Rows.Count > 0)
                        {
                            comboBoxPatientName.SelectedValue = currentPatientId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading patient names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        comboBoxDoctorName.DataSource = doctorTable;
                        comboBoxDoctorName.DisplayMember = "Name";
                        comboBoxDoctorName.ValueMember = "DoctorID";

                        // Set the selected value to match the currentDoctorId
                        if (doctorTable.Rows.Count > 0)
                        {
                            comboBoxDoctorName.SelectedValue = currentDoctorId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading doctor names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE Appointment
                        SET PatientID = @PatientID,
                            DoctorID = @DoctorID,
                            AppointmentDate = @AppointmentDate,
                            Time = @AppointmentTime,
                            Reason = @Reason
                        WHERE AppointmentID = @AppointmentID;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", comboBoxPatientName.SelectedValue);
                        cmd.Parameters.AddWithValue("@DoctorID", comboBoxDoctorName.SelectedValue);
                        cmd.Parameters.AddWithValue("@AppointmentDate", datePickerAppointmentDate.Value.Date);
                        cmd.Parameters.AddWithValue("@AppointmentTime", timePickerAppointmentTime.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@Reason", textBoxReason.Text);
                        cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Appointment updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update the appointment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
