using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Azure;
using Azure.Identity;
using ClassLibraryValidation;
using CMS_Project.Model;
using CMS_Project.Repository;
using CMS_Project.ViewModel;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

        

namespace CMS_Project.Service
    {

   public class CmsServiceImpl : ICmsService
        {
            //repository object instatiation
            private readonly CmsRepositoryImpl _repository;

            public CmsServiceImpl()
            {
                _repository = new CmsRepositoryImpl();
            }


            //passing username and password to repository
            #region Receptionist

            #region Authenticate
            public LoginResult AuthenticateUser(string username, string password)
            {
                return _repository.GetUserRole(username, password);
            }
            #endregion

            #region Addpat
            public int AddPatientAsync()
            {
                string name;
                while (true)
                {
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    if (PatientValidator.IsValidName(name))
                        break;
                    Console.WriteLine("Invalid name. Only letters and spaces allowed, and it must not start with a space.");
                }

                // Blood Group
                string bloodGroup;
                while (true)
                {
                    Console.Write("Enter Blood Group: ");
                    bloodGroup = Console.ReadLine();
                    if (PatientValidator.IsValidBloodGroup(bloodGroup))
                        break;
                    Console.WriteLine("Invalid blood group. Please enter a valid type like A+, B-, etc.");
                }

                // Address
                string address;
                while (true)
                {
                    Console.Write("Enter Address: ");
                    address = Console.ReadLine();
                    if (PatientValidator.IsValidName(address))
                        break;
                    Console.WriteLine("Invalid address. Please enter a valid address.");
                }

                // Phone Number
                string phoneNo;
                while (true)
                {
                    Console.Write("Enter Phone Number: ");
                    phoneNo = Console.ReadLine();
                    if (PatientValidator.IsValidPhoneNumber(phoneNo))
                        break;
                    Console.WriteLine("Invalid phone number. Must be 10 digits.");
                }

                // Emergency Contact Number
                string emergencyPhoneNo;
                while (true)
                {
                    Console.Write("Enter Emergency Contact Number: ");
                    emergencyPhoneNo = Console.ReadLine();
                    if (PatientValidator.IsValidEmergencyContact(emergencyPhoneNo))
                        break;
                    Console.WriteLine("Invalid emergency contact number. Must be 10 digits.");
                }

                // Date of Birth
                DateTime dob;
                while (true)
                {
                    Console.Write("Enter Date of Birth (dd-MM-yyyy): ");
                    string dobInput = Console.ReadLine();
                    if (DateTime.TryParseExact(dobInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out dob)
                        && PatientValidator.IsValidDateOfBirth(dob))
                    {
                        break;
                    }
                    Console.WriteLine("Invalid date of birth. Please use format dd-MM-yyyy and ensure it's not a future date.");
                }

                // Gender (optional validation)
                string gender = string.Empty;

                while (true)
                {
                    Console.WriteLine("Select Gender: ");
                    Console.WriteLine("1. Female");
                    Console.WriteLine("2. Male");
                    Console.WriteLine("3. Other");
                    Console.Write("Enter your choice (1-3): ");
                    string genderInput = Console.ReadLine();

                    if (genderInput == "1")
                    {
                        gender = "Female";
                        break;
                    }
                    else if (genderInput == "2")
                    {
                        gender = "Male";
                        break;
                    }
                    else if (genderInput == "3")
                    {
                        gender = "Other";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                    }
                }


                string maritalStatus = string.Empty;

                while (true)
                {
                    Console.WriteLine("Select Marital Status: ");
                    Console.WriteLine("1. Single");
                    Console.WriteLine("2. Married");
                    Console.WriteLine("3. Divorced");
                    Console.WriteLine("4. Widowed");
                    Console.Write("Enter your choice (1-4): ");
                    string input = Console.ReadLine();

                    if (input == "1")
                    {
                        maritalStatus = "Single";
                        break;
                    }
                    else if (input == "2")
                    {
                        maritalStatus = "Married";
                        break;
                    }
                    else if (input == "3")
                    {
                        maritalStatus = "Divorced";
                        break;
                    }
                    else if (input == "4")
                    {
                        maritalStatus = "Widowed";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 4.");
                    }
                }

                // Nationality
                string nationality;
                while (true)
                {
                    Console.Write("Enter Nationality: ");
                    nationality = Console.ReadLine();
                    if (PatientValidator.IsValidName(nationality))
                        break;
                    Console.WriteLine("Invalid nationality. Only letters and spaces allowed.");
                }
                var patient = new Patient
                {
                    PatientName = name,
                    BloodGroup = bloodGroup,
                    PatientAddress = address,
                    PhoneNo = phoneNo,
                    EmergencyContactInfo = emergencyPhoneNo,
                    DateOfBirth = dob,
                    Gender = gender,
                    MaritalStatus = maritalStatus,
                    Nationality = nationality
                };
                int patientId = _repository.AddPatient(patient);
                Console.WriteLine($"Patient added successfully. Patient Registeration no.: {patientId}");
                Console.ReadKey();
                Console.WriteLine("\nWhat do you want to do next:");
                Console.WriteLine("1.Schedule Appointment");
                Console.WriteLine("2.Update Details of Patient");
                Console.WriteLine("3.Main menu");
                Console.WriteLine("Enter your choice");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        FixAppointmentAsync(patientId);//edit app
                        break;
                    case "2":
                        UpdatePatientAsync();
                        break;
                    default:
                        break;
                }
                return patientId;
            }
            #endregion

            #region SearchPat-ID
            public void SearchPatientByIdAsync()
            {
                Console.Write("Enter Patient Registeration No.: ");
                int id = int.Parse(Console.ReadLine());

                var patient = _repository.GetPatientById(id);
                if (patient != null)

                {
                    int age = DateTime.Now.Year - patient.DateOfBirth.Value.Year;
                    if (DateTime.Now.Date < patient.DateOfBirth.Value.Date.AddYears(age))
                    {
                        age--; // Adjust if birthday hasn't occurred yet this year
                    }
                    Console.Clear();
                    Console.WriteLine("\nPatient Found\n");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{"Registeration no.",-5} {"Name",-20} {"Phone",-15} {"DOB",-15} {"Age",-5}{"Gender",-10}");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{patient.PatientId,-5} {patient.PatientName,-20} {patient.PhoneNo,-15} {patient.DateOfBirth.Value.ToString("dd-MM-yyyy"),-15} {age,-5} {patient.Gender,-10}");

                    Console.ReadKey();
                    Console.WriteLine("\nWhat do you want to do next:");
                    Console.WriteLine("1.Schedule Appointment");
                    Console.WriteLine("2.Update Details of Patient");
                    Console.WriteLine("3.Main menu");
                    Console.WriteLine("Enter your choice");
                    string option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            FixAppointmentAsync(patient.PatientId);//edit app
                            break;
                        case "2":
                            UpdatePatientAsync();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Patient not found.");
                    Console.WriteLine("Do you want to Add Patient (y/n):");
                    string inputs = Console.ReadLine()?.ToLower();
                    char Addpat = string.IsNullOrWhiteSpace(inputs) ? 'n' : inputs[0];


                    if (Addpat == 'y')
                    {
                        Console.Clear();
                        Console.WriteLine("Register Patient");
                        AddPatientAsync();
                    }
                }
            }
            #endregion
            public int SearchPatientById()
            {
                Console.Write("Enter Patient Registeration No.: ");
                int id = int.Parse(Console.ReadLine());

                var patient = _repository.GetPatientById(id);
                if (patient != null)

                {
                    int age = DateTime.Now.Year - patient.DateOfBirth.Value.Year;
                    if (DateTime.Now.Date < patient.DateOfBirth.Value.Date.AddYears(age))
                    {
                        age--; // Adjust if birthday hasn't occurred yet this year
                    }
                    Console.Clear();
                    Console.WriteLine("\nPatient Found\n");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{"Registeration no.",-5} {"Name",-20} {"Phone",-15} {"DOB",-15} {"Age",-5}{"Gender",-10}");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{patient.PatientId,-5} {patient.PatientName,-20} {patient.PhoneNo,-15} {patient.DateOfBirth.Value.ToString("dd-MM-yyyy"),-15} {age,-5} {patient.Gender,-10}");
                    return patient.PatientId;


                }
                else
                {
                    Console.WriteLine("Patient not found.");
                    Console.ReadKey();

                    return -1;

                }
            }
            #region SearchPat-phoneno.

            public void SearchPatientsByPhone()
            {
                Console.Write("Enter Phone Number: ");
                string phone = Console.ReadLine();

                List<Patient> patients = _repository.GetPatientsByPhone(phone);

                if (patients.Count == 0)
                {
                    Console.WriteLine("No patient found.");
                    Console.WriteLine("Do you want to Add Patient (y/n):");
                    string inputs = Console.ReadLine()?.ToLower();
                    char Addpat = string.IsNullOrWhiteSpace(inputs) ? 'n' : inputs[0];


                    if (Addpat == 'y')
                    {
                        Console.WriteLine("Register Patient");
                        AddPatientAsync();
                    }
                }
                else if (patients.Count == 1)
                {

                    Console.WriteLine("\nPatient Found\n");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{"Registeration no.",-5} {"Name",-20} {"Phone",-15} {"DOB",-15} {"Age",-5}{"Gender",-10}");
                    Console.WriteLine("--------------------------------------------------------------------------------------");

                    foreach (var p in patients)
                    {
                        int age = DateTime.Now.Year - p.DateOfBirth.Value.Year;
                        if (DateTime.Now.Date < p.DateOfBirth.Value.Date.AddYears(age))
                        {
                            age--; // Adjust if birthday hasn't occurred yet this year
                        }

                        Console.WriteLine($"{p.PatientId,-5} {p.PatientName,-20} {p.PhoneNo,-15} {p.DateOfBirth.Value.ToString("dd-MM-yyyy"),-15} {age,-5} {p.Gender,-10}");


                        Console.WriteLine("What do you want to do next:");
                        Console.WriteLine("1.Fix Appointment");
                        Console.WriteLine("2.Update Details of Patient");
                        Console.WriteLine("3.Main menu");
                        Console.WriteLine("Enter your choice");
                        string option = Console.ReadLine();

                        switch (option)
                        {
                            case "1":
                                FixAppointmentAsync(p.PatientId);
                                break;
                            case "2":
                                UpdatePatientAsync();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else

                {
                    Console.WriteLine("\nPatient Found\n");
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    Console.WriteLine($"{"Registeration no.",-5} {"Name",-20} {"Phone",-15} {"DOB",-15} {"Age",-5}{"Gender",-10}");
                    Console.WriteLine("--------------------------------------------------------------------------------------");


                    foreach (var p in patients)
                    {
                        int age = DateTime.Now.Year - p.DateOfBirth.Value.Year;
                        if (DateTime.Now.Date < p.DateOfBirth.Value.Date.AddYears(age))
                        {
                            age--; // Adjust if birthday hasn't occurred yet this year
                        }

                        Console.WriteLine($"{p.PatientId,-5} {p.PatientName,-20} {p.PhoneNo,-15} {p.DateOfBirth.Value.ToString("dd-MM-yyyy"),-15} {age,-5} {p.Gender,-10}");


                    }
                    Console.WriteLine("Enter the Registeration number of patient you want to select");
                    SearchPatientByIdAsync();
                }
            }

            #endregion

            #region UpdatePat
            public void UpdatePatientAsync()
            {
                Console.Write("Enter Patient Registeration no. to update: ");
                int patientId = int.Parse(Console.ReadLine());

                var existing = _repository.GetPatientById(patientId);
                if (existing == null)
                {
                    Console.WriteLine("Patient not found.");
                    return;
                }

                Console.WriteLine("Enter new details (leave blank to keep current):");

                Console.Write($"Name ({existing.PatientName}): ");
                string name = Console.ReadLine();
                name = string.IsNullOrWhiteSpace(name) ? existing.PatientName : name;

                Console.Write($"Blood Group ({existing.BloodGroup}): ");
                string bloodGroup = Console.ReadLine();
                bloodGroup = string.IsNullOrWhiteSpace(bloodGroup) ? existing.BloodGroup : bloodGroup;

                Console.Write($"Address ({existing.PatientAddress}): ");
                string address = Console.ReadLine();
                address = string.IsNullOrWhiteSpace(address) ? existing.PatientAddress : address;

                Console.Write($"Phone No ({existing.PhoneNo}): ");
                string phone = Console.ReadLine();
                phone = string.IsNullOrWhiteSpace(phone) ? existing.PhoneNo : phone;

                Console.Write($"Emergency Contact ({existing.EmergencyContactInfo}): ");
                string emergency = Console.ReadLine();
                emergency = string.IsNullOrWhiteSpace(emergency) ? existing.EmergencyContactInfo : emergency;


                Console.Write("Enter Date of Birth (yyyy-MM-dd): ");
                DateTime dob = DateTime.Parse(Console.ReadLine());
                //exsist or not check

                Console.Write($"Gender ({existing.Gender}): ");
                string gender = Console.ReadLine();
                gender = string.IsNullOrWhiteSpace(gender) ? existing.Gender : gender;

                Console.Write($"Marital Status ({existing.MaritalStatus}): ");
                string marital = Console.ReadLine();
                marital = string.IsNullOrWhiteSpace(marital) ? existing.MaritalStatus : marital;

                Console.Write($"Nationality ({existing.Nationality}): ");
                string nationality = Console.ReadLine();
                nationality = string.IsNullOrWhiteSpace(nationality) ? existing.Nationality : nationality;

                var updated = new Patient
                {
                    PatientId = patientId,
                    PatientName = name,
                    BloodGroup = bloodGroup,
                    PatientAddress = address,
                    PhoneNo = phone,
                    EmergencyContactInfo = emergency,
                    DateOfBirth = dob,
                    Gender = gender,
                    MaritalStatus = marital,
                    Nationality = nationality
                };

                try
                {
                    _repository.UpdatePatient(updated);
                    Console.WriteLine("Patient updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            #endregion

            #region FixApp
            public void FixAppointmentAsync(int patientId)
            {
                // Step 1: Get all departments
                var departments = _repository.GetAllDepartments();
                if (departments.Count == 0)
                {
                    Console.WriteLine("No departments found.");
                    return;
                }
                Console.Clear();
                Console.WriteLine("Departments:");
                foreach (var dept in departments)
                {
                    Console.WriteLine($"{dept.DepartmentId}. {dept.DepartmentName}");
                }

                // Step 2: Ask for department ID
                int departmentId;
                while (true)
                {
                    Console.Write("Select Department to view available doctors: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out departmentId))
                    {
                        break; // valid input, exit the loop
                    }
                    else
                    {
                        Console.WriteLine("Invalid Department ID. Please enter a valid number.");
                    }
                }

                // Step 3: Get active doctors for selected department
                var doctors = _repository.GetActiveDoctorsByDepartment(departmentId);
                if (doctors.Count == 0)
                {
                    Console.WriteLine("No active doctors available in this department.");
                    return;
                }

                Console.WriteLine("Available Doctors:");
                foreach (var doc in doctors)
                {
                    Console.WriteLine($"{doc.DoctorId}.{doc.DoctorName}");
                }

                // Step 4: Ask for doctor ID
                int doctorId;
                while (true)
                {
                    Console.Write("Select Doctor from above list: ");
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out doctorId))
                    {
                        Console.WriteLine("Invalid Doctor ID. Please enter a valid number.");
                        continue;
                    }

                    if (!doctors.Any(d => d.DoctorId == doctorId))
                    {
                        Console.WriteLine("Doctor ID not found or not active in this department. Please try again.");
                        continue;
                    }

                    // Valid and existing doctor ID found
                    break;
                }

                // Step 5: Ask whether to book for today or tomorrow
                DateTime selectedDate;
            chhoseagain:
                while (true)
                {
                    Console.WriteLine("Book appointment for:");
                    Console.WriteLine("1. Today");
                    Console.WriteLine("2. Tomorrow");
                    Console.Write("Enter choice (1 or 2): ");
                    string choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        selectedDate = DateTime.Today;
                        break;
                    }
                    else if (choice == "2")
                    {
                        selectedDate = DateTime.Today.AddDays(1);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    }
                }

                Appointment existingApp = _repository.GetDuplicateAppointment(patientId, doctorId, selectedDate);

                if (existingApp != null)
                {
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("\n❗ Duplicate appointment found:");
                    Console.WriteLine($"Appointment ID : {existingApp.AppointmentId}");
                    Console.WriteLine($"Date           : {existingApp.AppointmentDate}");
                    Console.WriteLine($"Status         : {existingApp.AppointmentStatus}");
                    Console.WriteLine("\nThis appointment already exists.");

                    Console.WriteLine("Do you want to:");
                    Console.WriteLine("1. Reschedule the appointment");
                    Console.WriteLine("2. Cancel the appointment");
                    Console.WriteLine("3. Exit");
                    Console.Write("Enter your choice: ");
                    string editapp = Console.ReadLine();

                    switch (editapp)
                    {
                        case "1":
                            _repository.CancelAppointment(existingApp.AppointmentId);
                            Console.WriteLine(" Existing appointment cancelled. Please continue to book a new one.");
                            break;

                        case "2":
                            _repository.CancelAppointment(existingApp.AppointmentId);
                            return;
                        default:
                            Console.WriteLine("No action taken.");
                            return;
                    }
                }

                string slotype = "";
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                // Slot type selection logic
                if (selectedDate.Date == DateTime.Today)
                {
                    if (currentTime < TimeSpan.FromHours(12))
                    {
                        Console.WriteLine("Select Slot Type:");
                        Console.WriteLine("1. Morning");
                        Console.WriteLine("2. Evening");
                        string choic = Console.ReadLine();
                        slotype = choic == "1" ? "Morning" : "Evening";
                    }
                    else
                    {
                        Console.WriteLine("It's already afternoon. Only Evening slots can be booked.");
                        slotype = "Evening";
                    }
                }
                else // Tomorrow or future
                {
                    Console.WriteLine("Select Slot Type:");
                    Console.WriteLine("1. Morning");
                    Console.WriteLine("2. Evening");
                    string choic = Console.ReadLine();
                    slotype = choic == "1" ? "Morning" : "Evening";
                }

                var availableSlots = GetAvailableAppointmentSlots(doctorId, selectedDate, slotype);

                if (availableSlots.Count == 0)
                {
                    Console.WriteLine("No available slots for selected day.");
                    goto chhoseagain;

                }

                Console.WriteLine("Available Time Slots:");
                for (int i = 0; i < availableSlots.Count; i++)
                    Console.WriteLine($"{i + 1}. {availableSlots[i]:hh\\:mm}");

                Console.Write("Choose a time slot number: ");
                if (!int.TryParse(Console.ReadLine(), out int slotChoice) || slotChoice < 1 || slotChoice > availableSlots.Count)
                {
                    Console.WriteLine("Invalid slot selected.");
                    return;
                }

                TimeSpan chosenTime = availableSlots[slotChoice - 1];
                DateTime appointmentDateTime = selectedDate.Add(chosenTime);

                Console.WriteLine($"Appointment scheduled for: {appointmentDateTime:yyyy-MM-dd HH:mm}");

                // Step 6: Finalize and save appointment
                var appointment = new Appointment
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    AppointmentDate = appointmentDateTime,
                    AppointmentStatus = "Scheduled"
                };

                int appoinmentid = _repository.AddAppointment(appointment);
                Console.WriteLine("Appointment Scheduled Successfully.");

                // Step 7: Generate Bill
                decimal consultationFee = _repository.GetConsultationFee(appointment.DoctorId);

                Bill bill = new Bill
                {
                    AppointmentId = appoinmentid,
                    DoctorId = appointment.DoctorId,
                    BillAmount = consultationFee,
                    BillDate = DateTime.Now
                };

                _repository.GenerateBill(bill);
                Console.WriteLine("Bill Generated Successfully.");

                Console.ReadKey();
                Console.Clear();

                // Step 8: Display bill details
                BillViewModel billDetails = _repository.GetBillDetails(bill.AppointmentId);

                if (billDetails != null)
                {
                    Console.WriteLine("\n--- Bill Generated ---");
                    Console.WriteLine($"Token Number : {billDetails.AppointmentId}");
                    Console.WriteLine($"Patient Name   : {billDetails.PatientName}");
                    Console.WriteLine($"Doctor Name    : {billDetails.DoctorName}");
                    Console.WriteLine($"Amount         : Rs.{billDetails.BillAmount}");
                    Console.WriteLine($"Date           : {billDetails.BillDate:yyyy-MM-dd HH:mm}");
                    Console.WriteLine("------------------------");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve bill details.");
                }
                Console.ReadKey();
                Console.Clear();
            }

            #endregion

            #region GetAvailableAppointmentSlots
            public List<TimeSpan> GetAvailableAppointmentSlots(int doctorId, DateTime date, string slotType)
            {
                var workingSlots = _repository.GetDoctorSchedule(doctorId, slotType); // step 1
                var bookedSlots = _repository.GetBookedSlots(doctorId, date);         // step 2

                TimeSpan interval = TimeSpan.FromMinutes(20);
                List<TimeSpan> available = new();

                TimeSpan now = DateTime.Now.TimeOfDay;

                foreach (var (start, end) in workingSlots)
                {
                    for (TimeSpan t = start; t < end; t += interval)
                    {
                        // ✅ Skip past time slots only if booking for today
                        if (date.Date == DateTime.Today && t <= now)
                            continue;

                        if (!bookedSlots.Contains(t))
                            available.Add(t);
                    }
                }

                return available;
            }



            #endregion

            #region AllApp


            public void FilterAppointmentsByDate()
            {
                Console.WriteLine("Do you want to view appointments for:");
                Console.WriteLine("1. Today");
                Console.WriteLine("2. Tomorrow");
                Console.Write("Enter choice (1 or 2): ");

                string choice = Console.ReadLine();
                DateTime selectedDate;

                if (choice == "1")
                {
                    selectedDate = DateTime.Today;
                }
                else if (choice == "2")
                {
                    selectedDate = DateTime.Today.AddDays(1);
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    return;
                }

                var all = _repository.GetAllAppointments();

                var filtered = all
                    .Where(a =>
                        a.AppointmentDate.Date == selectedDate.Date &&
                        (a.AppointmentStatus.Equals("Scheduled", StringComparison.OrdinalIgnoreCase) ||
                         a.AppointmentStatus.Equals("Rescheduled", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (filtered.Count == 0)
                {
                    Console.WriteLine($"No scheduled or rescheduled appointments on {selectedDate:yyyy-MM-dd}.");
                }
                else
                {
                    Console.WriteLine($"\nAppointments on {selectedDate:yyyy-MM-dd}:");
                    Console.WriteLine("----------------------------------------------------------------------------------------------");
                    Console.WriteLine($"{"Token No.",-10}{"Appoinment No.",-15}{"Patient Name",-20}{"Doctor Name",-20}{"Date",-25}{"Status",-15}");
                    Console.WriteLine("----------------------------------------------------------------------------------------------");
                    int serial = 1;
                    foreach (var app in filtered)
                    {
                        PrintAppointment(app, serial);
                        serial++;
                    }
                }
            }

            public void PrintAppointment(AppointmentViewModel app, int serial)
            {
                Console.WriteLine($"{serial,-10}{app.AppointmentId,-15}{app.PatientName,-20}{app.DoctorName,-20}{app.AppointmentDate,-25:g}{app.AppointmentStatus,-15}");
            }
            #endregion

            #region collection report
            public void CollectionReport()
            {
                DateTime startDate;
                while (true)
                {
                    Console.Write("Enter start date (yyyy-MM-dd): ");
                    string input = Console.ReadLine();

                    if (DateTime.TryParse(input, out startDate) && startDate <= DateTime.Today)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date. Please enter a valid past or today’s date in yyyy-MM-dd format.");
                    }
                }

                DateTime endDate;
                while (true)
                {
                    Console.Write("Enter end date (yyyy-MM-dd): ");
                    string input = Console.ReadLine();

                    if (DateTime.TryParse(input, out endDate) && endDate <= DateTime.Today)
                    {
                        break; // Valid date, exit loop
                    }
                    else
                    {
                        Console.WriteLine("Invalid end date. Please enter a valid date not in the future (format: yyyy-MM-dd).");
                    }
                }

                var (bills, totalPatients, totalAmount) = _repository.GetBillsWithSummary(startDate, endDate);

                if (bills.Count == 0)
                {
                    Console.WriteLine("No bills found for the selected date range.");
                }
                else
                {
                    Console.WriteLine($"\n--- Bills from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} ---");
                    foreach (var b in bills)
                    {
                        Console.WriteLine($"Appointment No. : {b.AppointmentId}");
                        Console.WriteLine($"Patient Name   : {b.PatientName}");
                        Console.WriteLine($"Doctor Name    : {b.DoctorName}");
                        Console.WriteLine($"Amount         : ₹{b.BillAmount}");
                        Console.WriteLine($"Bill Date      : {b.BillDate:yyyy-MM-dd HH:mm}");
                        Console.WriteLine("--------------------------------------");
                    }

                    // Summary section
                    Console.WriteLine($"\nTotal Patients: {totalPatients}");
                    Console.WriteLine($"Total Amount Collected: ₹{totalAmount}");
                    Console.WriteLine("--------------------------------------");
                }
            }
            #endregion

            #endregion

            #region doctor
            public (int DoctorId, string DoctorName)? GetDoctorIdAndNameByUserId(int userId)
            {
                return _repository.GetDoctorIdAndNameByUserId(userId);
            }

            public List<AppointmentViewModel> GetTodaysScheduledAppointmentsByDoctorId(int doctorId)
            {
                return _repository.GetTodaysScheduledAppointmentsByDoctorId(doctorId);
            }


            public Patient GetPatientById(int patientId)
            {
                return _repository.GetPatientById(patientId);
            }



            public int ManageConsultation(int appointmentId, int patientId)
            {
                var consultation = new Consultation
                {
                    AppointmentId = appointmentId,
                    PatientId = patientId
                };

                Console.Write("Enter Patient History: ");
                string input = Console.ReadLine();
                consultation.PatientHistory = string.IsNullOrWhiteSpace(input) ? null : input;

                Console.Write("Enter Symptoms: ");
                input = Console.ReadLine();
                consultation.Symptoms = string.IsNullOrWhiteSpace(input) ? null : input;

                Console.Write("Enter Diagnosis: ");
                input = Console.ReadLine();
                consultation.Diagnosis = string.IsNullOrWhiteSpace(input) ? null : input;

                Console.Write("Enter Treatment: ");
                input = Console.ReadLine();
                consultation.Treatment = string.IsNullOrWhiteSpace(input) ? null : input;

                Console.Write("Add Notes (optional): ");
                string notes = Console.ReadLine();
                consultation.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;

                int patid = patientId;
                consultation.PatientId = patid;


                int consultationId = _repository.AddConsultation(consultation);

                Console.WriteLine("New consultation added successfully.");
                return consultationId;
            }
            public void AddPrescription(int patientId)
            {
                var allPrescriptions = _repository.GetPrescriptionsByPatientId(patientId);
                if (allPrescriptions.Any())
                {
                    Console.WriteLine("All Prescriptions for Patient:");
                    int index = 1;
                    foreach (var p in allPrescriptions)
                    {
                        Console.WriteLine($"\nPrescription {index++}:");
                        Console.WriteLine($"Consultation ID: {p.ConsultationId}");
                        Console.WriteLine($"Medicine Name: {p.MedicineName}");
                        Console.WriteLine($"Dosage: {p.Dosage}");
                        Console.WriteLine($"Duration: {p.Duration}");
                        Console.WriteLine($"Instructions: {p.Instructions}");
                    }
                }

            }


            public void ManageLabTests(int consultationId, int patientId)
            {
                var labTests = _repository.GetLabTestsByPatientId(patientId);

                if (labTests.Count == 0)
                {
                    Console.WriteLine("No lab tests found.");
                }
                else
                {
                    Console.WriteLine("--- Lab Tests ---");
                    for (int i = 0; i < labTests.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {labTests[i].TestName} ({labTests[i].TestStatus})");
                    }

                    Console.Write("\nSelect a lab test to view (enter number): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= labTests.Count)
                    {
                        var selected = labTests[choice - 1];
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
                    }
                }

                Console.Write("\nDo you want to prescribe a new lab test? (yes/no): ");
                string addNew = Console.ReadLine().Trim().ToLower();

                if (addNew == "yes")
                {
                    Console.Write("Enter Test Name: ");
                    string testName = Console.ReadLine();

                    var newTest = new LabTest
                    {
                        ConsultationId = consultationId,
                        PatientId = patientId,
                        TestName = testName,
                        TestStatus = "Pending"
                    };

                    _repository.AddLabTest(newTest);
                    Console.WriteLine("Lab test prescribed successfully.");
                }
            }
            public void AddLabTest(int consultationId, int patientId)
            {
                Console.Write("Enter Test Name: ");
                string testName = Console.ReadLine();

                var test = new LabTest
                {
                    ConsultationId = consultationId,
                    PatientId = patientId,
                    TestName = testName,
                    TestStatus = "Pending"
                };

                _repository.AddLabTest(test);
                Console.WriteLine("Lab test requested.");
            }

            public void ViewLabResults(int patientId)
            {
                var results = _repository.GetLabTestsByPatientId(patientId);
                if (results.Count == 0)
                {
                    Console.WriteLine("No lab tests found.");
                    return;
                }

                foreach (var test in results)
                {
                    Console.WriteLine($"\nTest: {test.TestName}\nDate: {test.TestDate}\nStatus: {test.TestStatus}\nResult: {test.Result ?? "Pending"}");
                }
            }

            public void MarkAppointmentCompleted(int appointmentId)
            {
                _repository.UpdateAppointmentStatus(appointmentId, "Completed");
            }

            public void UpdateDoctorSchedule(int doctorId)
            {
                _repository.UpdateDoctorSchedule(doctorId);
            }

            public List<ConsultationViewModel> GetConsultationsByPatientId(int patientId)
            {
                return _repository.GetConsultationsByPatientId(patientId);
            }
            public List<string> GetAllMedicines()
            {
                return _repository.GetAllMedicines();
            }

            public List<string> GetAllLabTests()
            {
                return _repository.GetAllLabTests();
            }

            #endregion

        }
    }
 