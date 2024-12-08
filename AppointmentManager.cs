using System;
using g7_Clinic_Management;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public static class AppointmentManager
    {
        private static string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";

        // Appointment Related Options
        public static void AppointmentMenu()
        {
            Console.Clear();
            Console.WriteLine("Appointment Management");
            Console.WriteLine("Press 1 to View Appointments");
            Console.WriteLine("Press 2 to Add Appointment");
            Console.WriteLine("Press 3 to Update Appointment");
            Console.WriteLine("Press 4 to Delete Appointment");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAppointments();
                    break;
                case "2":
                    OpenAddAppointmentForm();
                    break;
                case "3":
                    UpdateAppointment();
                    break;
                case "4":
                    DeleteAppointment();
                    break;
                case "0":
                    Program.ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Appointment menu.");
                    Console.ReadKey();
                    AppointmentMenu();
                    break;
            }
        }

        // View Appointments
        static void ViewAppointments()
        {
            string query = @"
SELECT a.AppointmentID, a.AppointmentDate, a.Time, 
       p.Name AS PatientName, d.Name AS DoctorName, a.Reason
FROM Appointment a
INNER JOIN Patient p ON a.PatientID = p.PatientID
INNER JOIN Doctor d ON a.DoctorID = d.DoctorID;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Appointments ---");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Appointment ID: {reader["AppointmentID"]}, Date: {Convert.ToDateTime(reader["AppointmentDate"]).ToShortDateString()}, Time: {reader["Time"]}, Patient: {reader["PatientName"]}, Doctor: {reader["DoctorName"]}, Reason: {reader["Reason"]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No appointments found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Appointment Menu.");
            Console.ReadKey();
            AppointmentMenu();
        }

        // Open Add Appointment Form
        static void OpenAddAppointmentForm()
        {
            try
            {
                // Simply show the AddAppointmentForm without reinitializing Application settings
                AddAppointmentForm addAppointmentForm = new AddAppointmentForm();
                addAppointmentForm.ShowDialog(); // Show the form as a dialog
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while opening the form: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Appointment Menu.");
            Console.ReadKey();
            AppointmentMenu(); // Return to Appointment Menu
        }


        // Update Appointment
        static void UpdateAppointment()
        {
            Console.Write("Enter the Patient Name to search appointments: ");
            string patientName = Console.ReadLine();

            // Query to fetch patient details
            string fetchPatientsQuery = @"
SELECT PatientID, Name, PhoneNumber, Address 
FROM Patient 
WHERE Name LIKE @PatientName;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch patients matching the name
                    using (MySqlCommand cmd = new MySqlCommand(fetchPatientsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientName", "%" + patientName + "%");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Matching Patients ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Patient ID: {reader["PatientID"]}, Name: {reader["Name"]}, Phone: {reader["PhoneNumber"]}, Address: {reader["Address"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No matching patients found.");
                                Console.WriteLine("\nPress any key to return to Appointment Menu.");
                                Console.ReadKey();
                                AppointmentMenu();
                                return;
                            }
                        }
                    }

                    Console.Write("\nEnter Patient ID to view appointments: ");
                    if (int.TryParse(Console.ReadLine(), out int patientId))
                    {
                        string fetchAppointmentsQuery = @"
SELECT AppointmentID, AppointmentDate, Time, Reason 
FROM Appointment 
WHERE PatientID = @PatientID;";

                        using (MySqlCommand cmd = new MySqlCommand(fetchAppointmentsQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                Console.WriteLine("\n--- Appointments for Patient ---");
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"Appointment ID: {reader["AppointmentID"]}, Date: {Convert.ToDateTime(reader["AppointmentDate"]).ToShortDateString()}, Time: {reader["Time"]}, Reason: {reader["Reason"]}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No appointments found for the specified patient.");
                                    Console.WriteLine("\nPress any key to return to Appointment Menu.");
                                    Console.ReadKey();
                                    AppointmentMenu();
                                    return;
                                }
                            }
                        }

                        Console.Write("\nEnter Appointment ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int appointmentId))
                        {
                            // Open UpdateAppointmentForm
                            try
                            {
                                UpdateAppointmentForm updateAppointmentForm = new UpdateAppointmentForm(appointmentId);
                                updateAppointmentForm.ShowDialog();
                                Console.WriteLine("Appointment updated successfully.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred while opening the update form: " + ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Appointment ID.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Patient ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Appointment Menu.");
            Console.ReadKey();
            AppointmentMenu();
        }

        // Delete Appointment
        static void DeleteAppointment()
        {
            Console.Write("Enter the Patient Name to search appointments: ");
            string patientName = Console.ReadLine();

            string fetchPatientsQuery = @"
SELECT PatientID, Name, PhoneNumber, Address 
FROM Patient 
WHERE Name LIKE @PatientName;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch patients matching the name
                    using (MySqlCommand cmd = new MySqlCommand(fetchPatientsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientName", "%" + patientName + "%");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Matching Patients ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Patient ID: {reader["PatientID"]}, Name: {reader["Name"]}, Phone: {reader["PhoneNumber"]}, Address: {reader["Address"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No matching patients found.");
                                Console.WriteLine("\nPress any key to return to Appointment Menu.");
                                Console.ReadKey();
                                AppointmentMenu();
                                return;
                            }
                        }
                    }

                    Console.Write("\nEnter Patient ID to view appointments: ");
                    if (int.TryParse(Console.ReadLine(), out int patientId))
                    {
                        string fetchAppointmentsQuery = @"
SELECT AppointmentID, AppointmentDate, Time, Reason 
FROM Appointment 
WHERE PatientID = @PatientID;";

                        using (MySqlCommand cmd = new MySqlCommand(fetchAppointmentsQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                Console.WriteLine("\n--- Appointments for Patient ---");
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"Appointment ID: {reader["AppointmentID"]}, Date: {Convert.ToDateTime(reader["AppointmentDate"]).ToShortDateString()}, Time: {reader["Time"]}, Reason: {reader["Reason"]}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No appointments found for the specified patient.");
                                    Console.WriteLine("\nPress any key to return to Appointment Menu.");
                                    Console.ReadKey();
                                    AppointmentMenu();
                                    return;
                                }
                            }
                        }

                        Console.Write("\nEnter Appointment ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int appointmentId))
                        {
                            string deleteAppointmentQuery = "DELETE FROM Appointment WHERE AppointmentID = @AppointmentID";

                            using (MySqlCommand cmd = new MySqlCommand(deleteAppointmentQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("Appointment deleted successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Failed to delete the appointment. Please check the Appointment ID.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Appointment ID.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Patient ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Appointment Menu.");
            Console.ReadKey();
            AppointmentMenu();
        }
    }
}
