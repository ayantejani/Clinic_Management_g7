using System;
using g7_Clinic_Management;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public static class PatientManager
    {
        private static string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";


        // Patient Related-Menu Options
        public static void PatientMenu()
        {
            Console.Clear();
            Console.WriteLine("Patient Management");
            Console.WriteLine("Press 1 to View Patients");
            Console.WriteLine("Press 2 to Add Patient");
            Console.WriteLine("Press 3 to Update Patient");
            Console.WriteLine("Press 4 to Delete Patient");
            Console.WriteLine("Press 0 to Return to Main Menu");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewPatients();
                    break;
                case "2":
                    OpenAddPatientForm();  // Opens Add Patient Form
                    break;
                case "3":
                    UpdatePatientMenu();  // Opens Update Patient Menu
                    break;
                case "4":
                    DeletePatientMenu();  // Opens Delete Patient Menu
                    break;
                case "0":
                    Program.ShowMainMenu();  // Returns to Main Menu
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Patient menu.");
                    break;
            }
        }

        // Update Patient Menu
        static void UpdatePatientMenu()
        {
            Console.Write("\nEnter Patient Name to Search: ");
            string patientName = Console.ReadLine();

            string searchQuery = "SELECT PatientID, Name, DateOfBirth, PhoneNumber, Address FROM Patient WHERE Name LIKE @Name";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(searchQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + patientName + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("\n--- Patients Matching the Name ---");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Patient ID: {reader["PatientID"]}, Name: {reader["Name"]}, " +
                                                      $"Date of Birth: {Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString()}, " +
                                                      $"Phone: {reader["PhoneNumber"]}, Address: {reader["Address"]}");
                                }

                                Console.Write("\nEnter the Patient ID to Update: ");
                                string input = Console.ReadLine();

                                if (int.TryParse(input, out int patientId))
                                {
                                    OpenUpdatePatientForm(patientId); // Proceed to open the update form
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Patient ID. Returning to Patient Menu.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No patients found matching the given name.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Patient Menu.");
            Console.ReadKey();
            PatientMenu(); // Return to Patient Menu
        }

        // Open Update Patient Form
        static void OpenUpdatePatientForm(int patientId)
        {
            try
            {
                using (var form = new UpdatePatientForm(patientId))
                {
                    form.ShowDialog(); // Open the form as a dialog
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while opening the update form: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Patient Menu.");
            Console.ReadKey();
            PatientMenu(); // Return to Patient Menu
        }


        // Delete Patient Menu
        static void DeletePatientMenu()
        {
            Console.Write("\nEnter Patient Name to Search: ");
            string patientName = Console.ReadLine();

            string fetchPatientsQuery = "SELECT PatientID, Name, DateOfBirth, PhoneNumber, Address FROM Patient WHERE Name LIKE @Name";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch patients matching the name
                    using (MySqlCommand cmd = new MySqlCommand(fetchPatientsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + patientName + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Matching Patients ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Patient ID: {reader["PatientID"]}, Name: {reader["Name"]}, DOB: {Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString()}, Phone: {reader["PhoneNumber"]}, Address: {reader["Address"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No patients found with the given name.");
                                Console.WriteLine("\nPress any key to return to Patient Menu.");
                                Console.ReadKey();
                                PatientMenu();
                                return;
                            }
                        }
                    }

                    // Ask for Patient ID to delete
                    Console.Write("\nEnter Patient ID to Delete: ");
                    if (int.TryParse(Console.ReadLine(), out int patientId))
                    {
                        // Fetch and display patient details
                        DisplayPatientFullDetails(patientId);

                        // Confirm deletion
                        Console.WriteLine("\nDo you want to delete this patient? (Y/N): ");
                        string confirmation = Console.ReadLine();
                        if (confirmation?.ToUpper() == "Y")
                        {
                            // Delete related records and patient
                            DeletePatientAndRecords(patientId, connection);
                            Console.WriteLine("\nPatient and all associated records deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Deletion cancelled. Returning to Patient Menu.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Patient ID. Returning to Patient Menu.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Patient Menu.");
            Console.ReadKey();
            PatientMenu();
        }

        // Display Patient Details with associated records
        static void DisplayPatientFullDetails(int patientId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch and display patient details
                    string patientQuery = "SELECT * FROM Patient WHERE PatientID = @PatientID";
                    using (MySqlCommand cmd = new MySqlCommand(patientQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                Console.WriteLine("\n--- Patient Details ---");
                                Console.WriteLine($"Name: {reader["Name"]}, DOB: {Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString()}, Phone: {reader["PhoneNumber"]}, Address: {reader["Address"]}");
                            }
                            else
                            {
                                Console.WriteLine("Patient not found.");
                                return;
                            }
                        }
                    }

                    // Display Appointments
                    string appointmentQuery = "SELECT * FROM Appointment WHERE PatientID = @PatientID";
                    Console.WriteLine("\n--- Appointments ---");
                    DisplayAssociatedRecords(connection, appointmentQuery, patientId);

                    // Display Prescriptions
                    string prescriptionQuery = @"
                SELECT p.PrescriptionID, p.MedicationName, p.Dosage, p.Duration 
                FROM Prescription p
                INNER JOIN Appointment a ON p.AppointmentID = a.AppointmentID
                WHERE a.PatientID = @PatientID";
                    Console.WriteLine("\n--- Prescriptions ---");
                    DisplayAssociatedRecords(connection, prescriptionQuery, patientId);

                    // Display Billing Details
                    string billingQuery = @"
                SELECT b.BillingID, b.Amount, b.PaymentStatus, b.PaymentDate, b.ModeOfPayment 
                FROM Billing_Details b
                INNER JOIN Appointment a ON b.AppointmentID = a.AppointmentID
                WHERE a.PatientID = @PatientID";
                    Console.WriteLine("\n--- Billing Details ---");
                    DisplayAssociatedRecords(connection, billingQuery, patientId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching patient details: " + ex.Message);
            }
        }

        // Display Associated Records (Appointments, Prescriptions, Billing)
        public static void DisplayAssociatedRecords(MySqlConnection connection, string query, int patientId)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write($"{reader.GetName(i)}: {reader[i]}  ");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No records found.");
                    }
                }
            }
        }

        // Delete Patient and Associated Records
        static void DeletePatientAndRecords(int patientId, MySqlConnection connection)
        {
            string deleteAppointmentsQuery = "DELETE FROM Appointment WHERE PatientID = @PatientID";
            string deletePrescriptionsQuery = @"
        DELETE p FROM Prescription p
        INNER JOIN Appointment a ON p.AppointmentID = a.AppointmentID
        WHERE a.PatientID = @PatientID";
            string deleteBillingQuery = @"
        DELETE b FROM Billing_Details b
        INNER JOIN Appointment a ON b.AppointmentID = a.AppointmentID
        WHERE a.PatientID = @PatientID";
            string deletePatientQuery = "DELETE FROM Patient WHERE PatientID = @PatientID";

            // Delete related records
            ExecuteDeleteQuery(connection, deletePrescriptionsQuery, patientId);
            ExecuteDeleteQuery(connection, deleteBillingQuery, patientId);
            ExecuteDeleteQuery(connection, deleteAppointmentsQuery, patientId);

            // Delete the patient
            ExecuteDeleteQuery(connection, deletePatientQuery, patientId);
        }

        // Execute Delete Query
        static void ExecuteDeleteQuery(MySqlConnection connection, string query, int patientId)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PatientID", patientId);
                cmd.ExecuteNonQuery();
            }
        }

        // Open Add Patient Form
        static void OpenAddPatientForm()
        {
            // Create an instance of AddPatientForm
            using (var addPatientForm = new AddPatientForm())
            {
                // Show the form as a modal dialog
                addPatientForm.ShowDialog();
            }

            Console.WriteLine("\nPress any key to return to Patient Menu.");
            Console.ReadKey();

            // Return to the Patient Menu
            PatientMenu();
        }


        // Viewing patients
        static void ViewPatients()
        {
            string query = "SELECT * FROM Patient";  // Fetch all patients
            ExecuteQuery(query);  // Executes the query and displays the results
            Console.WriteLine("\nPress any key to return to Patient menu.");
            Console.ReadKey();
            PatientMenu();  // Returns to Patient menu
        }
        // Execute SQL Query and Display Results
        static void ExecuteQuery(string query)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader.GetName(i)}: {reader[i]} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }
        }
    }
}
