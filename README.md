# Bank Management System (EF Core CLI Application)

A robust, crash-proof console-based Bank Management System built using **C#**, **.NET 8**, and **Entity Framework Core (Code-First approach)**. This project implements complex database relationships (One-to-One, One-to-Many, and Many-to-Many), inherits entities using Single-Table Inheritance (TPH), and provides an interactive CLI interface for managing bank operations securely.

---

## 🛠️ Tech Stack & Concepts Covered
* **Framework:** .NET 8 (C#)
* **ORM:** Entity Framework Core (EF Core 8)
* **Database:** Microsoft SQL Server (LocalDB / Express)
* **Architecture:** Code-First Database Design & Migrations
* **Design Patterns:** Table-per-Hierarchy (TPH) Inheritance, Join Tables (Many-to-Many)
* **Interface:** Interactive, Validate-First Command Line Interface (CLI)

---

## 📐 Database Architecture & ERD Mapping

The system architecture is translated directly from an Entity-Relationship Diagram (ERD) into robust C# Classes with EF Core Fluent API mappings.



### 1. **Branch & Manager (One-to-One / `1:1`)**
* Every **Branch** has exactly one **Manager**, and each **Manager** is assigned to one **Branch**. 
* To prevent dependency cycles during migrations and data seeding, the Foreign Key (`BranchId`) is configured strictly inside the `Manager` entity, using cascading deletion.

### 2. **Branch & Account (One-to-Many / `1:N`)**
* A single **Branch** can host multiple bank **Accounts**, but each **Account** belongs strictly to one **Branch**.

### 3. **Customer & Account (Many-to-Many / `M:N`)**
* A **Customer** can hold multiple **Accounts**, and an **Account** can have multiple owners (e.g., joint/shared accounts).
* This is managed through an explicit **Join Entity** (`CustomerAccount`) which tracks relationship-specific attributes:
  * `IsPrimaryOwner` (Boolean indicating whether they are the primary account holder or a Co-Holder).
  * `LinkedAt` (Timestamp of the relationship establishment).

### 4. **Customer Inheritance (Table-per-Hierarchy / TPH)**
* The system utilizes Object-Oriented inheritance. A base `Customer` class is inherited by two specific types:
  * `IndividualCustomer` (Adds `FirstName`, `LastName`, `NationalId`, and `DateOfBirth`).
  * `BusinessCustomer` (Adds `CompanyName`, `TaxRegistrationNumber`, and `CommercialRegisterNumber`).
* EF Core maps this structure into a single `Customers` table utilizing a shadow `Discriminator` column.

---

## 🚀 Key Features

The application operates as an interactive CLI database dashboard containing the following primary workflows:

### 1. **Robust Input Validation (Crash-Proof Loop)**
* Programmed defensively to prevent standard console runtime exceptions. 
* Invalid strings, parsing dates (e.g., wrong birthdate formats), or mismatching numeric types (typing letters where numbers are expected) are caught locally. The system prompts the user gracefully rather than crashing.

### 2. **Add New Customers (Individual & Business)**
* Prompts dynamically for the customer type.
* Prevents data duplication by validating unique constraints (such as `NationalId` and `TaxRegistrationNumber`) at the database interface level before sending queries.

### 3. **Interactive Account Creation & Ownership Assignment**
* Opens a standard **22-character unique bank account**.
* Validates whether the targeted `CustomerId` and `BranchCode` actually exist inside SQL Server before making relational links.
* Prompts the banker to assign ownership: **Primary Owner** or **Co-Holder**.

### 4. **Update Account Status & Relationship Deletions**
* Allows toggling Account Statuses (`Active`, `Suspended`, `Closed`).
* Supports removing a customer's link to an account.
* **Orphan Cleanup Logic:** If a joint account has all its owners deleted, the system automatically sweeps and purges the orphaned account row to preserve relational integrity.

### 5. **Eager Loaded Relational Reporting**
* Leverages `.Include()` and `.ThenInclude()` LINQ statements to load deeply nested data in a single SQL query round-trip.
* Displays a clean console dashboard layout detailing each customer's details alongside all their registered bank accounts, balances, and ownership structures.

---

## 📂 Project Structure

```text
├── Entites
│   ├── Account.cs            # Account entity & AccountType/Status enums
│   ├── Branch.cs             # Branch entity
│   ├── Customer.cs           # Abstract base Customer entity
│   ├── IndividualCustomer.cs # Individual Customer subclass
│   ├── BusinessCustomer.cs   # Business Customer subclass
│   ├── CustomerAccount.cs    # Explicit Join Entity (M:N Link)
│   ├── Manager.cs            # Bank Manager entity
│   └── BankDbContext.cs      # EF Core DbContext, Fluent API & Seeding configuration
├── Migrations                # Automatically generated EF Core Migration scripts
├── Program.cs                # Interactive CLI menu-loop & service logic
└── README.md                 # Project documentation
