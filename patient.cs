using System;
using g7_Clinic_Management;
using MySql.Data.MySqlClient;

namespace group7_Clinic_Management
{
    public static class PatientManager
    {
        private static string connectionString = "Server=localhost;Database=ClinicDB;User ID=root;Password=admin;";

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
                    AddPatient();
                    break;
                case "3":
                    UpdatePatient();
                    break;
                case "4":
                    DeletePatient();
                    break;
                case "0":
                    Program.ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to Patient Menu.");
                    Console.ReadKey();
                    PatientMenu();
                    break;
            }
        }

        public static void ViewPatients()
        {
            // Code to view patients
        }

        public static void AddPatient()
        {
            // Code to add a new patient
        }

        public static void UpdatePatient()
        {
            // Code to update patient details
        }

        public static void DeletePatient()
        {
            // Code to delete a patient
        }
    }
}
