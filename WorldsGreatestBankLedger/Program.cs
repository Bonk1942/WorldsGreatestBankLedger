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
 

        private static SqlConnection con = new SqlConnection();
        private static SqlCommand cmd = new SqlCommand();


        private static string _userName = "";
        private static string _fName = "";
        private static string _lName = "";
        private static bool _authenticated = false;


        static void Main(string[] args)
        {
            Customer cust = new Customer(); //create new customer obj

            //local desktop DB Connection String
            //con.ConnectionString = (@"Data Source = localhost; Initial Catalog = WorldsGreatestBankingLedger; Integrated Security = True");

            //local laptop DB Connection String
            con.ConnectionString = (@"Data Source=BELDZB15U31619\MSSQLSERVER2017;Initial Catalog=WorldsGreatestBankingLedger;Integrated Security=True");

            cmd.Connection = con;

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

            int r = sqlSPROC(cust);

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

            // TODO: Query DB Account table to Check if Username exists
            //       If User does not Exist, tell them they the credentials they provided were incorrect, please try again Login(); Set _authenticated = false; Login(cust);
            //       Else If User does Exist validate Password against whats in DB for User
            //            If password entered doesnt match password in db tell them they the credentials they provided were incorrect, please try again Set _authenticated = false; Login(cust);
            //            Else if Password Matches, allow them to do more..... What would you like to do? method Set _authenticated = true;

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

        //SQL
        #region
        private static int sqlSPROC(Customer cust)//create account
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_CreateAccount";

            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = cust.GetCustomerUserName();
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = cust.GetCustomerFirstName();
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = cust.GetCustomerLastName();
            cmd.Parameters.Add("@PW", SqlDbType.NVarChar, 255).Value = cust.GetCustomerPassword();

            int r;
            try
            {
                con.Open();
                r = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return r;
        }

        private static int sqlSPROC(Customer cust, string tranType, decimal amount)//deposit & withdraw 
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = cust.GetCustomerUserName();
            switch (tranType.Trim().ToLower())
            {
                case "deposit":
                    cmd.CommandText = "sp_Deposit";
                    cmd.Parameters.Add("@Credit", SqlDbType.Money).Value = amount;
                    break;
                case "withdraw":
                    cmd.CommandText = "sp_Withdraw";
                    cmd.Parameters.Add("@Debit", SqlDbType.Money).Value = amount;
                    break;
                default:
                    return 0;//get out of method and return 0... this shouldnt ever happen.
            }

            int r;
            try
            {
                con.Open();
                r = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return r;
        }
        #endregion 
    }
}
