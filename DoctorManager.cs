using System;
using g7_Clinic_Management;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public static class DoctorManager
    {
        private static string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";

        // Doctor-related menu options
        public static void DoctorMenu()
        {
            Console.Clear();
            Console.WriteLine("Doctor Management");
            Console.WriteLine("Press 1 to View Doctors");
            Console.WriteLine("Press 2 to Add Doctor");
            Console.WriteLine("Press 3 to Update Doctor");
            Console.WriteLine("Press 4 to Delete Doctor");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewDoctors();  // Call method to view doctors
                    break;
                case "2":
                    OpenAddDoctorForm();  // Call method to open Add Doctor Form
                    break;
                case "3":
                    UpdateDoctorMenu();  // Call method to update doctor
                    break;
                case "4":
                    DeleteDoctorMenu();  // Call method to delete doctor
                    break;
                case "0":
                    Program.ShowMainMenu();  // Return to the Main Menu
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Doctor menu.");
                    Console.ReadKey();
                    DoctorMenu();  // If invalid choice, go back to Doctor menu
                    break;
            }
        }

        // Update Doctor Menu
        static void UpdateDoctorMenu()
        {
            Console.Write("\nEnter Doctor Name to Search: ");
            string doctorName = Console.ReadLine();

            string searchQuery = "SELECT DoctorID, Name, Specialty, PhoneNumber FROM Doctor WHERE Name LIKE @Name";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand(searchQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + doctorName + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("\n--- Doctors Matching the Name ---");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Doctor ID: {reader["DoctorID"]}, Name: {reader["Name"]}, " +
                                                      $"Specialty: {reader["Specialty"]}, " +
                                                      $"Phone: {reader["PhoneNumber"]}");
                                }

                                Console.Write("\nEnter the Doctor ID to Update: ");
                                string input = Console.ReadLine();

                                if (int.TryParse(input, out int doctorId))
                                {
                                    OpenUpdateDoctorForm(doctorId); // Proceed to open the update form
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Doctor ID. Returning to Doctor Menu.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No doctors found matching the given name.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Doctor Menu.");
            Console.ReadKey();
            DoctorMenu(); // Return to Doctor Menu
        }



        // Open Update Doctor Form
        static void OpenUpdateDoctorForm(int doctorId)
        {
            try
            {
                // Using statement ensures proper disposal of the form
                using (var updateForm = new UpdateDoctorForm(doctorId))
                {
                    updateForm.ShowDialog();  // Show the update form dialog
                }

                Console.WriteLine("\nDoctor details updated successfully.");
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur
                Console.WriteLine($"An error occurred while updating the doctor: {ex.Message}");
            }
            finally
            {
                // Ensure return to Doctor menu after handling the form
                Console.WriteLine("\nPress any key to return to the Doctor menu.");
                Console.ReadKey();
                DoctorMenu();
            }
        }


        // Delete Doctor Menu
        static void DeleteDoctorMenu()
        {
            Console.Write("\nEnter Doctor Name to Search: ");
            string doctorName = Console.ReadLine();

            string fetchDoctorsQuery = "SELECT DoctorID, Name, Specialty, PhoneNumber FROM Doctor WHERE Name LIKE @Name";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch doctors matching the name
                    using (MySqlCommand cmd = new MySqlCommand(fetchDoctorsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + doctorName + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Matching Doctors ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Doctor ID: {reader["DoctorID"]}, Name: {reader["Name"]}, Specialty: {reader["Specialty"]}, Phone: {reader["PhoneNumber"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No doctors found with the given name.");
                                Console.WriteLine("\nPress any key to return to Doctor Menu.");
                                Console.ReadKey();
                                DoctorMenu();
                                return;
                            }
                        }
                    }

                    // Ask for Doctor ID to delete
                    Console.Write("\nEnter Doctor ID to Delete: ");
                    if (int.TryParse(Console.ReadLine(), out int doctorId))
                    {
                        // Display doctor details with associated records
                        DisplayDoctorDetails(doctorId);

                        // Confirm deletion
                        Console.WriteLine("\nDo you want to delete this doctor? (Y/N): ");
                        string confirmation = Console.ReadLine();
                        if (confirmation?.ToUpper() == "Y")
                        {
                            // Delete related records and doctor
                            DeleteDoctorAndRecords(doctorId, connection);
                            Console.WriteLine("\nDoctor and all associated records deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Deletion cancelled. Returning to Doctor Menu.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Doctor ID. Returning to Doctor Menu.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Doctor Menu.");
            Console.ReadKey();
            DoctorMenu();
        }

        // Display Doctor Details with associated records
        static void DisplayDoctorDetails(int doctorId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch and display doctor details
                    string doctorQuery = "SELECT * FROM Doctor WHERE DoctorID = @DoctorID";
                    using (MySqlCommand cmd = new MySqlCommand(doctorQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                Console.WriteLine("\n--- Doctor Details ---");
                                Console.WriteLine($"Name: {reader["Name"]}, Specialty: {reader["Specialty"]}, Phone: {reader["PhoneNumber"]}");
                            }
                            else
                            {
                                Console.WriteLine("Doctor not found.");
                                return;
                            }
                        }
                    }

                    // Display Appointments
                    string appointmentQuery = "SELECT * FROM Appointment WHERE DoctorID = @DoctorID";
                    Console.WriteLine("\n--- Appointments ---");
                    DisplayAssociatedDoctorRecords(connection, appointmentQuery, doctorId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching doctor details: {ex.Message}");
            }
        }

        // Delete Doctor and Associated Records
        static void DeleteDoctorAndRecords(int doctorId, MySqlConnection connection)
        {
            // Queries to delete associated records
            string deleteBillingQuery = @"
        DELETE b
        FROM Billing_Details b
        INNER JOIN Appointment a ON b.AppointmentID = a.AppointmentID
        WHERE a.DoctorID = @DoctorID";

            string deletePrescriptionsQuery = @"
        DELETE p
        FROM Prescription p
        INNER JOIN Appointment a ON p.AppointmentID = a.AppointmentID
        WHERE a.DoctorID = @DoctorID";

            string deleteAppointmentsQuery = "DELETE FROM Appointment WHERE DoctorID = @DoctorID";
            string deleteDoctorQuery = "DELETE FROM Doctor WHERE DoctorID = @DoctorID";

            try
            {
                // Delete billing records
                ExecuteDeleteDoctorQuery(connection, deleteBillingQuery, doctorId);

                // Delete prescriptions
                ExecuteDeleteDoctorQuery(connection, deletePrescriptionsQuery, doctorId);

                // Delete appointments
                ExecuteDeleteDoctorQuery(connection, deleteAppointmentsQuery, doctorId);

                // Delete doctor
                ExecuteDeleteDoctorQuery(connection, deleteDoctorQuery, doctorId);

                Console.WriteLine("\nDoctor and all associated records deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting records: {ex.Message}");
            }
        }

        // Execute Delete Query
        static void ExecuteDeleteDoctorQuery(MySqlConnection connection, string query, int doctorId)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.ExecuteNonQuery();
            }
        }


        // Display Associated Records (Appointments)
        static void DisplayAssociatedDoctorRecords(MySqlConnection connection, string query, int doctorId)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching associated records: {ex.Message}");
            }
        }



        // Open Add Doctor Form
        static void OpenAddDoctorForm()
        {
            // Open the Add Doctor Form
            AddDoctorForm addForm = new AddDoctorForm();
            addForm.ShowDialog();  // Show the Add Doctor form dialog

            // After the form closes, prompt to return to Doctor menu
            Console.WriteLine("\nPress any key to return to Doctor menu.");
            Console.ReadKey();
            DoctorMenu();  // Return to Doctor menu after adding a doctor
        }

        // View Doctors
        static void ViewDoctors()
        {
            // Query to select all doctors
            string query = "SELECT * FROM Doctor";
            ExecuteQuery(query);  // Executes the query and displays the results

            // Prompt to return to Doctor menu after viewing the doctors
            Console.WriteLine("\nPress any key to return to Doctor menu.");
            Console.ReadKey();
            DoctorMenu();  // Return to Doctor menu after viewing the doctors
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
