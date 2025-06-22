using APITaklimSmart.Helpers;
using Npgsql;
using System.Data;

namespace APITaklimSmart.Models
{
    public class RiwayatContext
    {
        private string __constr;
        private string __errorMsg;

        public RiwayatContext(string pObs)
        {
            __constr = pObs;
        }
        public List<Riwayat> ReadRiwayatAll()
        {
            List<Riwayat> listRiwayat = new List<Riwayat>();
            string query = "select * from riwayat";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listRiwayat.Add(new Riwayat()
                    {
                        Id_Riwayat = int.Parse(reader["id_riwayat"].ToString()),
                        Id_Penjadwalan = int.Parse(reader["id_penjadwalan"].ToString()),
                        Status_Lama = reader.IsDBNull("status_lama") ? null :
                                      Enum.Parse<StatusPenjadwalan>(reader["status_lama"].ToString(), true),
                        Status_Baru = Enum.Parse<StatusPenjadwalan>(reader["status_baru"].ToString(), true),
                        Changed_By = reader.GetInt32(reader.GetOrdinal("changed_by")),
                        Alasan = reader["alasan"]?.ToString(),
                        Changed_At = Convert.ToDateTime(reader["changed_at"])
                    });
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data riwayat :" + __errorMsg);
            }
            return listRiwayat;
        }

        public Riwayat ReadRiwayatById(int idRiwayat)
        {
            Riwayat riwayat = null;
            string query = "select * from riwayat where id_riwayat=@id_riwayat";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_riwayat", idRiwayat);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    riwayat = new Riwayat()
                    {
                        Id_Riwayat = int.Parse(reader["id_riwayat"].ToString()),
                        Id_Penjadwalan = int.Parse(reader["id_penjadwalan"].ToString()),
                        Status_Lama = reader.IsDBNull("status_lama") ? null :
                                      Enum.Parse<StatusPenjadwalan>(reader["status_lama"].ToString(), true),
                        Status_Baru = Enum.Parse<StatusPenjadwalan>(reader["status_baru"].ToString(), true),
                        Changed_By = reader.GetInt32(reader.GetOrdinal("changed_by")),
                        Alasan = reader["alasan"]?.ToString(),
                        Changed_At = Convert.ToDateTime(reader["changed_at"])
                    };
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data penjadwalan :" + __errorMsg);
            }
            return riwayat;
        }

        public bool UpdateStatusRiwayat(Riwayat data)
        {
            bool riwayat = false;
            string query = "UPDATE riwayat SET status_lama = @status_lama::status_penjadwalan, " +
                           "status_baru = @status_baru::status_penjadwalan, changed_by = @changed_by, changed_at = @changed_at " +
                           "WHERE id_riwayat = @id_riwayat";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@status_lama", data.Status_Lama.ToString().ToLower());
                cmd.Parameters.AddWithValue("@status_baru", data.Status_Baru.ToString().ToLower());
                cmd.Parameters.AddWithValue("@changed_by", data.Changed_By);
                cmd.Parameters.AddWithValue("@changed_at", data.Changed_At);
                cmd.Parameters.AddWithValue("@id_riwayat", data.Id_Riwayat);
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal update riwayat: " + __errorMsg);
            }
            return riwayat;
        }

        public bool UpdateStatusRiwayat(int id, UpdateRiwayatRequest input)
        {
            bool result = false;
            string updateQuery = "UPDATE riwayat SET status_lama = @status_lama::status_penjadwalan, " +
                                 "status_baru = @status_baru::status_penjadwalan, changed_by = @changed_by, changed_at = @changed_at " +
                                 "WHERE id_riwayat = @id_riwayat";

            DBHelper db = new DBHelper(this.__constr);
            try
            {
                var existing = ReadRiwayatById(id);
                if (existing == null)
                {
                    __errorMsg = "Riwayat tidak ditemukan.";
                    return false;
                }

                using (var cmdUpdate = db.GetNpgsqlCommand(updateQuery))
                {
                    cmdUpdate.Parameters.AddWithValue("@status_lama", existing.Status_Baru.ToString().ToLower());
                    cmdUpdate.Parameters.AddWithValue("@status_baru", input.Status_Baru.ToString().ToLower());
                    cmdUpdate.Parameters.AddWithValue("@changed_by", input.Changed_By);
                    cmdUpdate.Parameters.AddWithValue("@changed_at", input.Changed_At);
                    cmdUpdate.Parameters.AddWithValue("@id_riwayat", id);

                    int affected = cmdUpdate.ExecuteNonQuery();
                    result = affected > 0;
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal update status riwayat: " + __errorMsg);
            }

            return result;
        }
    }
}
