using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldsGreatestBankLedger
{
    class Program
    {
        private static string _userName = "";
        private static string _fName = "";
        private static string _lName = "";
        private static bool _authenticated = false;


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
            Console.WriteLine("");
            Console.WriteLine("-------------end of MAIN-------------");
            Console.ReadLine(); //pause app for inspection, remove before ship.
        }

        //Account creation and login
        #region 
        private static void CreateAccount(Customer cust)//needs more work
        {
            string pw = "";
            string pwConf = "";

            Console.WriteLine("Let me get some basic information from you...");

            //get name
            Console.WriteLine("What is your first name?");
            cust.SetCustomerFirstName(Console.ReadLine());
            Console.WriteLine("What is your last name?");
            cust.SetCustomerLastName(Console.ReadLine());

            //set up Username and password
            Console.WriteLine("What would you like your User Name to be?");
            cust.SetCustomerUserName(Console.ReadLine().Trim());

            //TODO: Query DB accounts table to Check if UserName Exists already


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

            //in the future, dont store plain text passwords (use some type of encryption/hashing)

            //TODO: COMMIT ACCOUNT INFO TO DATABASE

            int r = DBWork.SqlSPROC(cust);

            if (r == 0)
            {
                Console.WriteLine("There was a problem creating your account, please try again later.");
            }
            else
            {
                Console.WriteLine("Congratulations " + _fName + ", your account has been successfuly created!");
                Console.WriteLine("You may now login to the Worlds Greatest Banking Ledger!");
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine("");
                Login(cust);
            }
        }

        private static void Login(Customer cust)//needs more work
        {
            Console.WriteLine("Please enter your User Name:");
            cust.SetCustomerUserName(Console.ReadLine().Trim());
            Console.WriteLine("Please enter your Password:");
            cust.SetCustomerPassword(ReadPassword());

            bool userExists = DBWork.UserNameExist(cust.GetCustomerUserName());//check if user exists
            bool passwordValid = DBWork.ValidatePassword(cust.GetCustomerUserName(), cust.GetCustomerPassword());//validate password is correct
            
            if (userExists && passwordValid)
            {
                _authenticated = true;
                WhatNow(cust);
            }
            else
            {
                _authenticated = false;
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

        #region
        private static void WhatNow(Customer cust)
        {
            Console.WriteLine("What would you like to do now?  You can do any of the following:");
            Console.WriteLine("     1. Record a deposit");
            Console.WriteLine("     2. Record a withdrawal");
            Console.WriteLine("     3. Check balance");
            Console.WriteLine("     4. See transaction history");
            Console.WriteLine("     5. Logout");
            Console.WriteLine("     6. Create another new account");
            Console.Write("Please enter 1-6: ");
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
                case "5"://logout
                    _authenticated = false;
                    Console.WriteLine("Logout is complete!");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Login(cust);
                    break;
                case "6"://create another account
                    CreateAccount(cust);
                    break;
                default://do nothing and ask again
                    Console.WriteLine("That was not a valid response Please try again.");
                    WhatNow(cust);
                    break;
            }
        }
        private static void RecordDeposit(Customer cust)
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
            DBWork.SqlSPROC(cust, "deposit", amount);
            Console.WriteLine("Your deposit of $" + amount + " was recorded");
            Console.WriteLine("");
            WhatNow(cust);
        }

        private static void RecordWithdrawal(Customer cust)
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
            DBWork.SqlSPROC(cust, "withdraw", amount);
            Console.WriteLine("Your withdrawal of $" + amount + " was recorded");
            Console.WriteLine("");
            WhatNow(cust);
        }
        
        private static void CheckBalance(Customer cust)
        {
            string amount = DBWork.CheckBalance(cust).ToString();
            amount = amount.Substring(0, amount.Length - 2);
            Console.WriteLine("Your Current Balance is $" + amount);
            Console.WriteLine("");
            WhatNow(cust);
        }

        private static void DisplayTranHist(Customer cust)
        {
            DataTable table = DBWork.TranHist(cust);
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