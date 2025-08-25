using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class Bill
        {
            public int BillId { get; set; }
            public int AppointmentId { get; set; }
            public decimal BillAmount { get; set; }
            public DateTime BillDate { get; set; }
            public int PatientId { get; set; }
            public int DoctorId { get; set; }
        }

    }
 
