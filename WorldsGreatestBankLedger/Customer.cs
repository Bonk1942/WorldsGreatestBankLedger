using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldsGreatestBankLedger
{
    class Customer
    {
        string UserName;
        string FirstName;
        string LastName;
        string Password;

        //Set
        #region
        public void SetCustomer(string un, string fn, string ln, string pw)
        {
            UserName = un;
            FirstName = fn;
            LastName = ln;
            Password = pw;
        }

        public void SetCustomerUserName(string name)
        {
            UserName = name;
        }

        public void SetCustomerFirstName(string name)
        {
            FirstName = name;
        }

        public void SetCustomerLastName(string name)
        {
            LastName = name;
        }

        public void SetCustomerPassword(string pass)
        {
            Password = pass;
        }
        #endregion

        //Get
        #region
        public string GetCustomerUserName()
        {
            return UserName;
        }

        public string GetCustomerFirstName()
        {
            return FirstName;
        }

        public string GetCustomerLastName()
        {
            return LastName;
        }

        public string GetCustomerPassword()
        {
            return Password;
        }
        #endregion

    }//end class
}//end namespace
