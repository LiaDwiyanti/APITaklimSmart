using APITaklimSmart.Helpers;
using Npgsql;

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
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data user :" + __errorMsg);
            }
            return listUser;
        }
        public bool RegistUser(User user, Lokasi lokasi)
        {
            bool result = false;
            string queryUser = "INSERT INTO users (username, email, nohp, alamat, password, user_role, is_active, created_at, updated_at) " +
                                "VALUES (@username, @email, @nohp, @alamat, @password, @user_role::user_role, @is_active, @created_at, @updated_at) ";

            string queryLokasi = "INSERT INTO lokasi (nama_lokasi, alamat, latitude, longitude, deskripsi_lokasi, created_at) " +
                                 "VALUES (@nama_lokasi, @alamat_lokasi, @latitude, @longitude, @deskripsi_lokasi, @created_at_lokasi);";

            DBHelper db = new DBHelper(this.__constr);
            try
            {
                db.BeginTransaction();

                // Insert user
                NpgsqlCommand cmdUser = db.GetNpgsqlCommand(queryUser, true);
                cmdUser.Parameters.AddWithValue("@username", user.Username);
                cmdUser.Parameters.AddWithValue("@email", user.Email);
                cmdUser.Parameters.AddWithValue("@nohp", user.No_hp);
                cmdUser.Parameters.AddWithValue("@alamat", user.Alamat);
                cmdUser.Parameters.AddWithValue("@password", user.Password);
                cmdUser.Parameters.AddWithValue("@user_role", user.User_Role.ToString());
                cmdUser.Parameters.AddWithValue("@is_active", user.IsActive);
                cmdUser.Parameters.AddWithValue("@created_at", user.CreatedAt);
                cmdUser.Parameters.AddWithValue("@updated_at", user.UpdatedAt);
                cmdUser.ExecuteNonQuery();
                cmdUser.Dispose();

                // Insert lokasi
                NpgsqlCommand cmdLokasi = db.GetNpgsqlCommand(queryLokasi, true);
                cmdLokasi.Parameters.AddWithValue("@nama_lokasi", lokasi.Nama_Lokasi);
                cmdLokasi.Parameters.AddWithValue("@alamat_lokasi", lokasi.Alamat);
                cmdLokasi.Parameters.AddWithValue("@latitude", lokasi.Latitude);
                cmdLokasi.Parameters.AddWithValue("@longitude", lokasi.Longitude);
                cmdLokasi.Parameters.AddWithValue("@deskripsi_lokasi", lokasi.Deskripsi_Lokasi);
                cmdLokasi.Parameters.AddWithValue("@created_at_lokasi", lokasi.CreatedAt);
                cmdLokasi.ExecuteNonQuery();
                cmdLokasi.Dispose();

                db.CommitTransaction();
                result = true;
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal simpan user dan lokasi: " + __errorMsg);
                db.RollbackTransaction();
            }
            return result;
        }

        public User getUserByNoHP(string nohp)
        {
            User user = null;
            string query = "Select * from users where nohp = @nohp";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nohp", nohp);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
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
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }
            return user;
        }

        public User getUserById(int id)
        {
            User user = null;
            string query = "Select * from users where id_user = @id_user";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_user", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
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
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }
            return user;
        }

        public User getUserByUsername(string username)
        {
            User user = null;
            string query = "Select * from users where username = @username";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@username", username);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new User()
                    {
                        Id_User = int.Parse(reader["id_user"].ToString()),
                        Username = reader["username"].ToString(),
                        Email = reader["email"].ToString(),
                        No_hp = reader["nohp"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                        Password = reader["password"].ToString()
                    };
                }
                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }
            return user;
        }
    }
}
