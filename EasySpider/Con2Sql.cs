using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace EasySpider
{
    //Connect to SQLserver
    class Con2Sql
    {
        private static string con = System.Configuration.ConfigurationManager.AppSettings["DB"];    
        //connect to sqlserver
        public static SqlConnection connection = new SqlConnection(con);

        //do sql command like insert and delect
        public static bool SQL_Command(string sql)
        {
            bool flag = false;
            SqlConnection conn = Con2Sql.connection; ;
            try
            {
                SqlCommand com = new SqlCommand(sql, conn);

                if( conn.State == ConnectionState.Closed )    //if the conn is free then open it
                    conn.Open();
                com.ExecuteNonQuery();
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                if( conn.State == ConnectionState.Open )   //if the conn is open then close
                    conn.Close();
            }

            return flag;    
        }

        //事物，保证原子性
        public static bool SQL_TranCommit(string sql)
        {
            bool flag = false;
            SqlConnection conn = Con2Sql.connection;
            sql = "set XACT_ABORT on begin tran " + sql + " commit"; 
            try
            {
                SqlCommand com = new SqlCommand(sql, conn);

                if (conn.State == ConnectionState.Closed)    //if the conn is free then open it
                    conn.Open();
                com.ExecuteNonQuery();
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)   //if the conn is open then close
                    conn.Close();
            }

            return flag;
        }

        //sql bind to dataset
        public static bool DataSetBindSQl(string sql, DataSet dataset, string datasetName)
        {
            SqlDataAdapter dataapter;
            SqlConnection conn = Con2Sql.connection;
            bool flag = false;  

            try
            {
                if (conn.State == ConnectionState.Closed)    //if the conn is free then open it
                conn.Open();
                dataapter = new SqlDataAdapter(sql, conn);
                dataapter.Fill(dataset, datasetName);
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)   //if the conn is open then close
                    conn.Close();
            }

            return flag;
        }

        //sql bind to datatable
        public static bool DataTableBindSQL(string sql, DataTable dt)
        {
            SqlDataAdapter dataapter;
            SqlConnection conn = Con2Sql.connection;
            bool flag = false;

            try
            {
                if (conn.State == ConnectionState.Closed)    //if the conn is free then open it
                    conn.Open();
                dataapter = new SqlDataAdapter(sql, conn);
                dataapter.Fill(dt);
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)   //if the conn is open then close
                    conn.Close();
            }

            return flag;
        }
    }
}
