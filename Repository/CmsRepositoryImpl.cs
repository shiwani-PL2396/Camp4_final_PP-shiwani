using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using CMS_Project.Model;
using CMS_Project.ViewModel;
using Microsoft.Data.SqlClient;

namespace CMS_Project.Repository
    {
        public class CmsRepositoryImpl : ICmsRepository
        {
            private readonly string winConnString = ConfigurationManager.ConnectionStrings["CsWin"].ConnectionString;

            #region Authenticate
            public LoginResult GetUserRole(string username, string password)
            {
                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();

                    string query = @"SELECT u.UserId, r.RoleName 
                         FROM Tbl_User u
                         JOIN UserRole r ON u.RoleId = r.RoleId
                         WHERE u.Username = @username AND u.UserPassword = @Userpassword";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@Userpassword", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new LoginResult
                                {
                                    UserId = reader.GetInt32(0),
                                    RoleName = reader.GetString(1)
                                };
                            }
                            else
                            {
                                Console.WriteLine("\nUnAuthorized Entry. Retry After 3 sec.\n");
                                Thread.Sleep(3000);
                                Console.WriteLine("Press any key to continue .........");
                                Console.ReadKey();

                                return null;
                            }
                        }
                    }
                }
            }
            #endregion

            #region reception

            #region Add Patient
            public int AddPatient(Patient patient)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_AddPatient", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@PatientName", patient.PatientName);
                cmd.Parameters.AddWithValue("@BloodGroup", patient.BloodGroup ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PatientAddress", patient.PatientAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNo", patient.PhoneNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmergencyContactInfo", patient.EmergencyContactInfo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaritalStatus", patient.MaritalStatus ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Nationality", patient.Nationality ?? (object)DBNull.Value);

                // Add output parameter for PatientId
                SqlParameter outputIdParam = new SqlParameter("@PatientId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)outputIdParam.Value;
            }

            #endregion

            #region Search Patient by ID
            public Patient GetPatientById(int patientId)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_ViewPatientbyid", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Patient
                    {
                        PatientId = (int)reader["PatientId"],
                        PatientName = reader["PatientName"].ToString(),
                        BloodGroup = reader["BloodGroup"]?.ToString(),
                        PatientAddress = reader["PatientAddress"]?.ToString(),
                        PhoneNo = reader["PhoneNo"]?.ToString(),
                        EmergencyContactInfo = reader["EmergencyContactInfo"]?.ToString(),
                        DateOfBirth = reader["DateOfBirth"] != DBNull.Value
        ? ((DateTime)reader["DateOfBirth"]).Date
        : default,
                        Gender = reader["Gender"]?.ToString(),
                        MaritalStatus = reader["MaritalStatus"]?.ToString(),
                        Nationality = reader["Nationality"]?.ToString()
                    };
                }
                return null;
            }
            #endregion

            #region Update Patient
            public void UpdatePatient(Patient patient)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_UpdatePatient", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);
                cmd.Parameters.AddWithValue("@PatientName", patient.PatientName);
                cmd.Parameters.AddWithValue("@BloodGroup", patient.BloodGroup);
                cmd.Parameters.AddWithValue("@PatientAddress", patient.PatientAddress);
                cmd.Parameters.AddWithValue("@PhoneNo", patient.PhoneNo);
                cmd.Parameters.AddWithValue("@EmergencyContactInfo", patient.EmergencyContactInfo);
                cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                cmd.Parameters.AddWithValue("@MaritalStatus", patient.MaritalStatus);
                cmd.Parameters.AddWithValue("@Nationality", patient.Nationality);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            #endregion

            #region Search by Phone Number
            public List<Patient> GetPatientsByPhone(string phone)
            {
                List<Patient> patients = new List<Patient>();

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetPatientsByPhone", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@PhoneNo", phone);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    patients.Add(new Patient
                    {
                        PatientId = (int)reader["PatientId"],
                        PatientName = reader["PatientName"].ToString(),
                        BloodGroup = reader["BloodGroup"]?.ToString(),
                        PatientAddress = reader["PatientAddress"]?.ToString(),
                        PhoneNo = reader["PhoneNo"]?.ToString(),
                        EmergencyContactInfo = reader["EmergencyContactInfo"]?.ToString(),
                        DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : default,
                        Gender = reader["Gender"]?.ToString(),
                        MaritalStatus = reader["MaritalStatus"]?.ToString(),
                        Nationality = reader["Nationality"]?.ToString()
                    });
                }

                return patients;
            }
            #endregion

            #region AddApp
            public int AddAppointment(Appointment appointment)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_AddAppointmentSchedule", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                cmd.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                cmd.Parameters.AddWithValue("@AppointmentStatus", appointment.AppointmentStatus);

                conn.Open();
                object result = cmd.ExecuteScalar(); // get AppointmentId

                return Convert.ToInt32(result);
            }
            #endregion

            #region CancelApp

            public void CancelAppointment(int appointmentId)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_CancelAppointment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            #endregion

            #region AllApp
            public List<AppointmentViewModel> GetAllAppointments()
            {
                List<AppointmentViewModel> appointments = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetAllAppointments", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    appointments.Add(new AppointmentViewModel
                    {
                        AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                        PatientName = reader["PatientName"].ToString(),
                        DoctorName = reader["DoctorName"].ToString(),
                        AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                        AppointmentStatus = reader["AppointmentStatus"].ToString()
                    });
                }

                return appointments;
            }

            public List<Doctor> GetAllDoctors()
            {
                List<Doctor> doctors = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetAllDoctors", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctors.Add(new Doctor
                    {
                        DoctorId = Convert.ToInt32(reader["DoctorId"]),
                        DoctorName = reader["DoctorName"].ToString(),
                    });
                }

                return doctors;
            }
            #endregion

            //All Depts
            #region dept
            public List<Department> GetAllDepartments()
            {
                List<Department> departments = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                string query = "SELECT DepartmentId, DepartmentName FROM Department";
                using SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new Department
                    {
                        DepartmentId = (int)reader["DepartmentId"],
                        DepartmentName = reader["DepartmentName"].ToString()
                    });
                }

                return departments;
            }

            #endregion

            //Active doctor by dept
            #region DoctorBY Dept
            public List<Doctor> GetActiveDoctorsByDepartment(int departmentId)
            {
                List<Doctor> doctors = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                string query = @"
        SELECT DoctorId, DoctorName 
        FROM Doctor 
        WHERE IsActive = 'Y' AND DepartmentId = @DepartmentId";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    doctors.Add(new Doctor
                    {
                        DoctorId = (int)reader["DoctorId"],
                        DoctorName = reader["DoctorName"].ToString()
                    });
                }

                return doctors;
            }
            #endregion

            //Get Doc schedule
            #region doc schedule
            public List<(TimeSpan Start, TimeSpan End)> GetDoctorSchedule(int doctorId, string slotType)
            {
                List<(TimeSpan, TimeSpan)> slots = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                conn.Open();

                string query = @"
        SELECT StartTime, EndTime
        FROM DoctorSchedule
        WHERE DoctorId = @DoctorId AND SlotType = @SlotType AND IsBreak = 0";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                cmd.Parameters.AddWithValue("@SlotType", slotType);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    slots.Add((reader.GetTimeSpan(0), reader.GetTimeSpan(1)));
                }

                return slots;
            }
            #endregion

            //Get Booked Slot
            #region booked slot

            public List<TimeSpan> GetBookedSlots(int doctorId, DateTime date)
            {
                List<TimeSpan> bookedTimes = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                conn.Open();

                string query = @"
        SELECT CAST(AppointmentDate AS TIME)
        FROM Appointment
        WHERE DoctorId = @DoctorId AND CAST(AppointmentDate AS DATE) = @Date";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookedTimes.Add(reader.GetTimeSpan(0));
                }

                return bookedTimes;
            }
            #endregion

            //generate bill
            #region bill
            public void GenerateBill(Bill bill)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                conn.Open();

                // Get Consultation Fee from Doctor table
                string feeQuery = "SELECT ConsultationFee FROM Doctor WHERE DoctorId = @DoctorId";
                decimal consultationFee = 0;

                using (SqlCommand feeCmd = new SqlCommand(feeQuery, conn))
                {
                    feeCmd.Parameters.AddWithValue("@DoctorId", bill.DoctorId);
                    object? feeResult = feeCmd.ExecuteScalar();

                    if (feeResult != null && decimal.TryParse(feeResult.ToString(), out decimal fee))
                    {
                        consultationFee = fee;
                    }
                    else
                    {
                        throw new Exception("Consultation fee not found for this doctor.");
                    }
                }

                // Call the stored procedure
                using SqlCommand cmd = new SqlCommand("sp_GenerateBill", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AppointmentId", bill.AppointmentId);
                cmd.Parameters.AddWithValue("@BillAmount", consultationFee);  // Fee from Doctor table

                cmd.ExecuteNonQuery();
            }


            #endregion

            #region Bill details

            //get bill details
            public BillViewModel GetBillDetails(int appointmentId)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetBillDetailsByAppointmentId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new BillViewModel
                    {
                        AppointmentId = (int)reader["AppointmentId"],
                        PatientName = reader["PatientName"].ToString(),
                        DoctorName = reader["DoctorName"].ToString(),
                        BillAmount = (decimal)reader["BillAmount"],
                        BillDate = (DateTime)reader["BillDate"]
                    };
                }

                return null;
            }
            #endregion

            #region get consultation fee
            public decimal GetConsultationFee(int doctorId)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                conn.Open();

                string query = "SELECT ConsultationFee FROM Doctor WHERE DoctorId = @DoctorId";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                object? result = cmd.ExecuteScalar();

                if (result != null && decimal.TryParse(result.ToString(), out decimal fee))
                {
                    return fee;
                }
                else
                {
                    throw new Exception("Consultation fee not found for the doctor.");
                }
            }
            #endregion

            #region collection report
            public (List<BillViewModel> bills, int totalPatients, decimal totalAmount) GetBillsWithSummary(DateTime startDate, DateTime endDate)
            {
                List<BillViewModel> bills = new();
                int totalPatients = 0;
                decimal totalAmount = 0;

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetBillsByDateRange", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StartDate", startDate.Date);
                cmd.Parameters.AddWithValue("@EndDate", endDate.Date);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                // First result set: Bill list
                while (reader.Read())
                {
                    bills.Add(new BillViewModel
                    {
                        AppointmentId = (int)reader["AppointmentId"],
                        PatientName = reader["PatientName"].ToString(),
                        DoctorName = reader["DoctorName"].ToString(),
                        BillAmount = (decimal)reader["BillAmount"],
                        BillDate = (DateTime)reader["BillDate"]
                    });
                }

                // Second result set: Summary
                if (reader.NextResult() && reader.Read())
                {
                    totalPatients = (int)reader["TotalPatients"];
                    totalAmount = reader["TotalAmount"] != DBNull.Value ? (decimal)reader["TotalAmount"] : 0;
                }

                return (bills, totalPatients, totalAmount);
            }
            #endregion

            #region Duplicate Appoinment
            public Appointment GetDuplicateAppointment(int patientId, int doctorId, DateTime date)
            {
                Appointment appointment = null;

                string query = @"
SELECT TOP 1 * FROM Appointment 
WHERE PatientId = @PatientId 
  AND DoctorId = @DoctorId 
  AND CAST(AppointmentDate AS DATE) = @Date
  AND AppointmentStatus NOT IN ('Cancelled', 'Completed')"; // Exclude both

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    appointment = new Appointment
                    {
                        AppointmentId = (int)reader["AppointmentId"],
                        PatientId = (int)reader["PatientId"],
                        DoctorId = (int)reader["DoctorId"],
                        AppointmentDate = (DateTime)reader["AppointmentDate"],
                        AppointmentStatus = reader["AppointmentStatus"].ToString()
                    };
                }

                return appointment;
            }

            #endregion

            #endregion

            #region doctor
            public (int DoctorId, string DoctorName)? GetDoctorIdAndNameByUserId(int userId)
            {
                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();

                    string query = @"SELECT DoctorId, DoctorName 
                         FROM Doctor 
                         WHERE UserId = @userId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return (reader.GetInt32(0), reader.GetString(1));
                            }
                        }
                    }
                }

                return null; // No doctor found
            }

            public List<AppointmentViewModel> GetTodaysScheduledAppointmentsByDoctorId(int doctorId)
            {
                List<AppointmentViewModel> appointments = new List<AppointmentViewModel>();

                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("GetTodaysScheduledAppointmentsByDoctorId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new AppointmentViewModel
                                {
                                    AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                                    PatientId = Convert.ToInt32(reader["PatientId"]),
                                    PatientName = reader["PatientName"].ToString(),
                                    AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                                    AppointmentStatus = reader["AppointmentStatus"].ToString()
                                });
                            }
                        }
                    }
                }

                return appointments;
            }


            public void UpdateAppointmentStatus(int appointmentId, string status)
            {
                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateAppointmentStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
            }




            public Consultation GetConsultationByAppointmentId(int appointmentId)
            {
                Consultation consultation = null;
                string query = "SELECT * FROM Consultation WHERE AppointmentId = @AppointmentId";

                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                consultation = new Consultation
                                {
                                    ConsultationId = reader.GetInt32(0),
                                    AppointmentId = reader.GetInt32(1),
                                    PatientHistory = reader["PatientHistory"] as string,
                                    Symptoms = reader["Symptoms"] as string,
                                    Diagnosis = reader["Diagnosis"] as string
                                };
                            }
                        }
                    }
                }
                return consultation;
            }

            public List<Prescription> GetPrescriptionsByPatientId(int patientId)
            {
                List<Prescription> prescriptions = new();
                string query = "SELECT * FROM Prescription WHERE PatientId = @PatientId";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prescriptions.Add(new Prescription
                    {
                        PrescriptionId = reader.GetInt32(0),
                        ConsultationId = reader.GetInt32(1),
                        PatientId = reader.GetInt32(2),
                        MedicineName = reader["MedicineName"].ToString(),
                        Dosage = reader["Dosage"].ToString(),
                        Duration = reader["Duration"].ToString(),
                        Instructions = reader["Instructions"].ToString()
                    });
                }
                return prescriptions;
            }

            public List<Prescription> GetPrescriptionsByConsultationId(int consultationId)
            {
                List<Prescription> prescriptions = new();
                string query = "SELECT * FROM Prescription WHERE ConsultationId = @ConsultationId";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prescriptions.Add(new Prescription
                    {
                        PrescriptionId = reader.GetInt32(0),
                        ConsultationId = reader.GetInt32(1),
                        PatientId = reader.GetInt32(2),
                        MedicineName = reader["MedicineName"].ToString(),
                        Dosage = reader["Dosage"].ToString(),
                        Duration = reader["Duration"].ToString(),
                        Instructions = reader["Instructions"].ToString()
                    });
                }
                return prescriptions;
            }

            public void AddPrescription(Prescription prescription)
            {
                string query = @"INSERT INTO Prescription (ConsultationId, PatientId, MedicineName, Dosage, Duration, Instructions)
                         VALUES (@ConsultationId, @PatientId, @MedicineName, @Dosage, @Duration, @Instructions)";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@ConsultationId", prescription.ConsultationId);
                cmd.Parameters.AddWithValue("@PatientId", prescription.PatientId);
                cmd.Parameters.AddWithValue("@MedicineName", prescription.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", prescription.Dosage);
                cmd.Parameters.AddWithValue("@Duration", prescription.Duration);
                cmd.Parameters.AddWithValue("@Instructions", prescription.Instructions);

                cmd.ExecuteNonQuery();
            }

            public void UpdatePrescription(Prescription prescription)
            {
                string query = @"UPDATE Prescription SET
                         MedicineName = @MedicineName,
                         Dosage = @Dosage,
                         Duration = @Duration,
                         Instructions = @Instructions
                         WHERE PrescriptionId = @PrescriptionId";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@MedicineName", prescription.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", prescription.Dosage);
                cmd.Parameters.AddWithValue("@Duration", prescription.Duration);
                cmd.Parameters.AddWithValue("@Instructions", prescription.Instructions);
                cmd.Parameters.AddWithValue("@PrescriptionId", prescription.PrescriptionId);

                cmd.ExecuteNonQuery();
            }

            public int SaveOrUpdateConsultation(Consultation consultation)
            {
                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();

                    // Check if consultation already exists
                    string checkQuery = "SELECT ConsultationId FROM Consultation WHERE AppointmentId = @AppointmentId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@AppointmentId", consultation.AppointmentId);
                        var result = checkCmd.ExecuteScalar();

                        if (result != null) // already exists — update
                        {
                            int consultationId = Convert.ToInt32(result);

                            string updateQuery = @"
                    UPDATE Consultation
                    SET PatientHistory = @PatientHistory,
                        Symptoms = @Symptoms,
                        Diagnosis = @Diagnosis
                    WHERE AppointmentId = @AppointmentId";

                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@PatientHistory", consultation.PatientHistory ?? (object)DBNull.Value);
                                updateCmd.Parameters.AddWithValue("@Symptoms", consultation.Symptoms ?? (object)DBNull.Value);
                                updateCmd.Parameters.AddWithValue("@Diagnosis", consultation.Diagnosis ?? (object)DBNull.Value);
                                updateCmd.Parameters.AddWithValue("@AppointmentId", consultation.AppointmentId);
                                updateCmd.ExecuteNonQuery();
                            }

                            return consultationId;
                        }
                        else // insert new
                        {
                            string insertQuery = @"
                    INSERT INTO Consultation (AppointmentId, PatientHistory, Symptoms, Diagnosis)
                    OUTPUT INSERTED.ConsultationId
                    VALUES (@AppointmentId, @PatientHistory, @Symptoms, @Diagnosis)";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@AppointmentId", consultation.AppointmentId);
                                insertCmd.Parameters.AddWithValue("@PatientHistory", consultation.PatientHistory ?? (object)DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@Symptoms", consultation.Symptoms ?? (object)DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@Diagnosis", consultation.Diagnosis ?? (object)DBNull.Value);

                                int insertedId = (int)insertCmd.ExecuteScalar();
                                return insertedId;
                            }
                        }
                    }
                }
            }



            public void AddLabTest(LabTest labTest)
            {
                string query = @"INSERT INTO LabTest (ConsultationId, PatientId, TestName, TestStatus)
                     VALUES (@ConsultationId, @PatientId, @TestName, @TestStatus)";

                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ConsultationId", labTest.ConsultationId);
                        cmd.Parameters.AddWithValue("@PatientId", labTest.PatientId);
                        cmd.Parameters.AddWithValue("@TestName", labTest.TestName);
                        cmd.Parameters.AddWithValue("@TestStatus", labTest.TestStatus ?? "Pending");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            public List<LabTest> GetLabTestsByPatientId(int patientId)
            {
                List<LabTest> tests = new List<LabTest>();
                string query = "SELECT * FROM LabTest WHERE PatientId = @PatientId";

                using (SqlConnection conn = new SqlConnection(winConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientId", patientId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tests.Add(new LabTest
                                {
                                    LabTestId = reader.GetInt32(0),
                                    ConsultationId = reader.GetInt32(1),
                                    PatientId = reader.GetInt32(2),
                                    TestName = reader["TestName"].ToString(),
                                    TestDate = reader.GetDateTime(4),
                                    Result = reader["Result"] as string,
                                    TestStatus = reader["TestStatus"].ToString()
                                });
                            }
                        }
                    }
                }
                return tests;
            }

            public void UpdateDoctorSchedule(int doctorId)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                conn.Open();

                // Step 1: Show existing schedules
                string fetchQuery = @"
    SELECT ScheduleId, StartTime, EndTime, SlotType, IsBreak 
    FROM DoctorSchedule 
    WHERE DoctorId = @DoctorId";

                using SqlCommand fetchCmd = new SqlCommand(fetchQuery, conn);
                fetchCmd.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = fetchCmd.ExecuteReader();
                var schedules = new List<(int ScheduleId, TimeSpan StartTime, TimeSpan EndTime, string SlotType, bool IsBreak)>();

                Console.WriteLine("\n--- Existing Schedules ---");
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    TimeSpan start = reader.GetTimeSpan(1);
                    TimeSpan end = reader.GetTimeSpan(2);
                    string slot = reader.GetString(3);
                    bool isBreak = reader.GetBoolean(4);

                    schedules.Add((id, start, end, slot, isBreak));

                    Console.WriteLine($"ID: {id}, Time: {start:hh\\:mm} - {end:hh\\:mm}, Slot: {slot}, Break: {(isBreak ? "Yes" : "No")}");
                }
                reader.Close();

                if (schedules.Count == 0)
                {
                    Console.WriteLine("No schedule found for this doctor.");
                    return;
                }

                // Step 2: Choose a schedule to update
                Console.Write("Enter the Schedule ID you want to update: ");
                if (!int.TryParse(Console.ReadLine(), out int scheduleId) || !schedules.Any(s => s.ScheduleId == scheduleId))
                {
                    Console.WriteLine("Invalid Schedule ID.");
                    return;
                }

                // Step 3: Input updated values
                Console.Write("Enter new Start Time (HH:mm): ");
                TimeSpan newStart = TimeSpan.Parse(Console.ReadLine());

                Console.Write("Enter new End Time (HH:mm): ");
                TimeSpan newEnd = TimeSpan.Parse(Console.ReadLine());

                Console.Write("Enter new Slot Type (e.g., Morning, Evening): ");
                string newSlot = Console.ReadLine();

                Console.Write("Is Break? (yes/no): ");
                bool newBreak = Console.ReadLine().Trim().ToLower() == "yes";

                // Step 4: Perform the update
                string updateQuery = @"
    UPDATE DoctorSchedule
    SET StartTime = @StartTime, EndTime = @EndTime, SlotType = @SlotType, IsBreak = @IsBreak
    WHERE ScheduleId = @ScheduleId";

                using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@StartTime", newStart);
                updateCmd.Parameters.AddWithValue("@EndTime", newEnd);
                updateCmd.Parameters.AddWithValue("@SlotType", newSlot);
                updateCmd.Parameters.AddWithValue("@IsBreak", newBreak);
                updateCmd.Parameters.AddWithValue("@ScheduleId", scheduleId);

                int rowsAffected = updateCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("Schedule updated successfully.");
                else
                    Console.WriteLine("Update failed. Please check the schedule ID.");
            }


            public List<ConsultationViewModel> GetConsultationsByPatientId(int patientId)
            {
                List<ConsultationViewModel> consultations = new();

                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_GetConsultationsByPatientId", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    consultations.Add(new ConsultationViewModel
                    {
                        ConsultationId = Convert.ToInt32(reader["ConsultationId"]),
                        AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                        PatientHistory = reader["PatientHistory"]?.ToString(),
                        Symptoms = reader["Symptoms"]?.ToString(),
                        Diagnosis = reader["Diagnosis"]?.ToString(),
                        Treatment = reader["Treatment"]?.ToString(),
                        Notes = reader["Notes"]?.ToString(),
                        DoctorId = Convert.ToInt32(reader["DoctorId"]),
                        AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                    });
                }

                return consultations;
            }
            public int AddConsultation(Consultation consultation)
            {
                using SqlConnection conn = new SqlConnection(winConnString);
                using SqlCommand cmd = new SqlCommand("sp_AddConsultation", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@AppointmentId", consultation.AppointmentId);
                cmd.Parameters.AddWithValue("@PatientHistory", (object?)consultation.PatientHistory ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Symptoms", (object?)consultation.Symptoms ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Diagnosis", (object?)consultation.Diagnosis ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Treatment", (object?)consultation.Treatment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object?)consultation.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PatientId", (object?)consultation.PatientId ?? DBNull.Value);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // Return newly inserted ConsultationId
            }
            public List<string> GetAllMedicines()
            {
                List<string> medicines = new();
                string query = "SELECT MedicineName FROM MedicineMaster";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    medicines.Add(reader.GetString(0));
                }

                return medicines;
            }

            public List<string> GetAllLabTests()
            {
                List<string> tests = new();
                string query = "SELECT TestName FROM LabTestMaster";

                using SqlConnection conn = new(winConnString);
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tests.Add(reader.GetString(0));
                }

                return tests;
            }


            #endregion

        }

    }

 
