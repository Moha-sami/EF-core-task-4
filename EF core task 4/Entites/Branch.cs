using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace EF_core_task_4.Entites
{
    public class Branch
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!; // e.g., NBG-CAI-01
        public string Address { get; set; } = null!;

        // Navigation Properties
        //public int ManagerId { get; set; }
        public virtual Manager? Manager { get; set; }

        public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    }


}

