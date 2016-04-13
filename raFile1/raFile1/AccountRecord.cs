using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raFile1
{
    //class CONTAINS data from one record
    //represents schema for the data 
    class AccountRecord
    {
        private int _account;
        private string _fName;
        private string _lName;
        private double _balance;

        public AccountRecord()
        {
            Account = 0;
            FirstName = "";
            LastName = "";
            Balance = 0.0;
        }
        public AccountRecord(int acct, string fn, string ln, double bal)
        {
            Account = acct;
            FirstName = fn;
            LastName = ln;
            Balance = bal;
        }
        //create public properties
        public int Account
        {
            get 
            {
                return _account;
            }
            set 
            {
                _account = value;
            }

        }
        public string FirstName
        {
            get
            {
                return _fName;
            }
            set
            {
                _fName = value;
            }
        }
        public string LastName
        {
            get
            {
                return _lName;
            }
            set
            {
                _lName = value;
            }
        }
        public double Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
            }
        }

    }
}
