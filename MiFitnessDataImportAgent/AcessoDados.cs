using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MiFitnessDataImportAgent
{
    public class AcessoDados : IDisposable
    {
        private string sqlConn = ConfigurationManager.ConnectionStrings["ConexaoPadrao"].ToString();
        private SqlConnection conn;

        public AcessoDados()
        {
            conn = new SqlConnection(sqlConn);
            conn.Open();
        }

        public void executaComando(string comandoSql)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = comandoSql;
            cmd.CommandType = CommandType.Text;

            cmd.ExecuteNonQuery();
        }

        public DataSet listar(string sql)
        {
            DataSet ds = new DataSet();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            return ds;
        }

        public DataSet listar()
        {
            DataSet ds = new DataSet();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from usuario";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            return ds;
        }

        public void Dispose()
        {
            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
        }
    }
}