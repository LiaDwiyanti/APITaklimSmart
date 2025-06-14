using APITaklimSmart.Helpers;
using Npgsql;

namespace APITaklimSmart.Models
{
    public class LokasiContext
    {
        private string __constr;
        private string __errorMsg;

        public LokasiContext(string pObs)
        {
            __constr = pObs;
        }

        public bool SaveLokasi(Lokasi lokasi)
        {
            bool result = false;
            string query = "INSERT INTO lokasi (nama_lokasi, alamat, latitude, longitude, deskripsi_lokasi, created_at) " +
                           "VALUES (@nama_lokasi, @alamat, @latitude, @longitude, @deskripsi_lokasi, @created_at);";

            DBHelper db = new DBHelper(__constr);
            try
            {
                var cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@nama_lokasi", lokasi.Nama_Lokasi);
                cmd.Parameters.AddWithValue("@alamat", lokasi.Alamat);
                cmd.Parameters.AddWithValue("@latitude", lokasi.Latitude);
                cmd.Parameters.AddWithValue("@longitude", lokasi.Longitude);
                cmd.Parameters.AddWithValue("@deskripsi_lokasi", lokasi.Deskripsi_Lokasi ?? "");
                cmd.Parameters.AddWithValue("@created_at", lokasi.CreatedAt);

                int rowsAffected = cmd.ExecuteNonQuery();
                result = rowsAffected > 0;

                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal menyimpan lokasi: " + __errorMsg);
            }

            return result;
        }
    }
}
