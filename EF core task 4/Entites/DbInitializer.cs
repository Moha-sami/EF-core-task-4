//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace EF_core_task_4.Entites
//{
//    public static class DbInitializer
//    {
//        public static void SeedData(ModelBuilder modelBuilder)
//        {
//            // Seed Branches
//            modelBuilder.Entity<Branch>().HasData(
//                new Branch { Id = 1, Name = "Cairo Main Branch", Code = "NBG-CAI-01", Address = "Downtown Cairo, Egypt" },
//                new Branch { Id = 2, Name = "Alexandria Sporting Branch", Code = "NBG-ALX-02", Address = "Sporting, Alexandria, Egypt" }
//            );

//            // Seed Managers
//            modelBuilder.Entity<Manager>().HasData(
//                new Manager { Id = 1, FirstName = "Ahmed", LastName = "Ali", NationalId = "29001011234567", PhoneNumber = "01001234567", BranchId = 1 },
//                new Manager { Id = 2, FirstName = "Sherif", LastName = "Nasr", NationalId = "28503151234567", PhoneNumber = "01229876543", BranchId = 2 }
//            );

//            // EF Core TPH Seeding requires defining explicit discriminator backing values if using HasData on inherited types
//            modelBuilder.Entity<IndividualCustomer>().HasData(
//                new
//                {
//                    Id = 1,
//                    Email = "m.hassan@email.com",
//                    PhoneNumber = "01114567890",
//                    Address = "Maadi, Cairo",
//                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
//                    FirstName = "Mohamed",
//                    LastName = "Hassan",
//                    NationalId = "29505201234567",
//                    DateOfBirth = new DateTime(1995, 5, 20, 0, 0, 0, DateTimeKind.Utc)
//                },
//                new
//                {
//                    Id = 2,
//                    Email = "s.kamal@email.com",
//                    PhoneNumber = "01557891234",
//                    Address = "Smouha, Alexandria",
//                    CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
//                    FirstName = "Sarah",
//                    LastName = "Kamal",
//                    NationalId = "29910101234567",
//                    DateOfBirth = new DateTime(1999, 10, 10, 0, 0, 0, DateTimeKind.Utc)
//                }
//            );

//            modelBuilder.Entity<BusinessCustomer>().HasData(
//                new
//                {
//                    Id = 3,
//                    Email = "finance@techcorp.com",
//                    PhoneNumber = "0225123456",
//                    Address = "Smart Village, Giza",
//                    CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
//                    CompanyName = "TechCorp Egypt",
//                    TaxRegistrationNumber = "123-456-789",
//                    CommercialRegisterNumber = "998877"
//                }
//            );

//            // Seed Accounts
//            modelBuilder.Entity<Account>().HasData(
//                new Account { Id = 1, AccountNumber = "EG10002000300040005001", Balance = 75000.50m, Type = AccountType.Savings, Status = AccountStatus.Active, Currency = "EGP", BranchId = 1 },
//                new Account { Id = 2, AccountNumber = "EG10002000300040005002", Balance = 1500000.00m, Type = AccountType.Current, Status = AccountStatus.Active, Currency = "EGP", BranchId = 1 },
//                // Joint Account
//                new Account { Id = 3, AccountNumber = "EG10002000300040005003", Balance = 4500.00m, Type = AccountType.Current, Status = AccountStatus.Active, Currency = "EGP", BranchId = 2 }
//            );

//            // Seed Customer-Account Relationships (Many-to-Many Linking)
//            modelBuilder.Entity<CustomerAccount>().HasData(
//                new CustomerAccount { CustomerId = 1, AccountId = 1, IsPrimaryOwner = true }, // Mohamed -> Savings Acct
//                new CustomerAccount { CustomerId = 3, AccountId = 2, IsPrimaryOwner = true }, // TechCorp -> Corporate Acct

//                // Joint Account Setup: Shared by Mohamed and Sarah
//                new CustomerAccount { CustomerId = 1, AccountId = 3, IsPrimaryOwner = true },
//                new CustomerAccount { CustomerId = 2, AccountId = 3, IsPrimaryOwner = false }
//            );

//            // Seed Core Transactions
//            modelBuilder.Entity<Transaction>().HasData(
//                new Transaction { Id = 1, TransactionReference = "TX-99881122", Amount = 5000.00m, Type = TransactionType.Deposit, Timestamp = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc), Description = "Initial Deposit", AccountId = 1 },
//                new Transaction { Id = 2, TransactionReference = "TX-99881123", Amount = 1500000.00m, Type = TransactionType.Deposit, Timestamp = new DateTime(2026, 5, 2, 11, 30, 0, DateTimeKind.Utc), Description = "Business Liquidity Inflow", AccountId = 2 }
//            );
//        }
//    }
//}
