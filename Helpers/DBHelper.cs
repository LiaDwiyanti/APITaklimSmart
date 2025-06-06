using Npgsql;
using System.Data;

namespace APITaklimSmart.Helpers
{
    public class DBHelper
    {
        private NpgsqlConnection connection;
        private string __constr;
        public DBHelper(string pConstr)
        {
            __constr = pConstr;
            connection = new NpgsqlConnection(pConstr);
            connection.ConnectionString = __constr;
        }
        public NpgsqlCommand GetNpgsqlCommand(string query)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }
        public void closeConnection()
        {
            connection.Close();
        }
    }
}
