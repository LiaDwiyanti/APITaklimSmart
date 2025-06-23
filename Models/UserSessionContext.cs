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
                Console.WriteLine("Gagal membuat sesi login: " + ex.Message);
            }

            return sessionId;
        }

        public UserSession GetSessionById(int userId)
        {
            UserSession session = null;
            string query = "Select * from user_sessions where id_user = @id_user AND is_active = TRUE LIMIT 1";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_user", userId);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    session = new UserSession()
                    {
                        Id_Session = int.Parse(reader["session_id"].ToString()),
                        Id_User = int.Parse(reader["id_user"].ToString()),
                        LoginAt = Convert.ToDateTime(reader["login_at"]),
                        LogoutAt = reader["logout_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["logout_at"]),
                        DeviceInfo = reader["device_info"].ToString(),
                        IsActive = bool.Parse(reader["is_active"].ToString())
                    };
                }
                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal menemukan sesi login : " + __errorMsg);
            }
            return session;
        }

        public bool LogoutSession(int sessionId)
        {
            bool success = false;
            string query = "UPDATE user_sessions SET logout_at = @logout_at, is_active = FALSE WHERE session_id = @session_id;";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@logout_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@session_id", sessionId);

                int affected = cmd.ExecuteNonQuery();
                success = affected > 0;

                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gagal logout sesi: " + ex.Message);
            }

            return success;
        }
    }
}
