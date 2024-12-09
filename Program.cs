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
        public static void ShowMainMenu()
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
                        PatientManager.PatientMenu();
                        break;
                    case "3":
                        DoctorManager.DoctorMenu();
                        break;
                    case "4":
                        AppointmentManager.AppointmentMenu();
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
                    PatientManager.DisplayAssociatedRecords(connection, appointmentQuery, patientId);

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
                    PatientManager.DisplayAssociatedRecords(connection, prescriptionQuery, patientId);

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
                    PatientManager.DisplayAssociatedRecords(connection, billingQuery, patientId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching patient details: " + ex.Message);
            }
        }


        // Helper method for DBNull handling
        public static string DBNullSafe(object value, bool isDate = false)
        {
            if (value == DBNull.Value)
                return isDate ? "N/A" : "Unknown";
            if (isDate)
                return Convert.ToDateTime(value).ToShortDateString();
            return value.ToString();
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
