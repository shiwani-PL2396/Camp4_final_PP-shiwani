using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace CMS_Project.Model
    {
        public class Tbl_User
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string UserPassword { get; set; }
            public int RoleId { get; set; }
            public char IsActive { get; set; }
        }
    }
 