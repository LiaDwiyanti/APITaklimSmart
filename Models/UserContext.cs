using APITaklimSmart.Helpers;
using Npgsql;
using static APITaklimSmart.Models.Enums;

namespace APITaklimSmart.Models
{
    public class UserContext
    {
        private string __constr;
        private string __errorMsg;

        public UserContext(string pObs)
        {
            __constr = pObs;
        }

        public List<User> ReadUser()
        {
            List<User> listUser = new List<User>();
            string query = "Read * from users";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listUser.Add(new User()
                    {
                        Id_User = int.Parse(reader["id_user"].ToString()),
                        Username = reader["username"].ToString(),
                        Email = reader["email"].ToString(),
                        No_hp = reader["nohp"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                    });
                }
            }
            catch(Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data user :" + __errorMsg);
            }
            return listUser;
        }
        public bool RegistUser(User user)
        {
            bool result = false;
            string query = "INSERT INTO users (username, email, nohp, alamat, password, user_role, is_active, created_at, updated_at) " +
                           "VALUES (@username, @email, @no_hp, @alamat, @password, @user_role, @is_active, @created_at, @updated_at) RETURNING id_user;";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@no_hp", user.No_hp);
                cmd.Parameters.AddWithValue("@alamat", user.Alamat);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@user_role", user.User_Role.ToString());
                cmd.Parameters.AddWithValue("@is_active", user.IsActive);
                cmd.Parameters.AddWithValue("@created_at", user.CreatedAt);
                cmd.Parameters.AddWithValue("@updated_at", user.UpdatedAt);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                result = rowsAffected > 0;

                cmd.Dispose();
                db.closeConnection();
            }
            catch(Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan di registrasi user :" + __errorMsg);
            }
            return result;
        }
        public User getUserByNoHP(string no_hp)
        {
            User user = null;
            string query = "Select * from users where no_hp = @no_hp";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@no_hp", no_hp);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    user = new User()
                    {
                        Id_User = int.Parse(reader["id_user"].ToString()),
                        Username = reader["username"].ToString(),
                        Email = reader["email"].ToString(),
                        No_hp = reader["nohp"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                    };
                }
                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }
            return user;
        }
    }
}
