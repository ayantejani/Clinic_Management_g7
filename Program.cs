using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using group7_Clinic_Management;


namespace g7_Clinic_Management
{
    class Program
    {
        private static bool exitApplication = false;
        private static string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ShowMainMenu();
        }

        // Main menu with options
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



        // Search for patient by last name
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

            // Prompt to return to Main Menu
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();
        }

        // Function to display patient details along with their appointments, prescriptions, and billing information
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
                                    Console.WriteLine($"Appointment ID: {reader["AppointmentID"]}, Date: {Convert.ToDateTime(reader["AppointmentDate"]).ToShortDateString()}, Time: {reader["Time"]}, Reason: {reader["Reason"]}");
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
                                    Console.WriteLine($"Medication: {reader["MedicationName"]}, Dosage: {reader["Dosage"]}, Duration: {reader["Duration"]}");
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
                                    Console.WriteLine($"Amount: {reader["Amount"]}, Status: {reader["PaymentStatus"]}, Date: {Convert.ToDateTime(reader["PaymentDate"]).ToShortDateString()}, Mode: {reader["ModeOfPayment"]}");
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
                Console.WriteLine("\nPress 1 to go back to the Main Menu.");
                string userChoice = Console.ReadLine();
                if (userChoice == "1")
                {
                    ShowMainMenu(); // Return to the main menu
                }
                else
                {
                    Console.WriteLine("Invalid choice. Returning to Main Menu.");
                    ShowMainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        // Patient-related menu options
        static void PatientMenu()
        {
            Console.Clear();
            Console.WriteLine("Patient Management");
            Console.WriteLine("Press 1 to View Patients");
            Console.WriteLine("Press 2 to Add Patient (Under construction)");
            Console.WriteLine("Press 3 to Update Patient (Under construction)");
            Console.WriteLine("Press 4 to Delete Patient (Under construction)");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewPatients();
                    break;
                case "2":
                    Console.WriteLine("Add Patient is under construction.");
                    break;
                case "3":
                    Console.WriteLine("Update Patient is under construction.");
                    break;
                case "4":
                    Console.WriteLine("Delete Patient is under construction.");
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Patient menu.");
                    break;
            }
        }

        // Viewing patients
        
        static void ViewPatients()
        {
            string query = "SELECT * FROM Patient";
            ExecuteQuery(query);

            // Display message to go back to the main menu
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey(); // Wait for any key press
            ShowMainMenu(); // Redirect to Main Menu
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
                    ViewDoctors();
                    break;
                case "2":
                    OpenAddDoctorForm();
                    break;
                case "3":
                    UpdateDoctorMenu();
                    break;
                case "4":
                    DeleteDoctorMenu();
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Doctor menu.");
                    DoctorMenu();
                    break;
            }
        }
        static void UpdateDoctorMenu()
        {
            Console.Write("\nEnter Doctor ID to Update: ");
            string doctorIdInput = Console.ReadLine();

            if (int.TryParse(doctorIdInput, out int doctorId))
            {
                OpenUpdateDoctorForm(doctorId);
            }
            else
            {
                Console.WriteLine("Invalid Doctor ID. Returning to Doctor menu.");
                DoctorMenu();
            }
        }

        static void OpenUpdateDoctorForm(int doctorId)
        {
            UpdateDoctorForm updateForm = new UpdateDoctorForm(doctorId);
            updateForm.ShowDialog();
        }


        // Delete Doctor Menu
        static void DeleteDoctorMenu()
        {
            Console.Write("\nEnter Doctor ID to Delete: ");
            string doctorIdInput = Console.ReadLine();

            if (int.TryParse(doctorIdInput, out int doctorId))
            {
                // Display doctor details before asking for confirmation
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
                            // Proceed with the deletion
                            DeleteDoctor(doctorId);
                        }
                        else if (confirmChoice == "2")
                        {
                            // Go back to the Doctor Menu
                            DoctorMenu();
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Returning to Doctor menu.");
                            DoctorMenu();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Doctor not found. Returning to Doctor menu.");
                        DoctorMenu();
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Doctor ID. Returning to Doctor menu.");
                DoctorMenu();
            }
        }

        // Delete Doctors
        static void DeleteDoctor(int doctorId)
        {
            try
            {
                // Delete the doctor from the database
                string deleteQuery = "DELETE FROM Doctor WHERE DoctorID = @DoctorID";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(deleteQuery, connection);
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("\nDoctor deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error: Unable to delete doctor.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to return to Doctor menu.");
            Console.ReadKey();
            DoctorMenu();
        }

        static void OpenAddDoctorForm()
        {
            AddDoctorForm addForm = new AddDoctorForm();
            addForm.ShowDialog();
        }

        // View Doctors
        static void ViewDoctors()
        {
            string query = "SELECT * FROM Doctor";
            ExecuteQuery(query);

            // Prompt to return to Main Menu
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();
        }


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
                    AppointmentMenu();
                    break;
            }
        }


        // Open Add Appointment Form
        static void OpenAddAppointmentForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AddAppointmentForm());
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



        // View Prescription menu (only View option enabled)
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
                    break;
            }
        }

        // View Prescriptions
        static void ViewPrescriptions()
        {
            string query = "SELECT * FROM Prescription";
            ExecuteQuery(query);

            // Prompt to return to Main Menu
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();
        }


        // View Billing menu (only View option enabled)
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
                    break;
            }
        }

        // View Billing Records
        static void ViewBillingRecords()
        {
            string query = "SELECT * FROM Billing_Details";
            ExecuteQuery(query);

            // Prompt to return to Main Menu
            Console.WriteLine("\nPress any key to go to the Main Menu.");
            Console.ReadKey();
            ShowMainMenu();
        }

        public static void DisplayChanges(string action, string recordDetails)
        {
            Console.WriteLine($"\n{action} completed successfully.");
            Console.WriteLine($"Changes made: {recordDetails}");

            // Ask the user to return to the main menu
            Console.WriteLine("\nPress 1 to go to the main menu.");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.Clear(); // Clear the terminal for the main menu
                ShowMainMenu(); // Restart the program to show the main menu
            }
            else
            {
                Console.WriteLine("Invalid choice. Returning to main menu.");
                ShowMainMenu();
            }
        }


        // Execute SQL queries and display results in the terminal
        static void ExecuteQuery(string query)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Records ---");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i).PadRight(20));
                        }
                        Console.WriteLine();
                        Console.WriteLine(new string('-', reader.FieldCount * 20));

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader[i].ToString().PadRight(20));
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
