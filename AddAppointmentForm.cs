using System;
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
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBoxPatientName.Items.Add(new ComboBoxItem(reader["Name"].ToString(), reader["PatientID"].ToString()));
                            }
                        }
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
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBoxDoctorName.Items.Add(new ComboBoxItem(reader["Name"].ToString(), reader["DoctorID"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading doctor names: " + ex.Message);
            }
        }

        private void ButtonSubmit_Click(object sender, EventArgs e)
        {
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
                        MessageBox.Show("Appointment added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                this.Close(); // Close the form after adding the appointment
                Console.WriteLine("\nPress any key to go to the main menu.");
                Console.ReadKey(); // Wait for the user to press a key
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the appointment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

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
