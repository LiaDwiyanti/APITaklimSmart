using APITaklimSmart.Helpers;

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
