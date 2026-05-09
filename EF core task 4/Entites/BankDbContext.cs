using Microsoft.EntityFrameworkCore;
namespace EF_core_task_4.Entites
{


    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }
        public BankDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=NationalBankGroupDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
            }
        }

        //DbSets for each entity
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Manager> Managers { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<IndividualCustomer> IndividualCustomers { get; set; } = null!;
        public DbSet<BusinessCustomer> BusinessCustomers { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // --- 1. BRANCH CONFIGURATION ---
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(150);
                entity.Property(b => b.Code).IsRequired().HasMaxLength(20);
                entity.HasIndex(b => b.Code).IsUnique();
                entity.Property(b => b.Address).IsRequired().HasMaxLength(300);

                // One-to-One with Manager 
                modelBuilder.Entity<Branch>()
        .HasOne(b => b.Manager)
        .WithOne(m => m.Branch)
        .HasForeignKey<Manager>(m => m.BranchId) // الـ Foreign Key موجود في جدول الـ Manager بس
        .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<Branch>().HasData(
                    new Branch { Id = 1, Name = "فرع القاهرة الرئيسي", Code = "NBG-CAI-01", Address = "وسط البلد، القاهرة" },
                    new Branch { Id = 2, Name = "فرع الإسكندرية سبورتنج", Code = "NBG-ALX-02", Address = "Sporting, Alexandria" }
                );


                // --- 2. MANAGER CONFIGURATION ---
                modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.LastName).IsRequired().HasMaxLength(50);
                entity.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);

                entity.Property(m => m.NationalId).IsRequired().HasMaxLength(14);
                entity.HasIndex(m => m.NationalId).IsUnique();
            });
                
                modelBuilder.Entity<Manager>().HasData(
                    new Manager { Id = 1, FirstName = "أحمد", LastName = "علي", NationalId = "29001011234567", PhoneNumber = "01001234567", BranchId = 1 },
                    new Manager { Id = 2, FirstName = "شريف", LastName = "نصر", NationalId = "28503151234567", PhoneNumber = "01229876543", BranchId = 2 }
                );




                // --- 3. CUSTOMER INHERITANCE (TPH) CONFIGURATION ---
                modelBuilder.Entity<Customer>(entity =>
                {
                    entity.HasKey(c => c.Id);
                    entity.Property(c => c.Email).IsRequired().HasMaxLength(150);
                    entity.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
                    entity.Property(c => c.Address).IsRequired().HasMaxLength(300);

                    entity.HasDiscriminator<string>("CustomerType")
                          .HasValue<IndividualCustomer>("Individual")
                          .HasValue<BusinessCustomer>("Business");
                });

                modelBuilder.Entity<IndividualCustomer>(entity =>
                {
                    entity.Property(i => i.FirstName).IsRequired().HasMaxLength(50);
                    entity.Property(i => i.LastName).IsRequired().HasMaxLength(50);
                    entity.Property(i => i.NationalId).IsRequired().HasMaxLength(14);
                    entity.HasIndex(i => i.NationalId).IsUnique();
                });

                modelBuilder.Entity<BusinessCustomer>(entity =>
                {
                    entity.Property(b => b.CompanyName).IsRequired().HasMaxLength(150);
                    entity.Property(b => b.TaxRegistrationNumber).IsRequired().HasMaxLength(30);
                    entity.HasIndex(b => b.TaxRegistrationNumber).IsUnique();
                    entity.Property(b => b.CommercialRegisterNumber).IsRequired().HasMaxLength(30);
                });

                // --- 4. ACCOUNT CONFIGURATION ---
                modelBuilder.Entity<Account>(entity =>
                {
                    entity.HasKey(a => a.Id);
                    entity.Property(a => a.AccountNumber).IsRequired().HasMaxLength(30);
                    entity.HasIndex(a => a.AccountNumber).IsUnique();

                    // Financial precision is vital! 18 digits total, 2 after decimal
                    entity.Property(a => a.Balance).HasPrecision(18, 2).HasDefaultValue(0.00m);
                    entity.Property(a => a.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("EGP");
                    entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
                    entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

                    // One-to-Many: Branch -> Accounts
                    entity.HasOne(a => a.Branch)
                          .WithMany(b => b.Accounts)
                          .HasForeignKey(a => a.BranchId)
                          .OnDelete(DeleteBehavior.Restrict); // Prevent orphan deletes on active branches
                });

                // --- 5. CUSTOMER-ACCOUNT JOINT KEY CONFIGURATION (Many-To-Many) ---
                modelBuilder.Entity<CustomerAccount>(entity =>
                {
                    entity.HasKey(ca => new { ca.CustomerId, ca.AccountId });

                    entity.HasOne(ca => ca.Customer)
                          .WithMany(c => c.CustomerAccounts)
                          .HasForeignKey(ca => ca.CustomerId)
                          .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(ca => ca.Account)
                          .WithMany(a => a.CustomerAccounts)
                          .HasForeignKey(ca => ca.AccountId)
                          .OnDelete(DeleteBehavior.Cascade);
                });

                // --- 6. TRANSACTION CONFIGURATION ---
                modelBuilder.Entity<Transaction>(entity =>
                {
                    entity.HasKey(t => t.Id);
                    entity.Property(t => t.TransactionReference).IsRequired().HasMaxLength(50);
                    entity.HasIndex(t => t.TransactionReference).IsUnique();

                    entity.Property(t => t.Amount).HasPrecision(18, 2);
                    entity.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
                    entity.Property(t => t.Description).HasMaxLength(250);

                    // One-to-Many: Account -> Transactions
                    entity.HasOne(t => t.Account)
                          .WithMany(a => a.Transactions)
                          .HasForeignKey(t => t.AccountId)
                          .OnDelete(DeleteBehavior.Restrict); // Ensure historical ledger cannot be dropped easily
                });
            });
        }
    }
}



