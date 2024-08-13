using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Principal;

namespace BankApplication.DataAccess
{
    public class BankDbRepository : IAccountRepository
    {
        public void Create(IAccount account)
        {

            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;

            
            string sqlInsert = $"insert into Account values (@accID,@name,@pin,@active,@dtOfOpening,@balance,@privilegeType,@accType)";



            SqlCommand cmd = conn.CreateCommand();
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@name";
            p1.Value = account.Name;
            cmd.Parameters.Add(p1);

            cmd.Parameters.AddWithValue("@accID", account.AccNo);
          
            cmd.Parameters.AddWithValue("@pin", account.Pin);
            cmd.Parameters.AddWithValue("@active", account.IsActive);
            cmd.Parameters.AddWithValue("@dtOfOpening", account.DateOfOpening);
            cmd.Parameters.AddWithValue("@balance", account.Balance);
            cmd.Parameters.AddWithValue("@privilegeType", account.PrivilegeType.ToString());
            cmd.Parameters.AddWithValue("@accType", account.GetAccType());






            cmd.CommandText = sqlInsert;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Account inserted");
                // logout - close the connection
            }
            finally
            {
                conn.Close();
            }
        }

        public List<IAccount> GetAll()
        {
            //Get all the accounts
            //Create a List<IAccount> 
            List<IAccount> accountList=new List<IAccount>();

            SqlConnection conn = new SqlConnection();
            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            conn.ConnectionString = connStr;
            string sqlRead = "select * from Account";
            SqlCommand cmd=conn.CreateCommand();
            cmd.CommandText = sqlRead; //Read the connection
            cmd.Connection = conn;     //Read the connection
            conn.Open();
            SqlDataReader reader= cmd.ExecuteReader();
            while (reader.Read()) 
            {
                IAccount account;
                string accNo = reader["accId"].ToString();
                int pin =int.Parse( reader["pin"].ToString());
                string name=reader["name"].ToString();
                bool isActive = Convert.ToBoolean(reader["active"]);
                float balance = float.Parse(reader["balance"].ToString());
                PrivilegeType privilegeType = Enum.Parse<PrivilegeType>(reader["privilegeType"].ToString());

                if (reader["AccType"].ToString() == "SAVINGS")
                {
                    account = new Savings();
                }
                else 
                {
                    account = new Current();
                     
                }

                account.AccNo = accNo;
                account.Pin=pin;
                account.Name = name;
                account.Balance= balance;
                account.IsActive= isActive;
                account.PrivilegeType= privilegeType;



                accountList.Add(account);
            }

            conn.Close();
            return accountList;




        }

        

        public void Update(string accountNumber, double balance)
        {
            SqlConnection conn = new SqlConnection();
            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            conn.ConnectionString = connStr;

            string sqlUpdate = $"update Account SET  balance=@balance   where accId=@accId";

            SqlCommand cmd = conn.CreateCommand();
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@balance";
            p1.Value = balance;
            cmd.Parameters.Add(p1);

            cmd.Parameters.AddWithValue("@accID",accountNumber);


            cmd.CommandText = sqlUpdate;
            cmd.Connection = conn;
            try 
            { 
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0) { Console.WriteLine("Update successful"); }
                else { Console.WriteLine("Update unsuccessful"); }

            }
            finally { conn.Close(); }

        }

        public IAccount GetAccountById(string accountNo)
        {
            IAccount account = null;

            SqlConnection conn = new SqlConnection();

            string conStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = conStr;

            string sqlSelect = $"select * from Account where accId=@accId";

            SqlCommand cmd = new SqlCommand();

            cmd.Parameters.AddWithValue("@accId", accountNo);
            cmd.CommandText = sqlSelect;
            cmd.Connection = conn;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["accType"].ToString() == "SAVINGS")
                        account = new Savings();
                    else
                        account = new Current();



                    account.AccNo = reader["accId"].ToString();
                    account.Name = reader["name"].ToString();
                    account.Pin =int.Parse( reader["pin"].ToString());
                    account.IsActive = Convert.ToBoolean(reader["active"]);
                    account.DateOfOpening = Convert.ToDateTime(reader["dtOfOpening"]);
                    account.Balance = (double)reader["balance"];
                    account.PrivilegeType = Enum.Parse<PrivilegeType>(reader["privilegeType"].ToString());
                }
                if (account != null)
                {
                    //creating policy for the account using policyfactory instance
                    PolicyFactory policyFactory = PolicyFactory.Instance;
                    IPolicy policy = policyFactory.CreatePolicy(account.GetAccType(), account.PrivilegeType.ToString());

                    //assigning policy to account
                    account.Policy = policy;
                }
                else
                {
                    throw new AccountDoesNotExistException($"Account does not exist with account no:{accountNo}");
                }
                return account;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }
        }

        public long GetAccountCount()
        {
            long accountCount = 0;

            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;


            string sqlCount = $"SELECT COUNT(*) FROM Account";
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlCount;
            cmd.Connection = conn;

            conn.Open();
            SqlDataReader reader=cmd.ExecuteReader();
            while (reader.Read())
            {
                accountCount = Convert.ToInt64(reader[0]);
            }
            conn.Close();
           

            return accountCount;
            
        }

        public  Dictionary<string, long> GetAccountCountByType()
        {

            Dictionary<string, long> accountCountType = new Dictionary<string, long>(); 
            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;

            string sqlCountByType = $"SELECT accType, COUNT(*) AS Count from Account Group By accType";
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlCountByType;
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string accType = reader["accType"].ToString();
                long count = Convert.ToInt64(reader["Count"]);
                accountCountType[accType] = count;
                
            }
            conn.Close();
            return accountCountType;
        }

        public double GetTotalBankWorth()
        {
            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;

            string sqlTotalWorth = $"SELECT SUM(balance) from account";

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlTotalWorth;
            cmd.Connection = conn;
            conn.Open();

            double totalWorth = 0.0;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                totalWorth = Convert.ToDouble(reader[0]);
            }
            conn.Close();
            return totalWorth;

        }

        public void BankTableDataUpdate(string accNo, string bankCode, double amount)

        {
            switch (bankCode)
            {
                case "ICICI":
                    ICICIBankUpdate(accNo, amount);
                    break;
                case "CITI":
                    CITIBankUpdate(accNo, amount);
                    break;

                default:
                    Console.WriteLine("Bank data failed");
                    break;
            }

            
        }

        public void ICICIBankUpdate(string accNo, double amount)
        {
            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;

            string bankInsert = $"insert into ICICIBANK value(@AccId,@amt)";

            SqlCommand cmd = conn.CreateCommand();
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@AccId";
            p1.Value = accNo;
            cmd.Parameters.Add(p1);

            cmd.Parameters.AddWithValue("@amt", amount);
            cmd.CommandText = bankInsert;
            cmd.Connection = conn;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("External transaction table of ICICI updated");
            }
            finally { conn.Close(); }


            
        }

        public void CITIBankUpdate(string accNo, double amount)
        {
            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;

            string bankInsert = $"insert into CITIBANK value(@AccId,@amt)";

            SqlCommand cmd = conn.CreateCommand();
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@AccId";
            p1.Value = accNo;
            cmd.Parameters.Add(p1);

            cmd.Parameters.AddWithValue("@amt", amount);
            cmd.CommandText = bankInsert;
            cmd.Connection = conn;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("External transaction table of CITI updated");
            }
            finally { conn.Close(); }



        }
    }
}
