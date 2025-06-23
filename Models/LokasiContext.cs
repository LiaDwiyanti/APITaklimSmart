using APITaklimSmart.Helpers;
using Npgsql;

namespace APITaklimSmart.Models
{
    public class LokasiContext
    {
        private string __constr;
        public string __errorMsg;

        public LokasiContext(string pObs)
        {
            __constr = pObs;
        }

        public List<Lokasi> ReadLokasi()
        {
            List<Lokasi> listLokasi = new List<Lokasi>();
            string query = "select * from lokasi";
            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listLokasi.Add(new Lokasi()
                    {
                        Id_Lokasi = int.Parse(reader["id_lokasi"].ToString()),
                        Nama_Lokasi = reader["nama_lokasi"]?.ToString(),
                        Alamat = reader["alamat"]?.ToString(),
                        Latitude = reader.GetDecimal(reader.GetOrdinal("latitude")),
                        Longitude = reader.GetDecimal(reader.GetOrdinal("longitude"))
                    });
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data Lokasi :" + __errorMsg);
            }
            return listLokasi;
        }

        public Lokasi? ReadLokasiById(int id)
        {
            Lokasi? lokasi = null;
            string query = "SELECT * FROM lokasi WHERE id_lokasi = @id";

            DBHelper db = new DBHelper(__constr);
            try
            {
                var cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lokasi = new Lokasi
                    {
                        Id_Lokasi = Convert.ToInt32(reader["id_lokasi"]),
                        Nama_Lokasi = reader["nama_lokasi"]?.ToString(),
                        Alamat = reader["alamat"]?.ToString(),
                        Latitude = reader.GetDecimal(reader.GetOrdinal("latitude")),
                        Longitude = reader.GetDecimal(reader.GetOrdinal("longitude"))
                    };
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
            }

            return lokasi;
        }
    }
}
