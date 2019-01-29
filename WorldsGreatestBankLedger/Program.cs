using System;
using System.Data;

namespace WorldsGreatestBankLedger
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer cust = new Customer(); //create new customer obj
            
            string existingCustomer = "";
            bool valid = false; // set to false for the While loop to validate answer
            Console.WriteLine("Welcome to the Worlds Greatest Banking Ledger!");
            Console.WriteLine("");
            while (valid == false) //loop until given a valid response of yes or no
            {
                Console.WriteLine("Do you already have an account? (Yes or No)");
                existingCustomer = Console.ReadLine();
                if (existingCustomer.Trim().ToUpper() == "YES")
                {
                    valid = true; //good answer, get out of loop
                    Login(cust);
                }
                else if (existingCustomer.Trim().ToUpper() == "NO")
                {
                    valid = true; //good answer, get out of loop
                    CreateAccount(cust);
                }
                else //bad answer, repeat loop
                {
                    Console.WriteLine("Im sorry, I dont understand your answer.  Please respond with Yes or No");
                }
            }
        }

        #region Account creation and login methods
        private static void CreateAccount(Customer cust)
        {
            string pw = "";
            string pwConf = "";

            Console.WriteLine("Let me get some basic information from you...");

            //get name
            Console.WriteLine("What is your first name?");
            cust.SetCustomerFirstName(Console.ReadLine());
            Console.WriteLine("What is your last name?");
            cust.SetCustomerLastName(Console.ReadLine());

            //set Username
            Console.WriteLine("What would you like your User Name to be?");
            cust.SetCustomerUserName(Console.ReadLine().Trim());

            //check if Username exists in accounts table, stay in loop until a unique username is entered
            while (SQL_Data.UserNameExist(cust.GetCustomerUserName()))
            {
                Console.WriteLine("That Username has already been used, please try a different User Name");
                cust.SetCustomerUserName(Console.ReadLine().Trim());
            }

            //Set Password
            Console.WriteLine("Please enter a password:");
            pw = ReadPassword();
            Console.WriteLine("Please confirm your password:");
            pwConf = ReadPassword();
            while (pw != pwConf)
            {
                Console.WriteLine("Your password confirmation did not match, please try again.");
                Console.WriteLine("Please enter a password:");
                pw = ReadPassword();
                Console.WriteLine("Please confirm your password:");
                pwConf = ReadPassword();
            }
            cust.SetCustomerPassword(pw);
            
            //Commit data to table
            int r = SQL_Data.SetData(cust);

            //notify success/failure
            if (r == 0)
            {
                Console.WriteLine("There was a problem creating your account, please try again later.");
            }
            else
            {
                Console.WriteLine("Congratulations " + cust.GetCustomerFirstName() + ", your account has been successfuly created!");
                Console.WriteLine("You may now login to the Worlds Greatest Banking Ledger!");
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine("");
                Login(cust);
            }
        }

        private static void Login(Customer cust)
        {
            Console.WriteLine("Please enter your User Name:");
            cust.SetCustomerUserName(Console.ReadLine().Trim());
            Console.WriteLine("Please enter your Password:");
            cust.SetCustomerPassword(ReadPassword());

            bool userExists = SQL_Data.UserNameExist(cust.GetCustomerUserName());//check if user exists
            bool passwordValid = SQL_Data.ValidatePassword(cust.GetCustomerUserName(), cust.GetCustomerPassword());//validate password is correct
            
            if (userExists && passwordValid)
            {
                WhatNow(cust);
            }
            else
            {
                Console.WriteLine("The Credentials you entered were incorrect, please try again.");
                Login(cust);
            }
        }

        private static string ReadPassword() //mask user input when they are typing the password
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
        #endregion

        #region Workflow Methods
        private static void WhatNow(Customer cust)//main menu
        {
            Console.WriteLine("What would you like to do now?  You can do any of the following:");
            Console.WriteLine("     1. Record a deposit");
            Console.WriteLine("     2. Record a withdrawal");
            Console.WriteLine("     3. Check balance");
            Console.WriteLine("     4. See transaction history");
            Console.WriteLine("     5. Create another new account");
            Console.WriteLine("     6. Logout & Login as another User");
            Console.WriteLine("     7. Logout & Exit");

            Console.Write("Please enter a number (1-7): ");
            string choice = Console.ReadLine();
            choice = choice.Trim();
            
            switch (choice)
            {
                case "1"://deposit
                    RecordDeposit(cust);
                    break;
                case "2"://withdrawal
                    RecordWithdrawal(cust);
                    break;
                case "3"://check balance
                    CheckBalance(cust);
                    break;
                case "4"://display tran hist
                    DisplayTranHist(cust);
                    break;
                case "5"://create another account
                    CreateAccount(cust);
                    break;
                case "6"://logout and back in
                    cust.SetCustomer("", "", "", "");//clean any stored values in the Customer Object
                    Console.WriteLine("Logout is complete!");
                    Console.WriteLine("Please Login to continue... ");
                    Login(cust);
                    break;
                case "7"://exit app
                    cust.SetCustomer("", "", "", "");//clean any stored values in the Customer Object
                    Environment.Exit(0);
                    break;
                default://do nothing and ask again
                    Console.WriteLine("That was not a valid response Please try again.");
                    WhatNow(cust);
                    break;
            }
        }

        private static void RecordDeposit(Customer cust)//option 1 on main menu
        {
            decimal amount = 0;   
            Console.WriteLine("You chose to Record a Deposit, how much? ");
            string response = Console.ReadLine();
            try
            {
                amount = Convert.ToDecimal(response);
            }
            catch
            {
                Console.WriteLine("you entered an invalid value, please try again and use decimal values only.");
                RecordDeposit(cust);
            }           
            SQL_Data.SetData(cust, "deposit", amount);
            Console.WriteLine("Your deposit of $" + amount + " was recorded");
            Console.WriteLine("");
            WhatNow(cust);
        }

        private static void RecordWithdrawal(Customer cust)//option 2 on main menu
        {
            decimal amount = 0;
            Console.WriteLine("You chose to Record a Withdrawal, how much? ");
            string response = Console.ReadLine();
            try
            {
                amount = Convert.ToDecimal(response);
            }
            catch
            {
                Console.WriteLine("you entered an invalid value, please try again and use decimal values only.");
                RecordWithdrawal(cust);
            }
            SQL_Data.SetData(cust, "withdraw", amount);
            Console.WriteLine("Your withdrawal of $" + amount + " was recorded");
            Console.WriteLine("");
            WhatNow(cust);
        }
        
        private static void CheckBalance(Customer cust)//option 3 on main menu
        {
            string amount = SQL_Data.CheckBalance(cust).ToString();
            amount = amount.Substring(0, amount.Length - 2);
            Console.WriteLine("Your Current Balance is $" + amount);
            Console.WriteLine("");
            WhatNow(cust);
        }

        private static void DisplayTranHist(Customer cust)//option 4 on main menu
        {
            DataTable table = SQL_Data.TranHist(cust);
            Console.WriteLine("-------------------------------------------------------------------------");
            for (int i = 0; i< table.Rows.Count; i++)
            {
                string date = table.Rows[i][0].ToString().Substring(0, 10);
                int spaceIndex = date.IndexOf(" ");
                date = date.Substring(0, spaceIndex);

                string withdraw = table.Rows[i][1].ToString();
                withdraw = withdraw.Substring(0, withdraw.Length - 2);

                string deposit = table.Rows[i][2].ToString();
                deposit = deposit.Substring(0, deposit.Length - 2);

                Console.WriteLine("Date: " + date + " Withdraw = $" + withdraw + " Deposit = $" + deposit);
                Console.WriteLine("-------------------------------------------------------------------------");
            }
            CheckBalance(cust);
        }
        #endregion
    }
}