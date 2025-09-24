using System;
using System.Text.RegularExpressions;

namespace CustomerManagementConsole
{
    class Program
    {
        static Customer[] customers = new Customer[500];
        static int customerCount = 0;

        static void Main(string[] args)
        {
            InitializeCustomers();

            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                int choice = GetUserChoice();

                switch (choice)
                {
                    case 1: AddCustomer();
                        break;
                    case 2: ViewCustomers();
                        break;
                    case 3: SearchCustomer();
                        break;
                    case 4: UpdateCustomer();
                        break;
                    case 5: DeleteCustomer();
                        break;
                    case 6: exit = ExitApplication();
                        break;
                    default: Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static void InitializeCustomers()
        {
            customers[0] = new Customer { CustomerId = 101, Name = "Alice Johnson", Code = "AJ101", Address = "123 Maple St." };
            customers[1] = new Customer { CustomerId = 102, Name = "Bob Smith", Code = "BS102", Address = "456 Oak Ave." };
            customers[2] = new Customer { CustomerId = 103, Name = "Carol Davis", Code = "CD103", Address = "789 Pine Rd." };
            customers[3] = new Customer { CustomerId = 104, Name = "David Miller", Code = "DM104", Address = "321 Birch Ln." };
            customers[4] = new Customer { CustomerId = 105, Name = "Eva Brown", Code = "EB105", Address = "654 Cedar Blvd." };
            customers[5] = new Customer { CustomerId = 106, Name = "Frank Wilson", Code = "FW106", Address = "987 Spruce Ct." };
            customers[6] = new Customer { CustomerId = 107, Name = "Grace Lee", Code = "GL107", Address = "147 Elm St." };
            customers[7] = new Customer { CustomerId = 108, Name = "Henry Clark", Code = "HC108", Address = "258 Willow Dr." };
            customers[8] = new Customer { CustomerId = 109, Name = "Ivy Martinez", Code = "IM109", Address = "369 Aspen Way" };
            customers[9] = new Customer { CustomerId = 110, Name = "Jack Turner", Code = "JT110", Address = "741 Poplar Pl." };

            customerCount = 10;
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
                Console.ReadKey();
                return;
            }

            Console.Write("Enter CustomerId (numeric): ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid CustomerId. Must be numeric.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i].CustomerId == customerId)
                {
                    Console.WriteLine("CustomerId already exists. Try again.");
                    Console.ReadKey();
                    return;
                }
            }

            Console.Write("Enter Name (required, max 50 characters): ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            {
                Console.WriteLine("Invalid name. Max 50 characters.");
                Console.ReadKey();
                return;
            }

            string code;
            do
            {
                Console.Write("Enter Code (alphanumeric only, must include at least one letter and one number, max 10 characters): ");
                code = Console.ReadLine();
                if (!IsValidCode(code))
                {
                    Console.WriteLine("Invalid Code! Must be alphanumeric and include both letters and digits.");
                }
                else break;
            } while (true);

            Console.Write("Enter Address (max 200 characters, optional): ");
            string address = Console.ReadLine();
            if (address.Length > 200)
            {
                Console.WriteLine("Address cannot exceed 200 characters.");
                Console.ReadKey();
                return;
            }

            customers[customerCount++] = new Customer
            {
                CustomerId = customerId,
                Name = name,
                Code = code,
                Address = address
            };

            Console.WriteLine("Customer added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static bool IsValidCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 10)
                return false;

            Regex regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,10}$");
            return regex.IsMatch(input);
        }

        static void ViewCustomers()
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("| CustomerId | Name            | Code       | Address          |");
            Console.WriteLine("---------------------------------------------------------------");

            for (int i = 0; i < customerCount; i++)
            {
                Console.WriteLine($"| {customers[i].CustomerId,10} | {customers[i].Name,-15} | {customers[i].Code,-10} | {customers[i].Address,-15} |");
            }
            //10 → Align right, with a width of 10 characters.

            //-15 → Align left, with a width of 15 characters.
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
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
                    Console.WriteLine($"Found: {customers[i].CustomerId} - {customers[i].Name} - {customers[i].Code} - {customers[i].Address}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No matching customers found.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateCustomer()
        {
            Console.Write("Enter CustomerId to update: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId)) // int.TryParse tries to convert it to an integer--> retrn true / false, storing it in customerId. out is keywod (defined inside method and return to caller)
            {
                Console.WriteLine("Invalid input. Must be a number.");
                Console.ReadKey();
                return;
            }

            int index = FindCustomerById(customerId);
            if (index == -1)
            {
                Console.WriteLine("Customer not found.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nWhat would you like to update?");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Code");
            Console.WriteLine("3. Address");
            Console.WriteLine("4. All");
            Console.WriteLine("5. Cancel");
            Console.Write("Enter your choice (1-5): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": UpdateName(index); break;
                case "2": UpdateCode(index); break;
                case "3": UpdateAddress(index); break;
                case "4": UpdateName(index); UpdateCode(index); UpdateAddress(index); break;
                case "5": Console.WriteLine("Update cancelled."); break;
                default: Console.WriteLine("Invalid choice."); break;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void UpdateName(int index)
        {
            Console.Write("Enter new Name (max 50 characters): ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            {
                Console.WriteLine("Invalid name. Update failed.");
                return;
            }

            customers[index].Name = name;
            Console.WriteLine("Name updated successfully.");
        }

        static void UpdateCode(int index)
        {
            string code;
            do
            {
                Console.Write("Enter new Code (alphanumeric only, must include letters and digits, max 10 characters): ");
                code = Console.ReadLine();

                if (!IsValidCode(code))
                {
                    Console.WriteLine("Invalid Code! Must include both letters and numbers.");
                }
                else
                {
                    customers[index].Code = code;
                    Console.WriteLine("Code updated successfully.");
                    break;
                }
            } while (true);
        }

        static void UpdateAddress(int index)
        {
            Console.Write("Enter new Address (max 200 characters, optional): ");
            string address = Console.ReadLine();
            if (address.Length > 200)
            {
                Console.WriteLine("Address too long. Update failed.");
                return;
            }

            customers[index].Address = address;
            Console.WriteLine("Address updated successfully.");
        }

        static void DeleteCustomer()
        {
            Console.Write("Enter CustomerId to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid input. Must be a number.");
                Console.ReadKey();
                return;
            }

            int index = FindCustomerById(customerId);
            if (index == -1)
            {
                Console.WriteLine("Customer not found.");
                Console.ReadKey();
                return;
            }

            Console.Write($"Are you sure you want to delete Customer {customerId}? (Y/N): ");
            char confirm = Char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirm == 'Y')
            {
                for (int i = index; i < customerCount - 1; i++)
                {
                    customers[i] = customers[i + 1];
                }

                customers[customerCount - 1] = null;
                customerCount--;

                Console.WriteLine("Customer deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static int FindCustomerById(int customerId)
        {
            for (int i = 0; i < customerCount; i++)
            {
                if (customers[i].CustomerId == customerId)
                    return i;
            }
            return -1;
        }

        static bool ExitApplication()
        {
            Console.Write("Are you sure you want to exit? (Y/N): ");
            char confirm = Char.ToUpper(Console.ReadKey().KeyChar);
            //To store or work with the actual letter (like 'A' to 'Z' or 'a' to 'z'), you need to access the .KeyChar property of that object.
            Console.WriteLine();

            if (confirm == 'Y')
            {
                Console.WriteLine("Exiting application...");
                return true;
            }
            else
            {
                Console.WriteLine("Exit cancelled.");
                return false;
            }
        }
    }

    class Customer
    {
        public int CustomerId { get; set; } //auto-implemented property in C#.
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
    }
}
