using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace CMS_Project.Model
    {
        public class Appointment
        {
            public int AppointmentId { get; set; }
            public int PatientId { get; set; }
            public int DoctorId { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string AppointmentStatus { get; set; }
        }
    }
 
