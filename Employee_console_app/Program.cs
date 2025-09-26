using System;
using System.Text.RegularExpressions;
namespace CustomerManagementConsole
{
    abstract class Person
    {
        private string name = string.Empty;
        private string? address;
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
                    throw new ArgumentException("Name is required and max 50 characters.");
                name = value;
            }
        }
        public string? Address
        {
            get => address;
            set
            {
                if (value != null && value.Length > 200)
                    throw new ArgumentException("Address max length is 200 characters.");
                address = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }
    }
    class Customer : Person
    {
        private int customerId;
        private string code = string.Empty;
        public int CustomerId
        {
            get => customerId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("CustomerId must be positive numeric.");
                customerId = value;
            }
        }
        public string Code
        {
            get => code;
            set
            {
                if (!IsValidCode(value))
                    throw new ArgumentException("Code must be alphanumeric, max 10 chars, with at least one letter and one digit.");
                code = value;
            }
        }
        private static bool IsValidCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 10) return false;
            return Regex.IsMatch(input, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,10}$");
        }
    }

    // --- Interface and Manager (Abstraction and Implementation) ---

    interface ICustomerOperations
    {
        void AddCustomer(Customer customer);
        void DisplayCustomers(Customer[] customersToDisplay);
        Customer? SearchCustomerById(int customerId);
        Customer[] SearchCustomersByNameOrCode(string searchTerm);
        void UpdateCustomer(Customer customer);
        bool DeleteCustomer(int customerId);
        Customer[] GetAllCustomers();
        Customer[] GetCustomersSortedByName(bool ascending);
        Customer[] GetCustomersSortedById(bool ascending);
    }
    class CustomerManager : ICustomerOperations
    {
        private readonly Customer?[] customers = new Customer?[500];
        private int customerCount;
        public CustomerManager(int capacity)
        {
            // Initialized above for conciseness, but the constructor can enforce capacity
        }

        public void AddCustomer(Customer customer)
        {
            if (customerCount >= customers.Length)
                throw new InvalidOperationException("Maximum customer limit reached (500).");
            if (SearchCustomerById(customer.CustomerId) != null)
                throw new ArgumentException($"CustomerId {customer.CustomerId} already exists.");
            customers[customerCount++] = customer;
        }

        public Customer[] GetAllCustomers()
        {
            Customer[] activeCustomers = new Customer[customerCount];
            Array.Copy(customers, activeCustomers, customerCount);
            return activeCustomers!;
        }

        public Customer? SearchCustomerById(int customerId) =>
            Array.Find(customers, c => c?.CustomerId == customerId);


        public Customer[] SearchCustomersByNameOrCode(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Array.Empty<Customer>();
            string lowerTerm = searchTerm.ToLower().Trim();
            int foundCount = 0;
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i] != null &&
                    (customers[i]!.Name.ToLower().Contains(lowerTerm) ||
                     customers[i]!.Code.ToLower().Contains(lowerTerm)))
                {
                    foundCount++;
                }
            }

            Customer[] results = new Customer[foundCount];
            int resultIndex = 0;
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i] != null &&
                    (customers[i]!.Name.ToLower().Contains(lowerTerm) ||
                     customers[i]!.Code.ToLower().Contains(lowerTerm)))
                {
                    results[resultIndex++] = customers[i]!;
                }
            }
            return results;
        }

        public void UpdateCustomer(Customer updatedCustomer)
        {
            int index = Array.FindIndex(customers, c => c?.CustomerId == updatedCustomer.CustomerId);

            if (index == -1 || index >= customerCount)
                throw new ArgumentException("Customer not found to update.");
            customers[index]!.Name = updatedCustomer.Name;
            customers[index]!.Code = updatedCustomer.Code;
            customers[index]!.Address = updatedCustomer.Address;
        }

        public bool DeleteCustomer(int customerId)
        {
            int index = Array.FindIndex(customers, c => c?.CustomerId == customerId);
            if (index == -1 || index >= customerCount) return false;

            if (index < customerCount - 1)
            {
                Array.Copy(customers, index + 1, customers, index, customerCount - 1 - index);
            }

            customers[customerCount - 1] = null;
            customerCount--;
            return true;
        }

        private static void Swap(Customer?[] arr, int i, int j)
        {
            Customer? temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        private Customer[] SortCustomers(Func<Customer, Customer, int> comparisonLogic, bool ascending)
        {
            Customer?[] sorted = GetAllCustomers();
            int count = sorted.Length;
            // Bubble Sort implementation
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - 1 - i; j++)
                {
                    Customer c1 = sorted[j]!;
                    Customer c2 = sorted[j + 1]!;
                    int comparison = comparisonLogic(c1, c2);
                    bool shouldSwap = ascending ? (comparison > 0) : (comparison < 0);
                    if (shouldSwap)
                    {
                        Swap(sorted, j, j + 1);
                    }
                }
            }
            return sorted!;
        }

        public Customer[] GetCustomersSortedByName(bool ascending) =>
            SortCustomers((c1, c2) =>
                string.Compare(c1.Name, c2.Name, StringComparison.OrdinalIgnoreCase), ascending);

        public Customer[] GetCustomersSortedById(bool ascending) =>
            SortCustomers((c1, c2) =>
                c1.CustomerId.CompareTo(c2.CustomerId), ascending);

        public void DisplayCustomers(Customer[] customersToDisplay)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------------");
            Console.WriteLine("| CustomerId | Name                  | Code       | Address                         |");
            Console.WriteLine("----------------------------------------------------------------------------------");

            if (customersToDisplay == null || customersToDisplay.Length == 0)
            {
                Console.WriteLine("| No customers found.                                                            |");
            }
            else
            {
                foreach (Customer c in customersToDisplay)
                {
                    string displayAddress = c.Address ?? string.Empty;
                    if (displayAddress.Length > 30) displayAddress = displayAddress.Substring(0, 27) + "...";
                    Console.WriteLine($"| {c.CustomerId,10} | {c.Name,-21} | {c.Code,-10} | {displayAddress,-30} |");
                }
            }
            Console.WriteLine("----------------------------------------------------------------------------------");
        }
    }

    class Program
    {
        static readonly CustomerManager customerManager = new(500);

        static void Main()
        {
            InitializeCustomers();
            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                int choice = GetValidatedIntInput("Enter your choice: ", 1, 7, "Invalid choice. Please enter 1-7: ");
                switch (choice)
                {
                    case 1: AddCustomerUI(); break;
                    case 2: ViewAllCustomersUI(); break;
                    case 3: SortCustomersUI(); break;
                    case 4: SearchCustomerUI(); break;
                    case 5: UpdateCustomerUI(); break;
                    case 6: DeleteCustomerUI(); break;
                    case 7: exit = ExitApplication(); break;
                }
            }
        }

        private static int GetValidatedIntInput(string prompt, int min, int max, string errorMsg)
        {
            int choice;
            Console.Write(prompt);            
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < min || choice > max)
            {
                Console.WriteLine(errorMsg);
                Console.Write(prompt);
            }
            return choice;
        }

        private static int GetUserIdInput(string prompt) =>
            GetValidatedIntInput(prompt, 1, int.MaxValue, "Invalid ID. Please enter a positive numeric CustomerId: ");

        private static string GetValidatedStringProperty(string prompt, Action<string> validationAction)
        {
            string value = string.Empty;
            bool valid = false;
            while (!valid)
            {
                Console.Write(prompt);
                value = Console.ReadLine() ?? string.Empty;
                try
                {
                    validationAction(value);
                    valid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Validation Error: {ex.Message}");
                }
            }
            return value;
        }


        private static void InitializeCustomers()
        {
            try
            {
                customerManager.AddCustomer(new Customer { CustomerId = 101, Name = "Alice Johnson", Code = "AJ101", Address = "123 Maple St." });
                customerManager.AddCustomer(new Customer { CustomerId = 102, Name = "Bob Smith", Code = "BS102", Address = "456 Oak Ave." });
                customerManager.AddCustomer(new Customer { CustomerId = 103, Name = "Carol Davis", Code = "CD103", Address = "789 Pine Rd." });
                customerManager.AddCustomer(new Customer { CustomerId = 104, Name = "David Miller", Code = "DM104", Address = "321 Birch Ln." });
                customerManager.AddCustomer(new Customer { CustomerId = 105, Name = "Eva Brown", Code = "EB105", Address = "654 Cedar Blvd." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization Error: {ex.Message}");
            }
        }

        private static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("      Customer Management System");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Add New Customer");
            Console.WriteLine("2. View All Customers (Default Order)");
            Console.WriteLine("3. Sort Customers");
            Console.WriteLine("4. Search Customer (by Name or Code)");
            Console.WriteLine("5. Update Customer");
            Console.WriteLine("6. Delete Customer");
            Console.WriteLine("7. Exit");
            Console.WriteLine("========================================");
        }

        private static void AddCustomerUI()
        {
            Console.Clear();
            Console.WriteLine("--- Add New Customer ---");
            try
            {
                int id = GetUserIdInput("Enter CustomerId: ");
                if (customerManager.SearchCustomerById(id) != null)
                {
                    Console.WriteLine($"CustomerId {id} already exists. Cannot add.");
                    return;
                }

                string name = GetValidatedStringProperty("Enter Name (required, max 50 chars): ", val => new Customer { Name = val });
                string code = GetValidatedStringProperty("Enter Code (alphanumeric, letter+digit, max 10 chars): ", val => new Customer { Code = val });

                Console.Write("Enter Address (optional, max 200 chars): ");
                string address = Console.ReadLine() ?? string.Empty;

                Customer newCustomer = new Customer
                {
                    CustomerId = id,
                    Name = name,
                    Code = code,
                    Address = address
                };

                customerManager.AddCustomer(newCustomer);
                Console.WriteLine("Customer added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add customer: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ViewAllCustomersUI()
        {
            customerManager.DisplayCustomers(customerManager.GetAllCustomers());
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }

        private static void SortCustomersUI()
        {
            Console.Clear();
            Console.WriteLine("--- Sort Customers ---");

            int fieldChoice = GetValidatedIntInput("Sort Field (1. ID, 2. Name): ", 1, 2, "Invalid input. Please enter 1 or 2: ");
            int directionChoice = GetValidatedIntInput("Sort Direction (1. Asc, 2. Desc): ", 1, 2, "Invalid input. Please enter 1 or 2: ");
            bool ascending = (directionChoice == 1);

            Customer[] customersToDisplay = (fieldChoice == 1)
                ? customerManager.GetCustomersSortedById(ascending)
                : customerManager.GetCustomersSortedByName(ascending);

            customerManager.DisplayCustomers(customersToDisplay);
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }

        private static void SearchCustomerUI()
        {
            Console.Clear();
            Console.WriteLine("--- Search Customer ---");
            Console.Write("Enter Name or Code to search: ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            var results = customerManager.SearchCustomersByNameOrCode(searchTerm);

            Console.WriteLine($"Found {results.Length} matching customer(s):");
            customerManager.DisplayCustomers(results);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void UpdateCustomerUI()
        {
            Console.Clear();
            Console.WriteLine("--- Update Customer ---");
            try
            {
                int id = GetUserIdInput("Enter CustomerId to update: ");
                Customer? customer = customerManager.SearchCustomerById(id);

                if (customer == null)
                {
                    Console.WriteLine("Customer not found.");
                    return;
                }

                Console.WriteLine($"\n--- Current Details for ID {id} ---");
                Console.WriteLine($"Name: {customer.Name}");
                Console.WriteLine($"Code: {customer.Code}");
                Console.WriteLine($"Address: {customer.Address ?? "N/A"}");
                Console.WriteLine("-----------------------------------\n");

                Console.WriteLine("What would you like to update?");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Code");
                Console.WriteLine("3. All Fields (Name, Code, and Address)");
                Console.WriteLine("4. Cancel");

                int updateChoice = GetValidatedIntInput("Enter your choice (1-4): ", 1, 4, "Invalid choice. Please enter 1-4: ");

                if (updateChoice == 4)
                {
                    Console.WriteLine("Update cancelled.");
                    return;
                }
                    Customer tempCustomer = new Customer
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Code = customer.Code,
                    Address = customer.Address
                };
                switch (updateChoice)
                {
                    case 1: 
                        UpdateFieldWithValidation("Name", customer.Name, val => tempCustomer.Name = val);
                        break;
                    case 2: 
                        UpdateFieldWithValidation("Code", customer.Code, val => tempCustomer.Code = val);
                        break;
                    case 3: 
                        UpdateFieldWithValidation("Name", customer.Name, val => tempCustomer.Name = val);
                        UpdateFieldWithValidation("Code", customer.Code, val => tempCustomer.Code = val);
                        UpdateAddressField(customer.Address, tempCustomer);
                        break;
                }

                customerManager.UpdateCustomer(tempCustomer);
                Console.WriteLine("\nCustomer updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUpdate failed: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void UpdateAddressField(string? currentAddress, Customer tempCustomer)
        {
            Console.Write($"Update Address (Current: {currentAddress ?? "N/A"}). Enter new value [or press Enter to clear]: ");
            string? newAddress = Console.ReadLine();

            if (newAddress != null)
            {
                try
                {
                    tempCustomer.Address = newAddress;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Validation Error: {ex.Message}");
                }
            }
        }
        private static void UpdateFieldWithValidation(string fieldName, string currentValue, Action<string> updateAction)
        {
            string prompt = $"Update {fieldName} (Current: {currentValue}). Enter new {fieldName}: ";
            GetValidatedStringProperty(prompt, updateAction);
        }

        private static void DeleteCustomerUI()
        {
            Console.Clear();
            Console.WriteLine("--- Delete Customer ---");
            try
            {
                int id = GetUserIdInput("Enter CustomerId to delete: ");

                Customer? customer = customerManager.SearchCustomerById(id);
                if (customer == null)
                {
                    Console.WriteLine("Customer not found.");
                    return;
                }

                Console.Write($"Are you sure you want to delete Customer {id} ('{customer.Name}')? (Y/N): ");
                char confirm = Char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();

                if (confirm == 'Y')
                {
                    if (customerManager.DeleteCustomer(id))
                    {
                        Console.WriteLine("Customer deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to delete customer (Internal error).");
                    }
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deletion failed: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static bool ExitApplication()
        {
            Console.Clear();
            Console.Write("Are you sure you want to exit the Customer Management System? (Y/N): ");
            char confirm = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirm == 'Y')
            {
                Console.WriteLine("Exiting application. Goodbye!");
                return true;
            }
            else
            {
                Console.WriteLine("Exit cancelled.");
                return false;
            }
        }
    }
}