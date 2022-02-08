﻿using DatabaseProgrammingWithADODemos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Data.SqlClient;
using System.Configuration;
//using System.Data.OleDb;
using System.Data;
using System.Data.Common;

namespace DatabaseProgrammingWithADODemos.DataAccess
{
    public class ContactsRepository : IContactsRepository
    {
        public bool DeleteContact(int contactId)
        {
            IDbConnection conn = GetConnection();
            string sqlDelete = "delete contacts where contactid = @cid";
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlDelete;
            cmd.Connection = conn;

            IDbDataParameter p1 = cmd.CreateParameter();
            p1.ParameterName = "@cid";
            p1.Value = contactId;
            cmd.Parameters.Add(p1);

            int c = 0;
            using (conn)
            {
                conn.Open();
                c = cmd.ExecuteNonQuery();
                //conn.Close();
            }
            return c > 0;
        }

        public Contact GetContactById(int contactId)
        {
            
            SqlConnection conn = GetConnection();
            string selectSql = $"select * from contacts where contactid = {contactId}";
            SqlCommand cmd = new SqlCommand(selectSql, conn);
            Contact c = new Contact();
            using (conn)
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                
                c.ContactID = (int)reader[0];
                c.Name = reader[1].ToString();
                c.Mobile = reader.GetString(2);
                c.Email = reader["email"].ToString();
                c.Location = reader.GetString(4);
                reader.Close();
            }
            return c;
        }

        public List<Contact> GetContacts()
        {
            SqlConnection conn = GetConnection();
            string selectSql = $"select * from contacts";
            SqlCommand cmd = new SqlCommand(selectSql, conn);
           
            List<Contact> contacts = new List<Contact>();
            using (conn)
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Contact c = new Contact();
                    c.ContactID = (int)reader[0];
                    c.Name = reader[1].ToString();
                    c.Mobile = reader.GetString(2);
                    c.Email = reader["email"].ToString();
                    c.Location = reader.GetString(4);
                    contacts.Add(c);
                }
                reader.Close();
            }
            return contacts;
        }

        public List<Contact> GetContactsByLocation(string location)
        {
            SqlConnection conn = GetConnection();

            //location = "bangalore';delete contacts";

            // string selectSql = $"select * from contacts where location = '{location}'";
            string selectSql = "select * from contacts where location = @loc";
            //string selectSql = $"select * from contacts where location = 'bangalore';delete contacts'"; // SQL Injection attack
            SqlCommand cmd = new SqlCommand(selectSql, conn);
            //cmd.Parameters.AddWithValue("@loc", location);

            SqlParameter p1 = new SqlParameter();
            p1.ParameterName = "@loc";
            p1.Value = location;
            cmd.Parameters.Add(p1);

            List<Contact> contacts = new List<Contact>();
            using (conn)
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Contact c = new Contact();
                    c.ContactID = (int)reader[0];
                    c.Name = reader[1].ToString();
                    c.Mobile = reader.GetString(2);
                    c.Email = reader["email"].ToString();
                    c.Location = reader.GetString(4);
                    contacts.Add(c);
                }
                reader.Close();
            }
            return contacts;
        }

        public bool SaveContact(Contact contact)
        {
            SqlConnection sqlConnection = GetConnection();
            string sqlInsert = $"insert into contacts values('{contact.Name}','{contact.Mobile}','{contact.Email}','{contact.Location}')";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlInsert;
            cmd.Connection = sqlConnection;
            int count = 0;
            try
            {
                sqlConnection.Open();
                count = cmd.ExecuteNonQuery();
            }
            finally
            {
                sqlConnection.Close();
            }
            return count > 0;
        }
        public bool UpdateContact(Contact contact)
        {
            SqlConnection conn = GetConnection();
            string updateSql = $"update contacts set name = {contact.Name}, mobile = {contact.Mobile}, email={contact.Email}, location ={contact.Location} where contactid={contact.ContactID}";
            SqlCommand cmd = new SqlCommand(updateSql, conn);
            int count = 0;
            using (conn)
            {
                conn.Open();
                count = cmd.ExecuteNonQuery();
            }
            return count > 0;
        }
        private static IDbConnection GetConnection()
        {
            string provider = ConfigurationManager.ConnectionStrings["default"].ProviderName;
            DbProviderFactory factory =  DbProviderFactories.GetFactory(provider);
            string connStr = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            IDbConnection conn =  factory.CreateConnection();
            conn.ConnectionString = connStr;
            return conn;
        }
    }
}
