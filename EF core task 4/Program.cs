using System;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using EF_core_task_4.Entites; 

namespace EF_core_task_4;

public class Program
{
    public static void Main(string[] args)
    {
       
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        using var context = new BankDbContext();

        
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[خطأ أثناء الاتصال بالداتابيز]: {ex.Message}");
            Console.ResetColor();
            return;
        }

        bool keepRunning = true;
        while (keepRunning)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("====================================================");
            Console.WriteLine("         NATIONAL BANK GROUP - DATA INTERFACE       ");
            Console.WriteLine("====================================================");
            Console.ResetColor();
            Console.WriteLine(" 1. Add a new Customer ");
            Console.WriteLine(" 2. Open a new Account for a Customer ");
            Console.WriteLine(" 3. Update Account Status ");
            Console.WriteLine(" 4. Remove an Account from a Customer ");
            Console.WriteLine(" 5. List all Customers with accounts ");
            Console.WriteLine(" 0. Exit ");
            Console.WriteLine("----------------------------------------------------");
            Console.Write("Select an option [0-5]: ");

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    AddNewCustomer(context);
                    break;
                case "2":
                    OpenNewAccount(context);
                    break;
                case "3":
                    UpdateAccountStatus(context);
                    break;
                case "4":
                    RemoveAccountFromCustomer(context);
                    break;
                case "5":
                    ListAllCustomersWithAccounts(context);
                    break;
                case "0":
                    keepRunning = false;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Closing connection... Goodbye!");
                    Console.ResetColor();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option! Please enter a number from 0 to 5.");
                    Console.ResetColor();
                    PauseForUser();
                    break;
            }
        }
    }

    #region Option 1: Add a new Customer
    private static void AddNewCustomer(BankDbContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("--- [Option 1: Add a New Customer] ---");
        Console.ResetColor();

        string customerType = ReadValidatedString("Enter Customer Type (I for Individual / B for Business): ",
            s => s.ToUpper() == "I" || s.ToUpper() == "B");

        string email = ReadValidatedString("Email Address: ", s => s.Contains("@") && s.Contains("."));
        string phone = ReadValidatedString("Phone Number (Digits only, min 8 digits): ", s => s.All(char.IsDigit) && s.Length >= 8);
        string address = ReadNonEmptyString("Street Address: ");

        if (customerType.ToUpper() == "I")
        {
            // عميل فردي
            string firstName = ReadNonEmptyString("First Name: ");
            string lastName = ReadNonEmptyString("Last Name: ");
            string nationalId = ReadValidatedString("National ID (Exactly 14 digits): ", s => s.Length == 14 && s.All(char.IsDigit));

            // بنشيك لو الرقم القومي ده متسجل قبل كده لمنع التعارض
            if (context.IndividualCustomers.Any(c => c.NationalId == nationalId))
            {
                ShowError("Error: A customer with this National ID already exists in the system!");
                PauseForUser();
                return;
            }

            DateTime dob = ReadValidatedDate("Date of Birth (YYYY-MM-DD): ");

            var individual = new IndividualCustomer
            {
                Email = email,
                PhoneNumber = phone,
                Address = address,
                FirstName = firstName,
                LastName = lastName,
                NationalId = nationalId,
                DateOfBirth = dob
            };

            context.Customers.Add(individual);
            context.SaveChanges();
            ShowSuccess($"Individual Customer '{firstName} {lastName}' added successfully! Generated ID: {individual.Id}");
        }
        else
        {
            // عميل شركات
            string companyName = ReadNonEmptyString("Company Name: ");
            string taxReg = ReadNonEmptyString("Tax Registration Number: ");

            if (context.BusinessCustomers.Any(c => c.TaxRegistrationNumber == taxReg))
            {
                ShowError("Error: A business with this Tax Registration already exists!");
                PauseForUser();
                return;
            }

            string commReg = ReadNonEmptyString("Commercial Register Number: ");

            var business = new BusinessCustomer
            {
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CompanyName = companyName,
                TaxRegistrationNumber = taxReg,
                CommercialRegisterNumber = commReg
            };

            context.Customers.Add(business);
            context.SaveChanges();
            ShowSuccess($"Business Customer '{companyName}' registered successfully! Generated ID: {business.Id}");
        }

        PauseForUser();
    }
    #endregion

    #region Option 2: Open a New Account for a Customer
    private static void OpenNewAccount(BankDbContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("--- [Option 2: Open a New Account] ---");
        Console.ResetColor();

        // 1. التأكد من وجود العميل أولاً
        int customerId = ReadValidatedInt("Enter existing Customer ID: ");
        var customer = context.Customers.Find(customerId);
        if (customer == null)
        {
            ShowError($"Customer with ID {customerId} was not found!");
            PauseForUser();
            return;
        }

        // 2. التأكد من وجود الفرع بكود صحيح
        string branchCode = ReadNonEmptyString("Enter Branch Code (e.g., NBG-CAI-01): ");
        var branch = context.Branches.FirstOrDefault(b => b.Code == branchCode);
        if (branch == null)
        {
            ShowError($"Branch with code '{branchCode}' does not exist!");
            PauseForUser();
            return;
        }

        // 3. التحقق من تفاصيل الحساب الجديد
        string accountNo = ReadValidatedString("Account Number (Exactly 22 characters): ", s => s.Length == 22);
        if (context.Accounts.Any(a => a.AccountNumber == accountNo))
        {
            ShowError($"Account number '{accountNo}' is already registered in the system!");
            PauseForUser();
            return;
        }

        Console.WriteLine("\nAccount Types:\n1. Savings (توفير)\n2. Current (جاري)\n3. FixedDeposit (وديعة لأجل)");
        int typeChoice = ReadValidatedInt("Select Account Type [1-3]: ", val => val >= 1 && val <= 3);
        AccountType selectedType = (AccountType)(typeChoice - 1);

        decimal initialBalance = ReadValidatedDecimal("Enter Initial Deposit Balance (Minimum 0.00): ", val => val >= 0);

        Console.WriteLine("\nOwnership Role:\n1. Primary Owner (مالك أساسي)\n2. Co-Holder (شريك في الحساب)");
        int roleChoice = ReadValidatedInt("Select Role [1-2]: ", val => val >= 1 && val <= 2);
        bool isPrimary = (roleChoice == 1);

        // 4. حفظ الحساب الجديد في الداتابيز
        var account = new Account
        {
            AccountNumber = accountNo,
            Balance = initialBalance,
            Type = selectedType,
            BranchId = branch.Id,
            Status = AccountStatus.Active // الحساب بيفتح نشط تلقائياً
        };

        context.Accounts.Add(account);
        context.SaveChanges(); // بنحفظ عشان يتولد له ID

        // 5. إنشاء الربط في جدول الـ Join Table (CustomerAccount)
        var linkage = new CustomerAccount
        {
            CustomerId = customer.Id,
            AccountId = account.Id,
            IsPrimaryOwner = isPrimary,
            LinkedAt = DateTime.UtcNow
        };

        context.CustomerAccounts.Add(linkage);
        context.SaveChanges();

        ShowSuccess($"Account '{accountNo}' successfully opened and linked to Customer '{customer.Id}' as {(isPrimary ? "Primary Owner" : "Co-Holder")}!");
        PauseForUser();
    }
    #endregion

    #region Option 3: Update Account Status
    private static void UpdateAccountStatus(BankDbContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("--- [Option 3: Update Account Status] ---");
        Console.ResetColor();

        string accountNo = ReadNonEmptyString("Enter Account Number: ");
        int customerId = ReadValidatedInt("Enter Customer ID associated with this account: ");

        // بنتحقق من الربط لتأكيد إن العميل يملك صلاحية على الحساب ده
        var linkage = context.CustomerAccounts
            .Include(ca => ca.Account)
            .FirstOrDefault(ca => ca.CustomerId == customerId && ca.Account.AccountNumber == accountNo);

        if (linkage == null)
        {
            ShowError($"No linkage found between Account '{accountNo}' and Customer ID {customerId}!");
            PauseForUser();
            return;
        }

        var account = linkage.Account;
        Console.WriteLine($"\nCurrent Status of Account '{account.AccountNumber}' is: {account.Status}");
        Console.WriteLine("Choose New Status:\n1. Active (نشط)\n2. Suspended (موقوف)\n3. Closed (مغلق)");
        int choice = ReadValidatedInt("Select Status [1-3]: ", val => val >= 1 && val <= 3);

        // الـ EF Core بيقوم بتحديث الحالة وحفظ التغييرات
        account.Status = (AccountStatus)(choice - 1);
        context.SaveChanges();

        ShowSuccess($"Account '{account.AccountNumber}' status updated successfully to: {account.Status}");
        PauseForUser();
    }
    #endregion

    #region Option 4: Remove an Account from a Customer
    private static void RemoveAccountFromCustomer(BankDbContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("--- [Option 4: Remove Account from Customer] ---");
        Console.ResetColor();

        string accountNo = ReadNonEmptyString("Enter Account Number: ");
        int customerId = ReadValidatedInt("Enter Customer ID: ");

        // بندور على الربط جوه جدول الـ Join
        var linkage = context.CustomerAccounts
            .Include(ca => ca.Account)
            .FirstOrDefault(ca => ca.CustomerId == customerId && ca.Account.AccountNumber == accountNo);

        if (linkage == null)
        {
            ShowError($"Linkage not found between Customer ID {customerId} and Account '{accountNo}'!");
            PauseForUser();
            return;
        }

        // بنمسح سطر الربط
        context.CustomerAccounts.Remove(linkage);
        context.SaveChanges();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"Linkage between Customer {customerId} and Account '{accountNo}' has been removed.");
        Console.ResetColor();

        
        bool hasOtherOwners = context.CustomerAccounts.Any(ca => ca.AccountId == linkage.AccountId);
        if (!hasOtherOwners)
        {
            var orphanedAccount = context.Accounts.Find(linkage.AccountId);
            if (orphanedAccount != null)
            {
                context.Accounts.Remove(orphanedAccount);
                context.SaveChanges();
                Console.WriteLine("Orphaned account (with 0 linked customers) has been deleted from system.");
            }
        }

        ShowSuccess("Operation completed successfully!");
        PauseForUser();
    }
    #endregion

    #region Option 5: List all Customers (with accounts)
    private static void ListAllCustomersWithAccounts(BankDbContext context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("--- [Option 5: Customers & Registered Accounts Overview] ---");
        Console.ResetColor();

        // بنستخدم الـ Eager Loading (Include + ThenInclude) عشان نجيب البيانات كلها في خطوة واحدة وبأعلى كفاءة
        var customers = context.Customers
            .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
            .ToList();

        if (!customers.Any())
        {
            Console.WriteLine("No customers found in the system yet.");
            PauseForUser();
            return;
        }

        foreach (var customer in customers)
        {
            // بنتحقق من نوع الكلاس الموروث باستخدام الـ Pattern Matching
            string customerDetails = customer switch
            {
                IndividualCustomer ind => $"[Individual] Name: {ind.FirstName} {ind.LastName} (National ID: {ind.NationalId})",
                BusinessCustomer bus => $"[Business] Company: {bus.CompanyName} (Tax No: {bus.TaxRegistrationNumber})",
                _ => "Unknown Type"
            };

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nCustomer ID: {customer.Id} | {customerDetails}");
            Console.ResetColor();
            Console.WriteLine($"   Contact: {customer.Email} | Phone: {customer.PhoneNumber}");
            Console.WriteLine($"   Address: {customer.Address}");

            if (!customer.CustomerAccounts.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("   -> No accounts registered for this customer.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("   Registered Accounts:");
                foreach (var ca in customer.CustomerAccounts)
                {
                    var acc = ca.Account;
                    string role = ca.IsPrimaryOwner ? "Primary Owner" : "Co-Holder";

                    Console.Write($"     - Account No: {acc.AccountNumber,-22} | Type: {acc.Type,-12} | Status: ");

                    // تلوين الحالة لتسهيل القراءة
                    if (acc.Status == AccountStatus.Active) Console.ForegroundColor = ConsoleColor.Green;
                    else Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{acc.Status,-10}");
                    Console.ResetColor();

                    Console.WriteLine($" | Balance: {acc.Balance:N2} EGP ({role})");
                }
            }
            Console.WriteLine(new string('-', 75));
        }

        PauseForUser();
    }
    #endregion

    #region Input Validation Helpers (Crash-Proof Handlers)

    private static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input)) return input;
            ShowValidationError("Input cannot be blank. Please try again.");
        }
    }

    private static string ReadValidatedString(string prompt, Func<string, bool> validator)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input) && validator(input)) return input;
            ShowValidationError("Invalid format or criteria. Please try again.");
        }
    }

    private static int ReadValidatedInt(string prompt, Func<int, bool>? validator = null)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int result))
            {
                if (validator == null || validator(result)) return result;
            }
            ShowValidationError("Invalid number format. Please enter a valid integer.");
        }
    }

    private static decimal ReadValidatedDecimal(string prompt, Func<decimal, bool>? validator = null)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                if (validator == null || validator(result)) return result;
            }
            ShowValidationError("Invalid numeric decimal format. Use numbers (e.g., 500.25).");
        }
    }

    private static DateTime ReadValidatedDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return DateTime.SpecifyKind(result, DateTimeKind.Utc); // الـ EF Core بيفضل تخزين التواريخ بترميز Utc
            }
            ShowValidationError("Date must be in exactly YYYY-MM-DD format (e.g., 1995-10-15).");
        }
    }

    private static void PauseForUser()
    {
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey(true);
    }

    private static void ShowValidationError(string error)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"   [Input Error]: {error}");
        Console.ResetColor();
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[SUCCESS]: {message}");
        Console.ResetColor();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR]: {message}");
        Console.ResetColor();
    }

    #endregion
}