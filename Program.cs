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

        static void Main(string[] args)
        {
            // Initialize visual styles and text rendering before any window or form is created
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the main form or menu, ensuring that no window is created before this point
            ShowMainMenu();  // Or Application.Run(new MainForm()), depending on your app's flow
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
            Console.WriteLine("Press 1 to Search Patient by Last Name");
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
                        return;
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
                    string patientQuery = "SELECT * FROM Patient WHERE PatientID = @PatientID;";
                    using (MySqlCommand patientCmd = new MySqlCommand(patientQuery, connection))
                    {
                        patientCmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = patientCmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                Console.WriteLine("\n--- Patient Details ---");
                                Console.WriteLine($"Patient ID: {reader["PatientID"]}");
                                Console.WriteLine($"Name: {DBNullSafe(reader["Name"])}");
                                Console.WriteLine($"Date of Birth: {DBNullSafe(reader["DateOfBirth"], true)}");
                                Console.WriteLine($"Phone: {DBNullSafe(reader["PhoneNumber"])}");
                                Console.WriteLine($"Address: {DBNullSafe(reader["Address"])}");
                            }
                            else
                            {
                                Console.WriteLine("Patient not found.");
                                return;
                            }
                        }
                    }

                    // Fetch and display appointments
                    string appointmentQuery = "SELECT * FROM Appointment WHERE PatientID = @PatientID;";
                    using (MySqlCommand appointmentCmd = new MySqlCommand(appointmentQuery, connection))
                    {
                        appointmentCmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = appointmentCmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Appointments ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Appointment ID: {DBNullSafe(reader["AppointmentID"])}, Date: {DBNullSafe(reader["AppointmentDate"], true)}, Time: {DBNullSafe(reader["Time"])}, Reason: {DBNullSafe(reader["Reason"])}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No appointments found.");
                            }
                        }
                    }

                    // Fetch and display prescriptions
                    string prescriptionQuery = @"
            SELECT p.MedicationName, p.Dosage, p.Duration 
            FROM Prescription p
            INNER JOIN Appointment a ON p.AppointmentID = a.AppointmentID
            WHERE a.PatientID = @PatientID;";
                    using (MySqlCommand prescriptionCmd = new MySqlCommand(prescriptionQuery, connection))
                    {
                        prescriptionCmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = prescriptionCmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Prescriptions ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Medication: {DBNullSafe(reader["MedicationName"])}, Dosage: {DBNullSafe(reader["Dosage"])}, Duration: {DBNullSafe(reader["Duration"])}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No prescriptions found.");
                            }
                        }
                    }

                    // Fetch and display billing details
                    string billingQuery = @"
            SELECT b.Amount, b.PaymentStatus, b.PaymentDate, b.ModeOfPayment 
            FROM Billing_Details b
            INNER JOIN Appointment a ON b.AppointmentID = a.AppointmentID
            WHERE a.PatientID = @PatientID;";
                    using (MySqlCommand billingCmd = new MySqlCommand(billingQuery, connection))
                    {
                        billingCmd.Parameters.AddWithValue("@PatientID", patientId);
                        using (MySqlDataReader reader = billingCmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Billing Details ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Amount: {DBNullSafe(reader["Amount"])}, Status: {DBNullSafe(reader["PaymentStatus"])}, Date: {DBNullSafe(reader["PaymentDate"], true)}, Mode: {DBNullSafe(reader["ModeOfPayment"])}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No billing records found.");
                            }
                        }
                    }
                }

                // Add the prompt to return to the main menu
                Console.WriteLine("\nPress any key to return to the Main Menu.");
                Console.ReadKey();
                ShowMainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("\nPress any key to return to the Main Menu.");
                Console.ReadKey();
                ShowMainMenu();
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
            Console.Write("\nEnter Patient ID to Update: ");
            string patientId = Console.ReadLine();
            if (int.TryParse(patientId, out int id))
            {
                OpenUpdatePatientForm(id);  // Opens the UpdatePatientForm with the given Patient ID
            }
            else
            {
                Console.WriteLine("Invalid Patient ID. Returning to Patient menu.");
                Console.WriteLine("\nPress any key to return to the Patient menu.");
                Console.ReadKey();
                PatientMenu();  // Return to Patient menu if invalid input
            }
        }

        // Open Update Patient Form
        static void OpenUpdatePatientForm(int patientId)
        {
            using (var form = new UpdatePatientForm(patientId))  // Open the UpdatePatientForm as a modal dialog
            {
                form.ShowDialog();  // Wait until the form is closed
            }

            // After closing the form, prompt the user to press any key to return to the main menu
            Console.WriteLine("\nPatient information updated. Press any key to return to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();  // Return to Main Menu after closing the form
        }

        // Delete Patient Menu
        static void DeletePatientMenu()
        {
            Console.Write("\nEnter Patient ID to Delete: ");
            string patientId = Console.ReadLine();
            if (int.TryParse(patientId, out int id))
            {
                // Fetch and display patient details before deletion
                string patientQuery = "SELECT * FROM Patient WHERE PatientID = @PatientID";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(patientQuery, connection);
                    cmd.Parameters.AddWithValue("@PatientID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Read the patient's details

                            // Display patient details
                            string name = reader["Name"].ToString();
                            string dob = Convert.ToDateTime(reader["DateOfBirth"]).ToShortDateString();
                            string phone = reader["PhoneNumber"].ToString();
                            string address = reader["Address"].ToString();

                            Console.WriteLine("\n--- Patient Details ---");
                            Console.WriteLine($"Patient ID: {id}");
                            Console.WriteLine($"Name: {name}");
                            Console.WriteLine($"Date of Birth: {dob}");
                            Console.WriteLine($"Phone: {phone}");
                            Console.WriteLine($"Address: {address}");

                            // Ask for confirmation to delete
                            Console.WriteLine($"\nAre you sure you want to delete this patient? (Y/N): ");
                            string confirmation = Console.ReadLine();
                            if (confirmation?.ToUpper() == "Y")
                            {
                                DeletePatient(id);  // Proceed with deletion
                            }
                            else
                            {
                                Console.WriteLine("Patient deletion cancelled. Returning to Patient menu.");
                                PatientMenu();  // Go back to Patient menu if cancellation
                            }
                        }
                        else
                        {
                            Console.WriteLine("Patient not found.");
                            PatientMenu();  // Return to Patient menu if no patient is found
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Patient ID. Returning to Patient menu.");
                PatientMenu();  // Return to Patient menu if invalid input
            }
        }

        // Delete a Patient from the database
        static void DeletePatient(int patientId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Step 1: Delete related billing details
                    string deleteBillingQuery = @"
                DELETE FROM Billing_Details 
                WHERE AppointmentID IN (
                    SELECT AppointmentID 
                    FROM Appointment 
                    WHERE PatientID = @PatientID
                )";
                    using (MySqlCommand cmd = new MySqlCommand(deleteBillingQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 2: Delete related prescriptions
                    string deletePrescriptionsQuery = @"
                DELETE FROM Prescription 
                WHERE AppointmentID IN (
                    SELECT AppointmentID 
                    FROM Appointment 
                    WHERE PatientID = @PatientID
                )";
                    using (MySqlCommand cmd = new MySqlCommand(deletePrescriptionsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 3: Delete related appointments
                    string deleteAppointmentsQuery = "DELETE FROM Appointment WHERE PatientID = @PatientID";
                    using (MySqlCommand cmd = new MySqlCommand(deleteAppointmentsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 4: Delete the patient record
                    string deletePatientQuery = "DELETE FROM Patient WHERE PatientID = @PatientID";
                    using (MySqlCommand cmd = new MySqlCommand(deletePatientQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nPatient deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Error: Unable to delete patient.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Patient menu.");
            Console.ReadKey();
            PatientMenu();  // Return to Patient menu after deletion
        }

        // Open Add Patient Form
        static void OpenAddPatientForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AddPatientForm());  // Opens the AddPatientForm
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
            Console.Write("\nEnter Doctor ID to Delete: ");
            string doctorIdInput = Console.ReadLine();

            if (int.TryParse(doctorIdInput, out int doctorId))
            {
                // Display doctor details before deletion
                string query = $"SELECT * FROM Doctor WHERE DoctorID = {doctorId}";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read(); // Read the doctor details

                        // Display doctor details
                        string name = reader["Name"].ToString();
                        string specialty = reader["Specialty"].ToString();
                        string phone = reader["PhoneNumber"].ToString();

                        Console.WriteLine("\nDoctor Details:");
                        Console.WriteLine($"Name: {name}");
                        Console.WriteLine($"Specialty: {specialty}");
                        Console.WriteLine($"Phone: {phone}");

                        // Ask for confirmation to delete
                        Console.WriteLine("\nAre you sure you want to delete this doctor?");
                        Console.WriteLine("Press 1 for Yes, Press 2 to Go Back");

                        string confirmChoice = Console.ReadLine();

                        if (confirmChoice == "1")
                        {
                            DeleteDoctor(doctorId);  // Proceed with the deletion
                        }
                        else if (confirmChoice == "2")
                        {
                            Console.WriteLine("Doctor deletion cancelled. Returning to Doctor menu.");
                            Console.ReadKey();
                            DoctorMenu();  // Go back to the Doctor Menu
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Returning to Doctor menu.");
                            Console.ReadKey();
                            DoctorMenu();  // Go back to the Doctor Menu
                        }
                    }
                    else
                    {
                        Console.WriteLine("Doctor not found. Returning to Doctor menu.");
                        Console.ReadKey();
                        DoctorMenu();  // Go back to the Doctor Menu
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Doctor ID. Returning to Doctor menu.");
                Console.ReadKey();
                DoctorMenu();  // Return to Doctor menu if invalid input
            }
        }

        // Delete Doctors
        static void DeleteDoctor(int doctorId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Step 1: Delete related records from `prescription`
                    string deletePrescriptionsQuery = @"
                DELETE p
                FROM Prescription p
                INNER JOIN Appointment a ON p.AppointmentID = a.AppointmentID
                WHERE a.DoctorID = @DoctorID";
                    using (MySqlCommand prescriptionCmd = new MySqlCommand(deletePrescriptionsQuery, connection))
                    {
                        prescriptionCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        prescriptionCmd.ExecuteNonQuery();
                    }

                    // Step 2: Delete related records from `billing_details`
                    string deleteBillingQuery = @"
                DELETE b
                FROM Billing_Details b
                INNER JOIN Appointment a ON b.AppointmentID = a.AppointmentID
                WHERE a.DoctorID = @DoctorID";
                    using (MySqlCommand billingCmd = new MySqlCommand(deleteBillingQuery, connection))
                    {
                        billingCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        billingCmd.ExecuteNonQuery();
                    }

                    // Step 3: Delete related records from `appointment`
                    string deleteAppointmentsQuery = "DELETE FROM Appointment WHERE DoctorID = @DoctorID";
                    using (MySqlCommand appointmentCmd = new MySqlCommand(deleteAppointmentsQuery, connection))
                    {
                        appointmentCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        appointmentCmd.ExecuteNonQuery();
                    }

                    // Step 4: Delete the doctor record
                    string deleteDoctorQuery = "DELETE FROM Doctor WHERE DoctorID = @DoctorID";
                    using (MySqlCommand doctorCmd = new MySqlCommand(deleteDoctorQuery, connection))
                    {
                        doctorCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                        int rowsAffected = doctorCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nDoctor and all related appointments, billing details, and prescriptions have been deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("\nError: Unable to delete the doctor.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to the Doctor menu.");
            Console.ReadKey();
            DoctorMenu(); // Return to Doctor menu after deletion
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
        // View Appointment menu 
        static void AppointmentMenu()
        {
            Console.Clear();
            Console.WriteLine("Appointment Management");
            Console.WriteLine("Press 1 to View Appointments");
            Console.WriteLine("Press 2 to Add Appointment");
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
            string query = "SELECT * FROM Billing_Details";

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
                                Console.WriteLine($"Billing ID: {reader["BillingID"]}, Amount: {reader["Amount"]}, Status: {reader["PaymentStatus"]}, Date: {Convert.ToDateTime(reader["PaymentDate"]).ToShortDateString()}, Mode: {reader["ModeOfPayment"]}");
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
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to the Billing Menu.");
            Console.ReadKey();
            BillingMenu();
        }


        

    }
}
