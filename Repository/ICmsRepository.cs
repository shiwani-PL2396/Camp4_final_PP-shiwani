using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS_Project.Model;
using CMS_Project.ViewModel;
 

namespace CMS_Project.Repository
    {
        public interface ICmsRepository
        {
            LoginResult GetUserRole(string username, string password);

            #region reception
            public int AddPatient(Patient patient);
            public void UpdatePatient(Patient patient);
            public Patient GetPatientById(int id);
            public List<Patient> GetPatientsByPhone(string phone);



            public int AddAppointment(Appointment appointment);
            public void CancelAppointment(int appointmentId);

            #endregion

            #region doctor
            public List<AppointmentViewModel> GetTodaysScheduledAppointmentsByDoctorId(int doctorId);

            public void UpdateAppointmentStatus(int appointmentId, string status);



            public Consultation GetConsultationByAppointmentId(int appointmentId);
            public int SaveOrUpdateConsultation(Consultation consultation);
            //public void AddPrescription(Prescription prescription);
            public void AddLabTest(LabTest labTest);
            public List<LabTest> GetLabTestsByPatientId(int patientId);

            public List<Prescription> GetPrescriptionsByPatientId(int patientId);
            public List<Prescription> GetPrescriptionsByConsultationId(int consultationId);
            public void UpdatePrescription(Prescription prescription);

            public void UpdateDoctorSchedule(int doctorId);
            List<ConsultationViewModel> GetConsultationsByPatientId(int patientId);

            public void AddPrescription(Prescription newPrescription);


            public List<string> GetAllMedicines();
            public List<string> GetAllLabTests();

            #endregion
        }
    }
 