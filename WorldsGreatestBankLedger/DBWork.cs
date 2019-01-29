using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorldsGreatestBankLedger
{
    class DBWork
    {
        public static SqlConnection con = new SqlConnection();
        

        private static void DBConnect()//set up SQL Connection
        {
            //local desktop DB Connection String
            con.ConnectionString = (@"Data Source = localhost; Initial Catalog = WorldsGreatestBankingLedger; Integrated Security = True");

            //local laptop DB Connection String
            //con.ConnectionString = (@"Data Source=BELDZB15U31619\MSSQLSERVER2017;Initial Catalog=WorldsGreatestBankingLedger;Integrated Security=True");
            
        }

        private static int ExecuteSQL(SqlCommand cmd)
        {
            int r = 0;
            try
            {
                con.Open();
                r = cmd.ExecuteNonQuery();
                
            }
            finally
            {
                con.Close();
                cmd.Parameters.Clear();
            }
            return r;
        }//execute sql commands

        public static int SqlSPROC(Customer cust)//create account
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_CreateAccount";

            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = cust.GetCustomerUserName();
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = cust.GetCustomerFirstName();
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = cust.GetCustomerLastName();
            cmd.Parameters.Add("@PW", SqlDbType.NVarChar, 255).Value = cust.GetCustomerPassword();


            int r = ExecuteSQL(cmd);
            return r;
        }

        public static int SqlSPROC(Customer cust, string tranType, decimal amount)//deposit & withdraw 
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
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

            int r = ExecuteSQL(cmd);
            return r;
        }
        
        public static decimal CheckBalance(Customer cust)
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable table = new DataTable();
            cmd.CommandText = "SELECT sum([Credit]) - sum([Debit]) as bal FROM [vw_TranHist] WHERE  [UserName] LIKE '" + cust.GetCustomerUserName() + "'";
            con.Open();
            table.Load(cmd.ExecuteReader());
            con.Close();
            return Convert.ToDecimal(table.Rows[0][0].ToString());
        }

        public static DataTable TranHist(Customer cust)
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable table = new DataTable();
            cmd.CommandText = "SELECT TranDate, Debit, Credit FROM [vw_TranHist] WHERE  [UserName] LIKE '" + cust.GetCustomerUserName() + "'";
            con.Open();
            table.Load(cmd.ExecuteReader());
            con.Close();
            return table;
        }

        public static bool UserNameExist(string userName)
        {
            DBConnect(); SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable table = new DataTable();
            cmd.CommandText = "SELECT [UserName] FROM [vw_Login] WHERE  [UserName] LIKE '" + userName + "'";
            con.Open();
            table.Load(cmd.ExecuteReader());
            con.Close();

            if (table.Rows.Count >= 1)
            {
                return true;
            }
            return false;
        }

        public static bool ValidatePassword(string userName, string password)
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable table = new DataTable();
            cmd.CommandText = "SELECT [PW] FROM [vw_Login] WHERE  [UserName] LIKE '" + userName + "' AND [PW] LIKE '" + password + "'";
            con.Open();
            table.Load(cmd.ExecuteReader());
            con.Close();

            if (table.Rows.Count >= 1)
            {
                return true;
            }
            return false;
        }
    }
    
}
