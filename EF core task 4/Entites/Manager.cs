using System;
using System.Collections.Generic;
using System.Text;

namespace EF_core_task_4.Entites
{
    public class Manager
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Navigation Properties
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;
    }
}
