using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class Consultation
        {
            public int ConsultationId { get; set; }
            public int AppointmentId { get; set; }
            public string PatientHistory { get; set; }
            public string Symptoms { get; set; }
            public string Diagnosis { get; set; }

            public string Treatment { get; set; }
            public string Notes { get; set; }
            public int PatientId { get; set; }
        }
    }
 
