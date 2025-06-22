using APITaklimSmart.Helpers;
using Npgsql;

namespace APITaklimSmart.Models
{
    public class PenjadwalanContext
    {
        private string __constr;
        public string __errorMsg;

        public PenjadwalanContext(string pObs)
        {
            __constr = pObs;
        }
        public List<Penjadwalan> ReadPenjadwalan()
        {
            List<Penjadwalan> listPenjadwalan = new List<Penjadwalan>();
            string query = "select * from penjadwalan";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Enum.TryParse<StatusPenjadwalan>(
                        reader.GetString(reader.GetOrdinal("status_penjadwalan")),
                        true,
                        out StatusPenjadwalan status
                    );

                    listPenjadwalan.Add(new Penjadwalan()
                    {
                        Id_Penjadwalan = int.Parse(reader["id_penjadwalan"].ToString()),
                        Nama_Penjadwalan = reader["nama_penjadwalan"].ToString(),
                        Tanggal_Penjadwalan = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("tanggal_penjadwalan"))),
                        Waktu_Penjadwalan = reader.GetTimeSpan(reader.GetOrdinal("waktu_penjadwalan")),
                        Id_Lokasi = int.Parse(reader["id_lokasi"].ToString()),
                        Deskripsi_Penjadwalan = reader["deskripsi_penjadwalan"].ToString(),
                        Status_Penjadwalan = status
                    });
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data penjadwalan :" + __errorMsg);
            }
            return listPenjadwalan;
        }

        public Penjadwalan ReadPenjadwalanById(int idPenjadwalan)
        {
            Penjadwalan jadwal = null;
            string query = "select * from penjadwalan where id_penjadwalan=@id_penjadwalan";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_penjadwalan", idPenjadwalan);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Enum.TryParse<StatusPenjadwalan>(
                        reader.GetString(reader.GetOrdinal("status_penjadwalan")),
                        true,
                        out StatusPenjadwalan status
                    );

                    jadwal = new Penjadwalan()
                    {
                        Id_Penjadwalan = int.Parse(reader["id_penjadwalan"].ToString()),
                        Nama_Penjadwalan = reader["nama_penjadwalan"].ToString(),
                        Tanggal_Penjadwalan = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("tanggal_penjadwalan"))),
                        Waktu_Penjadwalan = reader.GetTimeSpan(reader.GetOrdinal("waktu_penjadwalan")),
                        Id_Lokasi = int.Parse(reader["id_lokasi"].ToString()),
                        Deskripsi_Penjadwalan = reader["deskripsi_penjadwalan"].ToString(),
                        Status_Penjadwalan = status
                    };
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data penjadwalan :" + __errorMsg);
            }
            return jadwal;
        }

        public bool CreatePenjadwalan(Penjadwalan jadwal)
        {
            bool result = false;
            string queryJadwal = "INSERT INTO penjadwalan (nama_penjadwalan, tanggal_penjadwalan, waktu_penjadwalan, id_lokasi, deskripsi_penjadwalan, status_penjadwalan, created_by, created_at) " +
                                "VALUES ( @nama_penjadwalan, @tanggal_penjadwalan, @waktu_penjadwalan, @id_lokasi, @deskripsi_penjadwalan, @status_penjadwalan::status_penjadwalan, @created_by, @created_at) RETURNING id_penjadwalan";

            string queryRiwayat = "INSERT INTO riwayat (id_penjadwalan, status_lama, status_baru, changed_by, alasan, changed_at) " +
                                 "VALUES (@id_penjadwalan, @status_lama::status_penjadwalan, @status_baru::status_penjadwalan, @changed_by, @alasan, @changed_at);";

            DBHelper db = new DBHelper(this.__constr);
            try
            {
                db.BeginTransaction();

                // Insert jadwal
                NpgsqlCommand cmdJadwal = db.GetNpgsqlCommand(queryJadwal, true);
                cmdJadwal.Parameters.AddWithValue("@nama_penjadwalan", jadwal.Nama_Penjadwalan);
                cmdJadwal.Parameters.AddWithValue("@tanggal_penjadwalan", jadwal.Tanggal_Penjadwalan);
                cmdJadwal.Parameters.AddWithValue("@waktu_penjadwalan", jadwal.Waktu_Penjadwalan);
                cmdJadwal.Parameters.AddWithValue("@id_lokasi", jadwal.Id_Lokasi);
                cmdJadwal.Parameters.AddWithValue("@deskripsi_penjadwalan", jadwal.Deskripsi_Penjadwalan ?? (object)DBNull.Value);
                cmdJadwal.Parameters.AddWithValue("@status_penjadwalan", jadwal.Status_Penjadwalan.ToString().ToLower());
                cmdJadwal.Parameters.AddWithValue("@created_by", jadwal.Created_By);
                cmdJadwal.Parameters.AddWithValue("@created_at", jadwal.Created_At);
                int idBaru = Convert.ToInt32(cmdJadwal.ExecuteScalar());
                cmdJadwal.Dispose();

                // Insert riwayat
                NpgsqlCommand cmdRiwayat = db.GetNpgsqlCommand(queryRiwayat, true);
                cmdRiwayat.Parameters.AddWithValue("@id_penjadwalan", idBaru);
                cmdRiwayat.Parameters.AddWithValue("@status_lama", DBNull.Value);
                cmdRiwayat.Parameters.AddWithValue("@status_baru", jadwal.Status_Penjadwalan.ToString().ToLower());
                cmdRiwayat.Parameters.AddWithValue("@changed_by", jadwal.Created_By);
                cmdRiwayat.Parameters.AddWithValue("@alasan", DBNull.Value);
                cmdRiwayat.Parameters.AddWithValue("@changed_at", DateTime.UtcNow);

                cmdRiwayat.ExecuteNonQuery();
                cmdRiwayat.Dispose();

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

        public bool UpdatePenjadwalan(Penjadwalan jadwal)
        {
            bool result = false;

            string query = @"UPDATE penjadwalan SET
            nama_penjadwalan = @nama_penjadwalan,
            tanggal_penjadwalan = @tanggal_penjadwalan,
            waktu_penjadwalan = @waktu_penjadwalan,
            id_lokasi = @id_lokasi,
            deskripsi_penjadwalan = @deskripsi_penjadwalan,
            status_penjadwalan = @status_penjadwalan::status_penjadwalan,
            updated_at = @updated_at WHERE id_penjadwalan = @id_penjadwalan";

            try
            {
                DBHelper db = new DBHelper(this.__constr);
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);

                cmd.Parameters.AddWithValue("@id_penjadwalan", jadwal.Id_Penjadwalan);
                cmd.Parameters.AddWithValue("@nama_penjadwalan", jadwal.Nama_Penjadwalan);
                cmd.Parameters.AddWithValue("@tanggal_penjadwalan", jadwal.Tanggal_Penjadwalan);
                cmd.Parameters.AddWithValue("@waktu_penjadwalan", jadwal.Waktu_Penjadwalan);
                cmd.Parameters.AddWithValue("@id_lokasi", jadwal.Id_Lokasi);
                cmd.Parameters.AddWithValue("@deskripsi_penjadwalan", jadwal.Deskripsi_Penjadwalan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status_penjadwalan", jadwal.Status_Penjadwalan.ToString().ToLower());
                cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

                int rowsAffected = cmd.ExecuteNonQuery();
                result = rowsAffected > 0;

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error UpdatePenjadwalan: " + ex.Message);
            }

            return result;
        }

        public bool DeletePenjadwalanById(int idPenjadwalan)
        {
            bool result = false;

            string query = "DELETE FROM penjadwalan WHERE id_penjadwalan = @id_penjadwalan;";

            try
            {
                DBHelper db = new DBHelper(this.__constr);
                using var cmd = db.GetNpgsqlCommand(query);

                cmd.Parameters.AddWithValue("@id_penjadwalan", idPenjadwalan);

                int affectedRows = cmd.ExecuteNonQuery();
                result = affectedRows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error DeletePenjadwalan: " + ex.Message);
            }

            return result;
        }

        public Penjadwalan? GetPenjadwalanTerdekat()
        {
            Penjadwalan? penjadwalan = null;
            string query = @"
                SELECT * FROM penjadwalan
                WHERE (tanggal_penjadwalan + waktu_penjadwalan) >= CURRENT_TIMESTAMP
                ORDER BY (tanggal_penjadwalan + waktu_penjadwalan) ASC
                LIMIT 1;
    ";

            DBHelper db = new DBHelper(this.__constr);
            try
            {
                var cmd = db.GetNpgsqlCommand(query);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Enum.TryParse<StatusPenjadwalan>(
                        reader.GetString(reader.GetOrdinal("status_penjadwalan")),
                        true,
                        out StatusPenjadwalan status
                    );
                    penjadwalan = new Penjadwalan
                    {
                        Id_Penjadwalan = int.Parse(reader["id_penjadwalan"].ToString()),
                        Nama_Penjadwalan = reader["nama_penjadwalan"].ToString(),
                        Tanggal_Penjadwalan = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("tanggal_penjadwalan"))),
                        Waktu_Penjadwalan = reader.GetTimeSpan(reader.GetOrdinal("waktu_penjadwalan")),
                        Id_Lokasi = int.Parse(reader["id_lokasi"].ToString()),
                        Deskripsi_Penjadwalan = reader["deskripsi_penjadwalan"].ToString(),
                        Status_Penjadwalan = status
                    };
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }

            return penjadwalan;
        }
    }
}
