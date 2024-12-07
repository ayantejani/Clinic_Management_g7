using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace g7_Clinic_Management
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=clinicdb;User ID=root;Password=admin;";

        [STAThread]
        static void Main(string[] args)
        {
            ShowMainMenu();
        }

        // Main menu with options
        static void ShowMainMenu()
        {
            Console.Clear(); // Clear the terminal for the main menu
            Console.WriteLine("Welcome to the Clinic Management System\n");
            Console.WriteLine("Press 1 for Patients");
            Console.WriteLine("Press 2 for Doctors");
            Console.WriteLine("Press 3 for Appointments");
            Console.WriteLine("Press 4 for Prescriptions");
            Console.WriteLine("Press 5 for Billings");
            Console.WriteLine("Press 0 to Exit");

            while (true)
            {
                Console.Write("\nEnter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PatientMenu();
                        break;
                    case "2":
                        DoctorMenu();
                        break;
                    case "3":
                        AppointmentMenu();
                        break;
                    case "4":
                        PrescriptionMenu();
                        break;
                    case "5":
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

        // Patient-related menu options
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
                    OpenAddPatientForm();
                    break;
                case "3":
                    UpdatePatientMenu();
                    break;
                case "4":
                    DeletePatientMenu();
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
        }

        // Open Add Patient Form
        static void OpenAddPatientForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AddPatientForm());
        }

        // Update Patient Menu
        static void UpdatePatientMenu()
        {
            Console.Write("\nEnter Patient ID to Update: ");
            string patientId = Console.ReadLine();
            if (int.TryParse(patientId, out int id))
            {
                OpenUpdatePatientForm(id);
            }
            else
            {
                Console.WriteLine("Invalid Patient ID. Returning to Patient menu.");
            }
        }

        // Open Update Patient Form
        static void OpenUpdatePatientForm(int patientId)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdatePatientForm(patientId));
        }

        // Delete Patient Menu
        static void DeletePatientMenu()
        {
            Console.Write("\nEnter Patient ID to Delete: ");
            string patientId = Console.ReadLine();
            if (int.TryParse(patientId, out int id))
            {
                // Implement logic to delete patient by ID
                Console.WriteLine("Delete functionality is under construction.");
            }
            else
            {
                Console.WriteLine("Invalid Patient ID. Returning to Patient menu.");
            }
        }

        // View Doctor menu (only View option enabled)
        static void DoctorMenu()
        {
            Console.Clear();
            Console.WriteLine("Doctor Management");
            Console.WriteLine("Press 1 to View Doctors");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewDoctors();
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Doctor menu.");
                    break;
            }
        }

        // View Doctors
        static void ViewDoctors()
        {
            string query = "SELECT * FROM Doctor";
            ExecuteQuery(query);
        }

        // View Appointment menu (only View option enabled)
        static void AppointmentMenu()
        {
            Console.Clear();
            Console.WriteLine("Appointment Management");
            Console.WriteLine("Press 1 to View Appointments");
            Console.WriteLine("Press 0 to Return to Main Menu");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAppointments();
                    break;
                case "0":
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Appointment menu.");
                    break;
            }
        }

        // View Appointments
        static void ViewAppointments()
        {
            string query = "SELECT * FROM Appointment";
            ExecuteQuery(query);
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

        // Method to display changes in the terminal
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
    }
}
