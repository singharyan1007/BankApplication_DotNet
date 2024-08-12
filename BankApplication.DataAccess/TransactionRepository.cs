﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BankApplication.CommonLayer;

namespace BankApplication.DataAccess
{
    public class TransactionRepository : ITransactionRepository
    {
        public void Create(Transaction transaction)
        {
            Console.WriteLine($"The transaction type is {transaction.TransactionType.ToString().ToUpper()}");
            Console.WriteLine($"The transaction type is {transaction.TransactionStatus.ToString().ToUpper()}");

            SqlConnection conn = new SqlConnection();

            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            conn.ConnectionString = connStr;


            string sqlInsert = $"INSERT INTO [Trans] (TransId, transactionType, accNo, transDate, amount, transactionStatus) " +
                           "VALUES (@TransId, @transactionType, @accNo, @transDate, @amount, @transactionStatus)";




            SqlCommand cmd = conn.CreateCommand();
            SqlParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@TransId";
            p1.Value = transaction.TransID;
            cmd.Parameters.Add(p1);

            cmd.Parameters.AddWithValue("@accNo", transaction.FromAccount.AccNo);
            cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType.ToString().ToUpper());
            
            cmd.Parameters.AddWithValue("@TransDate",transaction.TranDate);
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@TransactionStatus",transaction.TransactionStatus.ToString().ToUpper());

            cmd.CommandText = sqlInsert;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Transaction inserted");
                // logout - close the connection
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Transaction> GetAll()
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransactionById(int transId)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
