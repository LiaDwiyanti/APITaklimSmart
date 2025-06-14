using Npgsql;
using System.Data;

namespace APITaklimSmart.Helpers
{
    public class DBHelper
    {
        private NpgsqlConnection connection;
        private NpgsqlTransaction transaction;
        private string __constr;
        public DBHelper(string pConstr)
        {
            __constr = pConstr;
            connection = new NpgsqlConnection(pConstr);
            connection.ConnectionString = __constr;
        }
        public void BeginTransaction()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            transaction = connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            transaction?.Commit();
            CloseConnection();
        }

        public void RollbackTransaction()
        {
            transaction?.Rollback();
            CloseConnection();
        }
        public NpgsqlCommand GetNpgsqlCommand(string query, bool useTransaction = false)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

            if (useTransaction && transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandType = CommandType.Text;
            return cmd;
        }
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
