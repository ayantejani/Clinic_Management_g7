using System;
using group7_Clinic_Management;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using g7_Clinic_Management;

namespace g7_Clinic_Management
{
    class Program
    {
        private static bool exitApplication = false;
        private static string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    ShowMainMenu(); // Proceed to the main menu
                }
                else
                {
                    MessageBox.Show("Application will now exit.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit(); // Exit if login is canceled
                }
            }
        }



        public static void DisplayChanges(string action, string recordDetails)
        {
            Console.Clear();
            Console.WriteLine($"--- {action} ---");
            Console.WriteLine(recordDetails);

            // Add the prompt to return to the main menu
            Console.WriteLine("\nPress any key to return to the Main Menu.");
            Console.ReadKey(); // Wait for the user to press any key
            ShowMainMenu(); // Redirect to the main menu
        }

       


        // Main Menu
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Clinic Management System\n");
            Console.WriteLine("Press 1 to Search Patient by First/Last Name");
            Console.WriteLine("Press 2 for Patients");
            Console.WriteLine("Press 3 for Doctors");
            Console.WriteLine("Press 4 for Appointments");
            Console.WriteLine("Press 5 for Prescriptions");
            Console.WriteLine("Press 6 for Billings");
            Console.WriteLine("Press 0 to Exit");

            while (true)
            {
                Console.Write("\nEnter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchPatientByLastName(); 
                        break;
                    case "2":
                        PatientMenu();
                        break;
                    case "3":
                        DoctorMenu();
                        break;
                    case "4":
                        AppointmentMenu();
                        break;
                    case "5":
                        PrescriptionMenu(); 
                        break;
                    case "6":
                        BillingMenu(); 
                        break;
                    case "0":
                        Console.WriteLine("Exiting the application. Goodbye!");
                        Console.WriteLine("This Application was created by - \nAyan\nMansi\nAllen\nVamshi\nLasitha");
                        Environment.Exit(0); // Cleanly exits the application
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }


        // Search for a patient by last name
        static void SearchPatientByLastName()
        {
            Console.Clear();
            Console.WriteLine("Search Patient by Last Name");
            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();

            string query = "SELECT * FROM Patient WHERE Name LIKE @LastName;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LastName", "%" + lastName + "%");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("\n--- Patients Matching the Last Name ---");
                                while (reader.Read())
                                {
                                    string patientId = reader["PatientID"].ToString();
                                    string name = reader["Name"].ToString();
                                    string dob = Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString();
                                    string phone = reader["PhoneNumber"].ToString();
                                    string address = reader["Address"].ToString();

                                    Console.WriteLine($"Patient ID: {patientId}, Name: {name}, Date of Birth: {dob}, Phone: {phone}, Address: {address}");
                                }

                                Console.Write("\nEnter Patient ID to view full details or press 'M' to return to the Main Menu: ");
                                string userInput = Console.ReadLine();

                                if (userInput?.ToUpper() == "M")
                                {
                                    ShowMainMenu();
                                }
                                else if (int.TryParse(userInput, out int selectedPatientId))
                                {
                                    DisplayPatientDetails(selectedPatientId);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Returning to Main Menu.");
                                    ShowMainMenu();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No patients found with the specified last name.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            // Prompt to return to Main Menu if no details were accessed
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();
        }


        // Display Patient Details
        static void DisplayPatientDetails(int patientId)
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
                                Console.WriteLine($"Patient ID: {reader["PatientID"]}");
                                Console.WriteLine($"Name: {reader["Name"]}");
                                Console.WriteLine($"Date of Birth: {Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString()}");
                                Console.WriteLine($"Phone: {reader["PhoneNumber"]}");
                                Console.WriteLine($"Address: {reader["Address"]}");
                            }
                            else
                            {
                                Console.WriteLine("Patient not found.");
                                return;
                            }
                        }
                    }

                    // Display Appointments
                    string appointmentQuery = @"
                SELECT a.AppointmentID, a.AppointmentDate, a.Time, a.Reason, d.Name AS DoctorName
                FROM Appointment a
                INNER JOIN Doctor d ON a.DoctorID = d.DoctorID
                WHERE a.PatientID = @PatientID;";
                    Console.WriteLine("\n--- Appointments ---");
                    DisplayAssociatedRecords(connection, appointmentQuery, patientId);

                    // Display Prescriptions
                    string prescriptionQuery = @"
                SELECT p.AppointmentID, p.MedicationName, p.Dosage, p.Duration
                FROM Prescription p
                WHERE p.AppointmentID IN (
                    SELECT a.AppointmentID
                    FROM Appointment a
                    WHERE a.PatientID = @PatientID
                );";
                    Console.WriteLine("\n--- Prescriptions ---");
                    DisplayAssociatedRecords(connection, prescriptionQuery, patientId);

                    // Display Billing Details
                    string billingQuery = @"
                SELECT b.AppointmentID, b.Amount, b.PaymentStatus, b.PaymentDate, b.ModeOfPayment
                FROM Billing_Details b
                WHERE b.AppointmentID IN (
                    SELECT a.AppointmentID
                    FROM Appointment a
                    WHERE a.PatientID = @PatientID
                );";
                    Console.WriteLine("\n--- Billing Details ---");
                    DisplayAssociatedRecords(connection, billingQuery, patientId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching patient details: " + ex.Message);
            }
        }


        // Helper method for DBNull handling
        static string DBNullSafe(object value, bool isDate = false)
        {
            if (value == DBNull.Value)
                return isDate ? "N/A" : "Unknown";
            if (isDate)
                return Convert.ToDateTime(value).ToShortDateString();
            return value.ToString();
        }

        // Patient Related-Menu Options
        static void PatientMenu()
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
                    ShowMainMenu();  // Returns to Main Menu
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
        static void DisplayAssociatedRecords(MySqlConnection connection, string query, int patientId)
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AddPatientForm());  // Opens the AddPatientForm
            Console.WriteLine("\nPress any key to return to Patient Menu.");
            Console.ReadKey();
            PatientMenu(); // Return to Patient Menu
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

        

        // Doctor-related menu options
        static void DoctorMenu()
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
                    ShowMainMenu();  // Return to the Main Menu
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
            Console.Write("\nEnter Doctor ID to Update: ");
            string doctorIdInput = Console.ReadLine();

            if (int.TryParse(doctorIdInput, out int doctorId))
            {
                OpenUpdateDoctorForm(doctorId);  // Open update form with given doctor ID
            }
            else
            {
                Console.WriteLine("Invalid Doctor ID. Returning to Doctor menu.");
                Console.ReadKey();
                DoctorMenu();  // Return to Doctor menu if invalid input
            }
        }

        // Open Update Doctor Form
        static void OpenUpdateDoctorForm(int doctorId)
        {
            // Open the update form passing the doctorId to be updated
            UpdateDoctorForm updateForm = new UpdateDoctorForm(doctorId);
            updateForm.ShowDialog();  // Show the update form dialog

            // After the form closes, prompt to return to Doctor menu
            Console.WriteLine("\nPress any key to return to Doctor menu.");
            Console.ReadKey();
            DoctorMenu();  // Go back to Doctor menu
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


        // Appointment Related Options
        static void AppointmentMenu()
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
                    ShowMainMenu();
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





        // Prescription Menu
        static void PrescriptionMenu()
        {
            Console.Clear();
            Console.WriteLine("Prescription Management");
            Console.WriteLine("Press 1 to View Prescriptions");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewPrescriptions();
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Prescription menu.");
                    PrescriptionMenu();
                    break;
            }
        }

        // View Prescriptions
        static void ViewPrescriptions()
        {
            string query = "SELECT * FROM Prescription";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Prescriptions ---");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Prescription ID: {reader["PrescriptionID"]}, Medication: {reader["MedicationName"]}, Dosage: {reader["Dosage"]}, Duration: {reader["Duration"]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No prescriptions found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to the Prescription Menu.");
            Console.ReadKey();
            PrescriptionMenu();
        }

        // Billing Menu
        static void BillingMenu()
        {
            Console.Clear();
            Console.WriteLine("Billing Management");
            Console.WriteLine("Press 1 to View Billing Records");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewBillingRecords();
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Billing menu.");
                    BillingMenu();
                    break;
            }
        }

        // View Billing Records
        static void ViewBillingRecords()
        {
            string query = @"
        SELECT b.BillingID, b.Amount, b.PaymentStatus, b.PaymentDate, b.ModeOfPayment, a.PatientID, a.DoctorID
        FROM Billing_Details b
        LEFT JOIN Appointment a ON b.AppointmentID = a.AppointmentID;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Billing Records ---");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int billingId = reader["BillingID"] != DBNull.Value ? Convert.ToInt32(reader["BillingID"]) : 0;
                                decimal amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0;
                                string paymentStatus = reader["PaymentStatus"] != DBNull.Value ? reader["PaymentStatus"].ToString() : "N/A";
                                string paymentDate = reader["PaymentDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["PaymentDate"]).ToShortDateString()
                                    : "N/A";
                                string modeOfPayment = reader["ModeOfPayment"] != DBNull.Value ? reader["ModeOfPayment"].ToString() : "N/A";
                                int patientId = reader["PatientID"] != DBNull.Value ? Convert.ToInt32(reader["PatientID"]) : 0;
                                int doctorId = reader["DoctorID"] != DBNull.Value ? Convert.ToInt32(reader["DoctorID"]) : 0;

                                Console.WriteLine($"Billing ID: {billingId}, Amount: {amount:C}, Status: {paymentStatus}, " +
                                                  $"Date: {paymentDate}, Mode: {modeOfPayment}, " +
                                                  $"Patient ID: {patientId}, Doctor ID: {doctorId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No billing records found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the Billing Menu.");
            Console.ReadKey();
            BillingMenu();
        }





    }
}
