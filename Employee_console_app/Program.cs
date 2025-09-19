using System;

namespace CustomerManagementConsole
{
    class Program
    {
        // Declare an array to hold up to 500 customer records
        static Customer[] customers = new Customer[500];
        static int customerCount = 0;

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                int choice = GetUserChoice();

                switch (choice)
                {
                    case 1:
                        AddCustomer();
                        break;
                    case 2:
                        ViewCustomers();
                        break;
                    case 3:
                        SearchCustomer();
                        break;
                    case 4:
                        UpdateCustomer();
                        break;
                    case 5:
                        DeleteCustomer();
                        break;
                    case 6:
                        exit = ExitApplication();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("Customer Management System");
            Console.WriteLine("===============================");
            Console.WriteLine("1. Add New Customer");
            Console.WriteLine("2. View All Customers");
            Console.WriteLine("3. Search Customer");
            Console.WriteLine("4. Update Customer");
            Console.WriteLine("5. Delete Customer");
            Console.WriteLine("6. Exit");
            Console.WriteLine("===============================");
            Console.Write("Enter your choice: ");
        }

        static int GetUserChoice()
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 6)
            {
                Console.Write("Invalid input. Please enter a valid choice (1-6): ");
            }
            return choice;
        }

        static void AddCustomer()
        {
            if (customerCount >= 500)
            {
                Console.WriteLine("Maximum customer limit reached.");
                return;
            }

            Console.Write("Enter CustomerId (numeric): ");
            int customerId = int.Parse(Console.ReadLine());

            // Ensure unique CustomerId
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i].CustomerId == customerId)
                {
                    Console.WriteLine("CustomerId already exists. Try again.");
                    return;
                }
            }

            Console.Write("Enter Name (max 50 characters): ");
            string name = Console.ReadLine();
            if (string.IsNullOrEmpty(name) || name.Length > 50)
            {
                Console.WriteLine("Name is required and should not exceed 50 characters.");
                return;
            }

            Console.Write("Enter Code (max 10 characters): ");
            string code = Console.ReadLine();
            if (string.IsNullOrEmpty(code) || code.Length > 10)
            {
                Console.WriteLine("Code is required and should not exceed 10 characters.");
                return;
            }

            Console.Write("Enter Address (max 200 characters, optional): ");
            string address = Console.ReadLine();

            // Create and add the new customer
            customers[customerCount] = new Customer
            {
                CustomerId = customerId,
                Name = name,
                Code = code,
                Address = address
            };

            customerCount++;

            Console.WriteLine("Customer added successfully!");
            Console.ReadLine();
        }

        static void ViewCustomers()
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("| CustomerId | Name           | Code  | Address        |");
            Console.WriteLine("---------------------------------------------------------");

            for (int i = 0; i < customerCount; i++)
            {
                Console.WriteLine($"| {customers[i].CustomerId,10} | {customers[i].Name,-15} | {customers[i].Code,-5} | {customers[i].Address,-15} |");
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.ReadLine();
        }

        static void SearchCustomer()
        {
            Console.Write("Enter Name or Code to search: ");
            string searchTerm = Console.ReadLine().ToLower();

            bool found = false;
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i].Name.ToLower().Contains(searchTerm) || customers[i].Code.ToLower().Contains(searchTerm))
                {
                    Console.WriteLine($"Found Customer: {customers[i].CustomerId} - {customers[i].Name} - {customers[i].Code} - {customers[i].Address}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No matching customers found.");
            }
            Console.ReadLine();
        }

        static void UpdateCustomer()
        {
            Console.Write("Enter CustomerId to update: ");
            int customerId = int.Parse(Console.ReadLine());

            int index = FindCustomerById(customerId);
            if (index == -1)
            {
                Console.WriteLine("Customer not found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter new Name (max 50 characters): ");
            string name = Console.ReadLine();
            if (string.IsNullOrEmpty(name) || name.Length > 50)
            {
                Console.WriteLine("Name is required and should not exceed 50 characters.");
                return;
            }

            Console.Write("Enter new Code (max 10 characters): ");
            string code = Console.ReadLine();
            if (string.IsNullOrEmpty(code) || code.Length > 10)
            {
                Console.WriteLine("Code is required and should not exceed 10 characters.");
                return;
            }

            Console.Write("Enter new Address (max 200 characters, optional): ");
            string address = Console.ReadLine();

            customers[index].Name = name;
            customers[index].Code = code;
            customers[index].Address = address;

            Console.WriteLine("Customer updated successfully!");
            Console.ReadLine();
        }

        static void DeleteCustomer()
        {
            Console.Write("Enter CustomerId to delete: ");
            int customerId = int.Parse(Console.ReadLine());

            int index = FindCustomerById(customerId);
            if (index == -1)
            {
                Console.WriteLine("Customer not found.");
                Console.ReadLine();
                return;
            }

            Console.Write($"Are you sure you want to delete Customer {customerId}? (Y/N): ");
            char confirm = Char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirm == 'Y')
            {
                // Shift all records to remove the deleted customer
                for (int i = index; i < customerCount - 1; i++)
                {
                    customers[i] = customers[i + 1];
                }

                customers[customerCount - 1] = null; // Clear the last element
                customerCount--;

                Console.WriteLine("Customer deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
            Console.ReadLine();
        }

        static int FindCustomerById(int customerId)
        {
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i].CustomerId == customerId)
                {
                    return i;
                }
            }
            return -1;
        }

        static bool ExitApplication()
        {
            Console.Write("Are you sure you want to exit? (Y/N): ");
            char confirm = Char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();
            return confirm == 'Y';
        }
    }

    class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
    }
}
