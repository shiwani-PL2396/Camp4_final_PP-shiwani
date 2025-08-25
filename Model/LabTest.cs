using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class LabTest
        {
            public int LabTestId { get; set; }
            public int ConsultationId { get; set; }
            public int PatientId { get; set; }
            public string TestName { get; set; }
            public DateTime TestDate { get; set; }
            public string Result { get; set; }
            public string TestStatus { get; set; }
        }

    }
 
