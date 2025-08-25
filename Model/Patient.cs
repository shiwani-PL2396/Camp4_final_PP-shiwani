using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class Patient
        {
            public int PatientId { get; set; }
            public string PatientName { get; set; }
            public string BloodGroup { get; set; }
            public string PatientAddress { get; set; }
            public string PhoneNo { get; set; }
            public string EmergencyContactInfo { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string MaritalStatus { get; set; }
            public string Nationality { get; set; }
        }

    }
 
