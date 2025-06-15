using APITaklimSmart.Helpers;
using Npgsql;

namespace APITaklimSmart.Models
{
    public class UserSessionContext
    {
        private string __constr;
        private string __errorMsg;

        public UserSessionContext(string pObs)
        {
            __constr = pObs;
        }

        public int CreateLoginSession(int userId, string deviceInfo)
        {
            int sessionId = 0;
            string query = "INSERT INTO user_sessions (id_user, login_at, device_info, is_active) " +
                           "VALUES (@id_user, @login_at, @device_info, TRUE) RETURNING session_id;";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_user", userId);
                cmd.Parameters.AddWithValue("@login_at", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@device_info", deviceInfo ?? "");

                sessionId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal membuat sesi login: " + __errorMsg);
            }

            return sessionId;
        }

        public bool LogoutSession(int sessionId)
        {
            bool success = false;
            string query = "UPDATE user_sessions SET logout_at = @logout_at, is_active = FALSE WHERE id_session = @id_session;";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@logout_at", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@id_session", sessionId);

                int affected = cmd.ExecuteNonQuery();
                success = affected > 0;

                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal logout sesi: " + __errorMsg);
            }

            return success;
        }
    }
}
