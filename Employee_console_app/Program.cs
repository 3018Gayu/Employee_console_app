using System;
using System.Text.RegularExpressions;

namespace CustomerManagementConsole
{
    // Interface for Person
    interface IPerson
    {
        string Name { get; set; }
        string Address { get; set; }
        string GetDetails();
    }

    // Abstract Person class implementing IPerson
    abstract class Person : IPerson
    {
        private string name;
        private string address;

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
                    throw new ArgumentException("Name is required and max 50 characters.");
                name = value.Trim();
            }
        }

        public string Address
        {
            get => address;
            set
            {
                if (value != null && value.Length > 200)
                    throw new ArgumentException("Address max length is 200 characters.");
                address = value?.Trim() ?? "";
            }
        }

        public virtual string GetDetails()
        {
            return $"Name: {Name}, Address: {Address}";
        }
    }

    // Customer class inherits Person
    class Customer : Person
    {
        private int customerId;
        private string code;

        public int CustomerId
        {
            get => customerId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("CustomerId must be positive.");
                customerId = value;
            }
        }

        public string Code
        {
            get => code;
            set
            {
                if (!IsValidCode(value))
                    throw new ArgumentException("Code must be alphanumeric (letters & digits), max 10 chars, at least one letter and one digit.");
                code = value.Trim();
            }
        }

        private bool IsValidCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 10)
                return false;
            // At least one letter and one digit, only alphanumeric allowed
            return Regex.IsMatch(input, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,10}$");
        }

        public override string GetDetails()
        {
            return $"ID: {CustomerId}, Name: {Name}, Code: {Code}, Address: {Address}";
        }
    }

    // Interface for customer operations
    interface ICustomerOperations
    {
        void AddCustomer(Customer customer);
        void DisplayCustomers(Customer[] customers);
        Customer SearchCustomerById(int customerId);
        Customer[] SearchCustomersByNameOrCode(string searchTerm);
        void UpdateCustomer(Customer customer);
        bool DeleteCustomer(int customerId);
        Customer[] GetAllCustomers();
    }

    // Manager class handling customers
    class CustomerManager : ICustomerOperations
    {
        private Customer[] customers;
        private int customerCount;

        public CustomerManager()
        {
            customers = new Customer[500];
            customerCount = 0;
        }

        public void AddCustomer(Customer customer)
        {
            if (customerCount >= customers.Length)
                throw new InvalidOperationException("Customer limit reached.");
            if (SearchCustomerById(customer.CustomerId) != null)
                throw new ArgumentException("CustomerId already exists.");
            customers[customerCount++] = customer;
        }

        public Customer[] GetAllCustomers()
        {
            Customer[] result = new Customer[customerCount];
            Array.Copy(customers, result, customerCount);
            return result;
        }

        public Customer SearchCustomerById(int customerId)
        {
            for (int i = 0; i < customerCount; i++)
                if (customers[i]?.CustomerId == customerId)
                    return customers[i];
            return null;
        }

        public Customer[] SearchCustomersByNameOrCode(string searchTerm)
        {
            string lower = searchTerm.ToLower().Trim();
            Customer[] tempResults = new Customer[customerCount];
            int found = 0;

            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i] != null &&
                    (customers[i].Name.ToLower().Contains(lower) ||
                     customers[i].Code.ToLower().Contains(lower)))
                {
                    tempResults[found++] = customers[i];
                }
            }

            Customer[] results = new Customer[found];
            Array.Copy(tempResults, results, found);
            return results;
        }

        public void UpdateCustomer(Customer updatedCustomer)
        {
            for (int i = 0; i < customerCount; i++)
                if (customers[i]?.CustomerId == updatedCustomer.CustomerId)
                {
                    customers[i] = updatedCustomer;
                    return;
                }
            throw new ArgumentException("Customer not found.");
        }

        public bool DeleteCustomer(int customerId)
        {
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i]?.CustomerId == customerId)
                {
                    // Shift all after i left by one
                    for (int j = i; j < customerCount - 1; j++)
                        customers[j] = customers[j + 1];
                    customers[customerCount - 1] = null;
                    customerCount--;
                    return true;
                }
            }
            return false;
        }

        public void DisplayCustomers(Customer[] customersToDisplay)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            Console.WriteLine("| {0,10} | {1,-20} | {2,-10} | {3,-40} |", "CustomerId", "Name", "Code", "Address");
            Console.WriteLine("-------------------------------------------------------------------------------------------");

            if (customersToDisplay == null || customersToDisplay.Length == 0)
            {
                Console.WriteLine("|                           No customers found.                                            |");
            }
            else
            {
                foreach (Customer c in customersToDisplay)
                {
                    string addr = c.Address ?? "";
                    if (addr.Length > 40)
                        addr = addr.Substring(0, 37) + "...";
                    Console.WriteLine("| {0,10} | {1,-20} | {2,-10} | {3,-40} |", c.CustomerId, c.Name, c.Code, addr);
                }
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------");
        }
    }

    class Program
    {
        static void Main()
        {
            CustomerManager manager = new CustomerManager();
            AddDefaultCustomers(manager);

            while (true)
            {
                Console.WriteLine("\nCustomer Management Menu");
                Console.WriteLine("1. Add Customer");
                Console.WriteLine("2. View All Customers");
                Console.WriteLine("3. Search Customer");
                Console.WriteLine("4. Update Customer");
                Console.WriteLine("5. Delete Customer");
                Console.WriteLine("6. Exit");
                Console.Write("Enter choice: ");

                string input = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        AddCustomerFlow(manager);
                        break;
                    case "2":
                        manager.DisplayCustomers(manager.GetAllCustomers());
                        break;
                    case "3":
                        SearchCustomerFlow(manager);
                        break;
                    case "4":
                        UpdateCustomerFlow(manager);
                        break;
                    case "5":
                        DeleteCustomerFlow(manager);
                        break;
                    case "6":
                        if (ConfirmAction("Are you sure you want to exit? (yes/no): "))
                        {
                            Console.WriteLine("Exiting...");
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void AddDefaultCustomers(CustomerManager manager)
        {
            Customer[] defaults = new Customer[]
            {
                new Customer { CustomerId = 1, Name = "Alice Johnson", Code = "A1B2C3", Address = "123 Maple Street" },
                new Customer { CustomerId = 2, Name = "Bob Smith", Code = "B2C3D4", Address = "456 Oak Avenue" },
                new Customer { CustomerId = 3, Name = "Carol White", Code = "C3D4E5", Address = "789 Pine Road" },
                new Customer { CustomerId = 4, Name = "David Brown", Code = "D4E5F6", Address = "101 Elm Blvd" },
                new Customer { CustomerId = 5, Name = "Eve Black", Code = "E5F6G7", Address = "202 Birch Lane" },
            };

            foreach (var cust in defaults)
            {
                try
                {
                    manager.AddCustomer(cust);
                }
                catch
                {
                    // Ignore duplicates
                }
            }
        }

        static void AddCustomerFlow(CustomerManager manager)
        {
            try
            {
                Customer customer = new Customer();
                customer.CustomerId = ReadInt("Enter Customer ID: ", val =>
                {
                    if (val <= 0) throw new ArgumentException("Customer ID must be positive.");
                    if (manager.SearchCustomerById(val) != null) throw new ArgumentException("Customer ID already exists.");
                });

                customer.Name = ReadString("Enter Name: ", 1, 50, false);
                customer.Code = ReadCode("Enter Code: ");
                customer.Address = ReadString("Enter Address (optional): ", 0, 200, true);

                manager.AddCustomer(customer);
                Console.WriteLine("Customer added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void SearchCustomerFlow(CustomerManager manager)
        {
            Console.WriteLine("Search by:");
            Console.WriteLine("1. Customer ID");
            Console.WriteLine("2. Name or Code");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine() ?? "";
            Console.WriteLine();

            if (choice == "1")
            {
                int id = ReadInt("Enter Customer ID to search: ");
                Customer found = manager.SearchCustomerById(id);
                if (found == null)
                {
                    Console.WriteLine("No customer found with ID: " + id);
                }
                else
                {
                    manager.DisplayCustomers(new Customer[] { found });
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter Name or Code to search: ");
                string term = Console.ReadLine() ?? "";
                var foundCustomers = manager.SearchCustomersByNameOrCode(term);
                manager.DisplayCustomers(foundCustomers);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        static void UpdateCustomerFlow(CustomerManager manager)
        {
            int id = ReadInt("Enter Customer ID to update: ");
            Customer existing = manager.SearchCustomerById(id);
            if (existing == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.WriteLine("Current details:");
            manager.DisplayCustomers(new Customer[] { existing });

            Console.WriteLine("Which fields do you want to update?");
            Console.WriteLine("Options: name / code / address / all / none");
            Console.Write("Enter your choice: ");
            string choice = (Console.ReadLine() ?? "").Trim().ToLower();

            if (choice == "none")
            {
                Console.WriteLine("No updates made.");
                return;
            }

            try
            {
                if (choice == "name" || choice == "all")
                {
                    existing.Name = ReadString("Enter new Name: ", 1, 50, false);
                }

                if (choice == "code" || choice == "all")
                {
                    existing.Code = ReadCode("Enter new Code: ");
                }

                if (choice == "address" || choice == "all")
                {
                    existing.Address = ReadString("Enter new Address (optional): ", 0, 200, true);
                }

                manager.UpdateCustomer(existing);
                Console.WriteLine("Customer updated successfully!");
                manager.DisplayCustomers(new Customer[] { existing });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update failed: " + ex.Message);
            }
        }

        static void DeleteCustomerFlow(CustomerManager manager)
        {
            int id = ReadInt("Enter Customer ID to delete: ");
            Customer existing = manager.SearchCustomerById(id);
            if (existing == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            manager.DisplayCustomers(new Customer[] { existing });

            if (ConfirmAction($"Are you sure you want to delete Customer ID {id}? (yes/no): "))
            {
                if (manager.DeleteCustomer(id))
                {
                    Console.WriteLine("Customer deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Delete failed.");
                }
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        // Helper functions
        static int ReadInt(string prompt, Action<int>? validator = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value))
                {
                    try
                    {
                        validator?.Invoke(value);
                        return value;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Invalid input: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid integer.");
                }
            }
        }

        static string ReadString(string prompt, int minLen, int maxLen, bool allowEmpty)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim() ?? "";
                if (allowEmpty && input.Length == 0)
                    return "";

                if (input.Length >= minLen && input.Length <= maxLen)
                {
                    return input;
                }
                else
                {
                    Console.WriteLine($"Input length must be between {minLen} and {maxLen} characters.");
                }
            }
        }

        static string ReadCode(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? code = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Code cannot be empty.");
                    continue;
                }
                if (code.Length > 10)
                {
                    Console.WriteLine("Code max length is 10.");
                    continue;
                }
                if (!Regex.IsMatch(code, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,10}$"))
                {
                    Console.WriteLine("Code must be alphanumeric with at least one letter and one digit.");
                    continue;
                }
                return code;
            }
        }

        static bool ConfirmAction(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? answer = Console.ReadLine()?.Trim().ToLower();
                if (answer == "yes" || answer == "y")
                    return true;
                else if (answer == "no" || answer == "n")
                    return false;
                else
                    Console.WriteLine("Please enter yes or no.");
            }
        }
    }
}
