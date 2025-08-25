using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class Doctor
        {
            public int DoctorId { get; set; }
            public string DoctorName { get; set; }
            public int? DepartmentId { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public char IsActive { get; set; }
        }

    }
 
