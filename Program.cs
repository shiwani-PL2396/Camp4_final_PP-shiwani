using CMS_Project.Model;
using CMS_Project.Repository;
using CMS_Project.Service;
using static System.Net.Mime.MediaTypeNames;

namespace CMS_Project
{
    public class Program
    {

        static void Main(string[] args)
        {


            //service object instatiation
            ICmsService userService = new CmsServiceImpl();


            //login menu 
            #region login

            try
            {

                //useranme and password input

                while (true)
                {
                    Console.WriteLine("\n=== Clinical Mgmt System ===\n");

                    Console.Write("Enter Username: ");
                    string username = Console.ReadLine();

                    Console.Write("Enter Password: ");
                    string password = ReadPasswordWithMask();

                    LoginResult login = userService.AuthenticateUser(username, password);

                    if (login != null)
                    {
                        Console.Clear();
                        Console.WriteLine($"\nWelcome {login.RoleName}: {username}!\n");

                        if (login.RoleName == "Receptionist")
                            ReceptionistMenu();
                        else if (login.RoleName == "Doctor")
                            DoctorMenu(login.UserId); // now you can pass the UserId to fetch doctor-specific data
                    }




                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region receptionist menu
        public static void ReceptionistMenu()
        {
            try
            {
                ICmsService serviceInput = new CmsServiceImpl();

                bool running = true;

                //receptionist main menu
                while (running)
                {

                    Console.WriteLine("\n-- Receptionist Menu --");
                    Console.WriteLine("1. Search Patient");
                    Console.WriteLine("2. Register Patient");
                    Console.WriteLine("3. View Appointments");
                    Console.WriteLine("4. View Collection Report");
                    Console.WriteLine("5. Logout");
                    Console.Write("Enter choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":

                            Console.Clear();
                            Console.WriteLine("\nSearch Patient");
                            Console.WriteLine("Search by:");
                            Console.WriteLine("1. Registeration No.");
                            Console.WriteLine("2. Phone Number");
                            Console.WriteLine("3. Back");
                            Console.Write("Enter option: ");
                            string option = Console.ReadLine();

                            if (option == "1")
                                serviceInput.SearchPatientByIdAsync();
                            else if (option == "2")
                                serviceInput.SearchPatientsByPhone();
                            else if (option == "3")
                            {
                                Console.Clear();
                                break;
                            }
                            else
                                Console.WriteLine("Invalid option");
                            break;
                        case "2":
                            Console.Clear();
                            Console.WriteLine("Register the patient");
                            serviceInput.AddPatientAsync();
                            break;

                        case "3":
                            serviceInput.FilterAppointmentsByDate();
                            break;

                        case "4":
                            Console.WriteLine("View Collection Report ");
                            serviceInput.CollectionReport();
                            break;

                        case "5":
                            Console.WriteLine("Logging out...");
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");

                            break;
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Doctor Dashboard

        public static void DoctorMenu(int userId)
        {

            try
            {
                ICmsService doctorService = new CmsServiceImpl();
                ICmsRepository _repository = new CmsRepositoryImpl();


                var doctorInfo = doctorService.GetDoctorIdAndNameByUserId(userId);

                if (doctorInfo == null)
                {
                    Console.WriteLine("Doctor profile not found.");
                    return;
                }

                int doctorId = doctorInfo.Value.DoctorId;
                string doctorName = doctorInfo.Value.DoctorName;

                Console.Clear();
                Console.WriteLine("=== Doctor Dashboard ===");
                Console.WriteLine($"Doctor Name : {doctorName}\n");
            docboard:
                bool running = true;

                //receptionist main menu
                while (running)
                {
                    // Add menu logic here

                    Console.WriteLine("1. View Appointments");
                    Console.WriteLine("2. Patient Database");
                    Console.WriteLine("3. Schedule");
                    Console.WriteLine("4. Logout");
                    Console.Write("Enter choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":

                            #region View App
                            var appointments = doctorService.GetTodaysScheduledAppointmentsByDoctorId(doctorId);

                            if (appointments.Count == 0)
                            {
                                Console.WriteLine("No scheduled appointments for today.");
                                goto docboard;
                            }
                            else
                            {
                            appoinments:
                                Console.WriteLine("\n--- Today's Scheduled Appointments ---");

                                for (int i = 0; i < appointments.Count; i++)
                                {
                                    var appt = appointments[i];
                                    Console.WriteLine($"Token no.{i + 1}. Appoinment No.{appt.AppointmentId}, Patient: {appt.PatientName}, Time: {appt.AppointmentDate:hh:mm tt}, Status: {appt.AppointmentStatus}");
                                }
                                #endregion
                                Console.Write("\nSelect an appointment by number to view patient details: ");
                                string input = Console.ReadLine();
                                Console.Clear();

                                if (int.TryParse(input, out int apt) && apt >= 1 && apt <= appointments.Count)
                                {
                                    var selectedAppointment = appointments[apt - 1];
                                    int selectedAppointmentId = selectedAppointment.AppointmentId;
                                    int selectedPatientId = selectedAppointment.PatientId;

                                    //Console.WriteLine($"\nPatient ID: {selectedPatientId}");

                                    // Get patient details
                                    var patient = doctorService.GetPatientById(selectedPatientId);



                                    if (patient != null)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n------------ Patient Details ---------");
                                        Console.WriteLine($"Registeration no.: {patient.PatientId}");
                                        Console.WriteLine($"Name: {patient.PatientName}");
                                        Console.WriteLine($"Gender: {patient.Gender}");
                                        if (patient.DateOfBirth.HasValue)
                                        {
                                            DateTime dob = patient.DateOfBirth.Value;

                                            int age = DateTime.Today.Year - dob.Year;
                                            if (dob.Date > DateTime.Today.AddYears(-age)) age--;

                                            Console.WriteLine($"Date of Birth: {dob:yyyy-MM-dd}");
                                            Console.WriteLine($"Age: {age} years");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Date of Birth: Not Available");
                                            Console.WriteLine("Age: Unknown");
                                        }

                                        Console.WriteLine($"Date of Birth: {patient.DateOfBirth}");
                                        Console.WriteLine($"Phone No: {patient.PhoneNo}");
                                        Console.WriteLine($"Blood Group: {patient.BloodGroup}");
                                        Console.WriteLine($"Martial: {patient.MaritalStatus}");
                                        Console.WriteLine($"Address: {patient.PatientAddress}");
                                        Console.WriteLine($"Emergency Contact: {patient.EmergencyContactInfo}");


                                        int patientId = patient.PatientId;
                                        int counsultationid;


                                        if (patientId != null)
                                        {
                                            var consultations = doctorService.GetConsultationsByPatientId(patientId);
                                            if (consultations.Count == 0)
                                            {
                                                Console.WriteLine("-------------------------------------------------------------");

                                                Console.WriteLine("New Patient .No Exsisting Records ");
                                                counsultationid = doctorService.ManageConsultation(selectedAppointment.AppointmentId, patientId);
                                                goto patmenu;
                                            }
                                            else
                                            {
                                                Console.WriteLine("\nPatient History:");
                                                foreach (var c in consultations)
                                                {
                                                    Console.WriteLine("------------------------------------------------------");
                                                    Console.WriteLine($"Appointment Date: {c.AppointmentDate}");
                                                    Console.WriteLine($"History: {c.PatientHistory}");
                                                    Console.WriteLine($"Symptoms: {c.Symptoms}");
                                                    Console.WriteLine($"Diagnosis: {c.Diagnosis}");
                                                    Console.WriteLine($"Treatment: {c.Treatment}");
                                                    Console.WriteLine($"Notes: {c.Notes}");
                                                    Console.WriteLine("--------------------------------------------------------");
                                                }

                                                var allPrescriptions = _repository.GetPrescriptionsByPatientId(patientId);
                                                if (allPrescriptions.Any())
                                                {
                                                    Console.WriteLine("Patient Medications");
                                                    int index = 1;
                                                    foreach (var p in allPrescriptions)
                                                    {
                                                        Console.WriteLine("--------------------------------------------------------");
                                                        Console.WriteLine($"\nPrescription {index++}:");
                                                        Console.WriteLine($"Medicine Name: {p.MedicineName}");
                                                        Console.WriteLine($"Dosage: {p.Dosage}");
                                                        Console.WriteLine($"Duration: {p.Duration}");
                                                        Console.WriteLine($"Instructions: {p.Instructions}");
                                                        Console.WriteLine("--------------------------------------------------------");

                                                    }
                                                }

                                                var labTests = _repository.GetLabTestsByPatientId(patientId);

                                                if (labTests.Count == 0)
                                                {
                                                    Console.WriteLine("No lab tests found.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Lab Tests ");
                                                    for (int i = 0; i < labTests.Count; i++)
                                                    {
                                                        Console.WriteLine($"{i + 1}. {labTests[i].TestName} ({labTests[i].TestStatus})");
                                                    }

                                                    Console.WriteLine("Do you want to See Test Result?(y/n)");
                                                    string seeres = Console.ReadLine().ToLower();
                                                    if (seeres == "y")
                                                    {
                                                        Console.Write("\nSelect a lab test to view (enter number): ");
                                                        if (int.TryParse(Console.ReadLine(), out int choicee) && choicee >= 1 && choicee <= labTests.Count)
                                                        {
                                                            var selected = labTests[choicee - 1];
                                                            Console.WriteLine($"\n--- Lab Test Result ---");
                                                            Console.WriteLine($"Test Name : {selected.TestName}");
                                                            Console.WriteLine($"Test Date : {selected.TestDate}");
                                                            Console.WriteLine($"Status    : {selected.TestStatus}");
                                                            Console.WriteLine($"Result    : {(string.IsNullOrWhiteSpace(selected.Result) ? "Pending" : selected.Result)}");

                                                            Console.ReadKey();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Invalid selection.");
                                                            goto next;
                                                        }
                                                    }
                                                }
                                            }
                                        next:
                                            Console.WriteLine("Add Patient History");
                                            counsultationid = doctorService.ManageConsultation(selectedAppointment.AppointmentId, patientId);


                                        patmenu:

                                            Console.WriteLine("1.Add Prescription");
                                            Console.WriteLine("2.Add Lab Test");
                                            Console.WriteLine("3.Next Patient");
                                            Console.WriteLine("Choose");
                                            string choose = Console.ReadLine();


                                            if (choose == "1")
                                            {
                                                Console.WriteLine("\n--- Add New Prescription ---");


                                                var medList = doctorService.GetAllMedicines();
                                                Console.WriteLine("\nChoose Medicine:");
                                                for (int i = 0; i < medList.Count; i++)
                                                    Console.WriteLine($"{i + 1}. {medList[i]}");

                                                Console.Write("Enter choice: ");
                                                int medChoice = int.Parse(Console.ReadLine());

                                                Console.Write("Enter Dosage: ");
                                                string dosage = Console.ReadLine();

                                                Console.Write("Enter Duration: ");
                                                string duration = Console.ReadLine();

                                                Console.Write("Enter Instructions: ");
                                                string instructions = Console.ReadLine();

                                                var newprescription = new Prescription
                                                {
                                                    PatientId = patientId,
                                                    ConsultationId = counsultationid,
                                                    MedicineName = medList[medChoice - 1],
                                                    Dosage = dosage,
                                                    Duration = duration,
                                                    Instructions = instructions
                                                };

                                                _repository.AddPrescription(newprescription);
                                                Console.WriteLine("Prescription added.");
                                                goto patmenu;




                                            }
                                            else if (choose == "2")
                                            {
                                                var testList = doctorService.GetAllLabTests();
                                                Console.WriteLine("\nChoose Lab Test:");
                                                for (int i = 0; i < testList.Count; i++)
                                                    Console.WriteLine($"{i + 1}. {testList[i]}");

                                                Console.Write("Enter choice: ");
                                                int testChoice = int.Parse(Console.ReadLine());

                                                var labTest = new LabTest
                                                {
                                                    PatientId = patientId,
                                                    ConsultationId = counsultationid,
                                                    TestName = testList[testChoice - 1],
                                                    TestStatus = "Pending"
                                                };

                                                _repository.AddLabTest(labTest);
                                                Console.WriteLine("Lab test added.");
                                                //doctorService.ManageLabTests(selectedAppointment.PatientId);
                                                goto patmenu;
                                            }
                                            else if (choose == "3")
                                            {

                                                doctorService.MarkAppointmentCompleted(selectedAppointment.AppointmentId);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid choice. Try again.");
                                                break;

                                            }
                                        }


                                    }
                                    else
                                    {
                                        Console.WriteLine("Patient details not found.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid choice. Please select a valid appointment number.");
                                    goto appoinments;
                                }

                            }


                            Console.WriteLine(); // spacing
                            break;

                        case "2":

                            Console.WriteLine("\nSearch Patient");
                            int PatId = doctorService.SearchPatientById();

                            // Get patient details
                            var patients = doctorService.GetPatientById(PatId);



                            if (patients != null)
                            {
                                Console.Clear();
                                Console.WriteLine("\n------------ Patient Details ---------");
                                Console.WriteLine($"Registeration no.: {patients.PatientId}");
                                Console.WriteLine($"Name: {patients.PatientName}");
                                Console.WriteLine($"Gender: {patients.Gender}");
                                if (patients.DateOfBirth.HasValue)
                                {
                                    DateTime dob = patients.DateOfBirth.Value;

                                    int age = DateTime.Today.Year - dob.Year;
                                    if (dob.Date > DateTime.Today.AddYears(-age)) age--;

                                    Console.WriteLine($"Date of Birth: {dob:yyyy-MM-dd}");
                                    Console.WriteLine($"Age: {age} years");
                                }
                                else
                                {
                                    Console.WriteLine("Date of Birth: Not Available");
                                    Console.WriteLine("Age: Unknown");
                                }

                                Console.WriteLine($"Date of Birth: {patients.DateOfBirth}");
                                Console.WriteLine($"Phone No: {patients.PhoneNo}");
                                Console.WriteLine($"Blood Group: {patients.BloodGroup}");
                                Console.WriteLine($"Martial: {patients.MaritalStatus}");
                                Console.WriteLine($"Address: {patients.PatientAddress}");
                                Console.WriteLine($"Emergency Contact: {patients.EmergencyContactInfo}");
                                int patId = patients.PatientId;
                                int counsultationid;

                                if (patId != null)
                                {
                                    var consultations = doctorService.GetConsultationsByPatientId(patId);
                                    Console.WriteLine("\nPatient History:");
                                    foreach (var c in consultations)
                                    {
                                        Console.WriteLine("------------------------------------------------------");
                                        Console.WriteLine($"Appointment Date: {c.AppointmentDate}");
                                        Console.WriteLine($"History: {c.PatientHistory}");
                                        Console.WriteLine($"Symptoms: {c.Symptoms}");
                                        Console.WriteLine($"Diagnosis: {c.Diagnosis}");
                                        Console.WriteLine($"Treatment: {c.Treatment}");
                                        Console.WriteLine($"Notes: {c.Notes}");
                                        Console.WriteLine("--------------------------------------------------------");
                                    }

                                    var allPrescriptions = _repository.GetPrescriptionsByPatientId(patId);
                                    if (allPrescriptions.Any())
                                    {
                                        Console.WriteLine("Patient Medications");
                                        int index = 1;
                                        foreach (var p in allPrescriptions)
                                        {
                                            Console.WriteLine("--------------------------------------------------------");
                                            Console.WriteLine($"\nPrescription {index++}:");
                                            Console.WriteLine($"Medicine Name: {p.MedicineName}");
                                            Console.WriteLine($"Dosage: {p.Dosage}");
                                            Console.WriteLine($"Duration: {p.Duration}");
                                            Console.WriteLine($"Instructions: {p.Instructions}");
                                            Console.WriteLine("--------------------------------------------------------");

                                        }
                                    }

                                    var labTests = _repository.GetLabTestsByPatientId(patId);

                                    if (labTests.Count == 0)
                                    {
                                        Console.WriteLine("No lab tests found.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Lab Tests ");
                                        for (int i = 0; i < labTests.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. {labTests[i].TestName} ({labTests[i].TestStatus})");
                                        }

                                        Console.WriteLine("Do you want to See Test Result?(y/n)");
                                        string seeres = Console.ReadLine().ToLower();
                                        if (seeres == "y")
                                        {
                                            Console.Write("\nSelect a lab test to view (enter number): ");
                                            if (int.TryParse(Console.ReadLine(), out int choicee) && choicee >= 1 && choicee <= labTests.Count)
                                            {
                                                var selected = labTests[choicee - 1];
                                                Console.WriteLine($"\n--- Lab Test Result ---");
                                                Console.WriteLine($"Test Name : {selected.TestName}");
                                                Console.WriteLine($"Test Date : {selected.TestDate}");
                                                Console.WriteLine($"Status    : {selected.TestStatus}");
                                                Console.WriteLine($"Result    : {(string.IsNullOrWhiteSpace(selected.Result) ? "Pending" : selected.Result)}");

                                                Console.ReadKey();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid selection.");
                                                Console.ReadKey();


                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No such Patient");
                                Console.ReadKey();
                            }
                            break;
                        case "3":
                            doctorService.UpdateDoctorSchedule(doctorId);
                            break;
                        case "4":
                            Console.WriteLine("Logging out...");
                            running = false;
                            break;
                        default:
                            Console.WriteLine("invalid choice choose again");
                            Console.ReadKey();

                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }


        #endregion

        #region mask password
        static string ReadPasswordWithMask()
        {
            string password = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true); // true = don't display key

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b"); // erase last *
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*"); // show * instead of real character
                }
            }

            return password;
        }
        #endregion



    }
}
