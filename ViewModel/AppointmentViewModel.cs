using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CMS_Project.ViewModel
    {
        // ViewModel for displaying appointments
        public class AppointmentViewModel
        {
            public int AppointmentId { get; set; }
            public string PatientName { get; set; }
            public int PatientId { get; set; }
            public string DoctorName { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string AppointmentStatus { get; set; }
            public int DoctorId { get; set; }
        }
    }
 
