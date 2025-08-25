using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace CMS_Project.Model
    {
        public class Prescription
        {
            public int PrescriptionId { get; set; }
            public int ConsultationId { get; set; }
            public int PatientId { get; set; }
            public string MedicineName { get; set; }
            public string Dosage { get; set; }
            public string Duration { get; set; }
            public string Instructions { get; set; }
        }

    }
 