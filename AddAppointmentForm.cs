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
                        DataTable patientTable = new DataTable();
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

        // Load Doctor Names
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
                        DataTable doctorTable = new DataTable();
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
            if (comboBoxPatientName.SelectedIndex == 0 || comboBoxDoctorName.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a valid patient and doctor.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
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
                        cmd.Parameters.AddWithValue("@AppointmentTime", timePickerAppointmentTime.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@Reason", textBoxReason.Text);

                        cmd.ExecuteNonQuery();

                        // Display success message
                        MessageBox.Show("Appointment added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Display the newly added appointment details in the console
                        DisplayAppointmentDetails(connection);
                    }
                }

                this.Close(); // Close the form after adding the appointment
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Display the newly added appointment details in the console
        private void DisplayAppointmentDetails(MySqlConnection connection)
        {
            string fetchQuery = @"
                SELECT a.AppointmentID, a.AppointmentDate, a.Time, 
                       p.Name AS PatientName, d.Name AS DoctorName, a.Reason
                FROM Appointment a
                INNER JOIN Patient p ON a.PatientID = p.PatientID
                INNER JOIN Doctor d ON a.DoctorID = d.DoctorID
                ORDER BY a.AppointmentID DESC
                LIMIT 1;"; // Fetch the most recent appointment

            try
            {
                using (MySqlCommand fetchCmd = new MySqlCommand(fetchQuery, connection))
                {
                    using (MySqlDataReader reader = fetchCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.Clear();
                            Console.WriteLine("\n--- Appointment Added ---");
                            Console.WriteLine($"Appointment ID: {reader["AppointmentID"]}");
                            Console.WriteLine($"Patient Name: {reader["PatientName"]}");
                            Console.WriteLine($"Doctor Name: {reader["DoctorName"]}");
                            Console.WriteLine($"Date: {Convert.ToDateTime(reader["AppointmentDate"]).ToShortDateString()}");
                            Console.WriteLine($"Time: {reader["Time"]}");
                            Console.WriteLine($"Reason: {reader["Reason"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching the appointment details: " + ex.Message);
            }

            
        }

        // Cancel Button Click
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
