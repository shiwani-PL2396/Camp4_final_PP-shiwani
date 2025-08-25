using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS_Project.Model;
using CMS_Project.ViewModel;


namespace CMS_Project.Service
{
    public interface ICmsService
    {
 
            LoginResult AuthenticateUser(string username, string password);

            #region reception
            public int AddPatientAsync();
            public void UpdatePatientAsync();
            public void SearchPatientByIdAsync();
            public void SearchPatientsByPhone();

            public void FixAppointmentAsync(int patid);
            public void CollectionReport();
            public void FilterAppointmentsByDate();
            #endregion

            #region doctor
            (int DoctorId, string DoctorName)? GetDoctorIdAndNameByUserId(int userId);

            public List<AppointmentViewModel> GetTodaysScheduledAppointmentsByDoctorId(int doctorId);

            List<ConsultationViewModel> GetConsultationsByPatientId(int patientId);

            public Patient GetPatientById(int patientId);
            public int ManageConsultation(int appointmentId, int patientid);
            public void AddPrescription(int patientId);
            public void AddLabTest(int consultationId, int patientId);
            public void ViewLabResults(int patientId);
            public void MarkAppointmentCompleted(int appointmentId);
            public void ManageLabTests(int consultationId, int patientId);

            public void UpdateDoctorSchedule(int doctorId);
            public int SearchPatientById();
            public List<string> GetAllMedicines();
            public List<string> GetAllLabTests();

            #endregion
        }
    }
 
