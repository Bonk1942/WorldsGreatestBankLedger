using System;
using System.Data;
using System.Data.SqlClient;

namespace WorldsGreatestBankLedger
{
    class SQL_Data
    {
        private static SqlConnection con = new SqlConnection();

        #region Private Methods
        private static void DBConnect()//using this so I can easily change Connection String depending on where I am working on this.
        {
            //local desktop DB Connection String
            //con.ConnectionString = (@"Data Source = localhost; Initial Catalog = WorldsGreatestBankingLedger; Integrated Security = True");

            //local laptop DB Connection String
            con.ConnectionString = (@"Data Source=BELDZB15U31619\MSSQLSERVER2017;Initial Catalog=WorldsGreatestBankingLedger;Integrated Security=True");
            
        }

        private static int ExecuteSPROC(SqlCommand cmd)//This is reusable for SPROCS
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
        }

        private static DataTable ExecuteSQLText(SqlCommand cmd, ref DataTable table)//This is reusable for Views
        {
            con.Open();
            table.Load(cmd.ExecuteReader());
            con.Close();
            return table;
        }

        #endregion

        #region Public Methods
        public static int SetData(Customer cust)//create account (overloaded)
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


            int r = ExecuteSPROC(cmd);
            int r2 = SetData(cust, "deposit", 0);
            return r;
        }

        public static int SetData(Customer cust, string tranType, decimal amount)//deposit & withdraw (overloaded)
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
                    return 0;
            }

            int r = ExecuteSPROC(cmd);
            return r;
        }
        
        public static decimal CheckBalance(Customer cust)//returns a single value as a decimal
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable balance = new DataTable();
            cmd.CommandText = "SELECT sum([Credit]) - sum([Debit]) as bal FROM [vw_TranHist] WHERE  [UserName] LIKE '" + cust.GetCustomerUserName() + "'";
            ExecuteSQLText(cmd, ref balance);
            return Convert.ToDecimal(balance.Rows[0][0].ToString());
        }

        public static DataTable TranHist(Customer cust)//returns a DataTable with as many rows as there are returned from Query
        {
            DBConnect();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            // Table to store the query results
            DataTable tranHist = new DataTable();
            cmd.CommandText = "SELECT TranDate, Debit, Credit FROM [vw_TranHist] WHERE  [UserName] LIKE '" + cust.GetCustomerUserName() + "'";
            ExecuteSQLText(cmd, ref tranHist);
            return tranHist;
        }

        public static bool UserNameExist(string userName)//returns true if user exists in accounts table, false if not.
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

        public static bool ValidatePassword(string userName, string password)//returns true if username and password are a match with whats in SQL table
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
        #endregion
    }

}
