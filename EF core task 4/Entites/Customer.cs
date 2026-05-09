using System;
using System.Collections.Generic;
using System.Text;

namespace EF_core_task_4.Entites
{
    public abstract class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Many-to-Many Join entity navigation
        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new HashSet<CustomerAccount>();
    }

    public class IndividualCustomer : Customer
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }

    public class BusinessCustomer : Customer
    {
        public string CompanyName { get; set; } = null!;
        public string TaxRegistrationNumber { get; set; } = null!;
        public string CommercialRegisterNumber { get; set; } = null!;
    }
}
