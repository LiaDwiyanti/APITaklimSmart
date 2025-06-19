using APITaklimSmart.Helpers;
using Npgsql;

namespace APITaklimSmart.Models
{
    public class DokumentasiContext
    {
        private string __constr;
        private string __errorMsg;

        public DokumentasiContext(string pObs)
        {
            __constr = pObs;
        }

        public List<Dokumentasi> ReadDokumentasi()
        {
            List<Dokumentasi> listDokumentasi = new List<Dokumentasi>();
            string query = "SELECT  dokumentasi.*, penjadwalan.nama_penjadwalan AS nama_penjadwalan FROM dokumentasi LEFT JOIN penjadwalan ON dokumentasi.id_penjadwalan = penjadwalan.id_penjadwalan;";
            DBHelper db = new DBHelper(this.__constr);

            try
            {
                using (NpgsqlCommand cmd = db.GetNpgsqlCommand(query))
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listDokumentasi.Add(new Dokumentasi()
                        {
                            Id_Dokumentasi = Convert.ToInt32(reader["id_dokumentasi"]),
                            Id_Penjadwalan = Convert.ToInt32(reader["id_penjadwalan"]),
                            Nama_Penjadwalan = reader["nama_penjadwalan"]?.ToString(),
                            Uploaded_By = Convert.ToInt32(reader["uploaded_by"]),
                            File_Name = reader["file_name"]?.ToString(),
                            File_Url = reader["file_url"]?.ToString(),
                            File_Path = reader["file_path"]?.ToString(),
                            Caption_Dokumentasi = reader["caption_dokumentasi"]?.ToString(),
                            Upload_Date = reader["upload_date"] != DBNull.Value
                                ? Convert.ToDateTime(reader["upload_date"])
                                : DateTime.UtcNow
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat membaca data dokumentasi: " + __errorMsg);
            }

            db.CloseConnection();
            return listDokumentasi;
        }

        public bool InsertDokumentasi(Dokumentasi dokumentasi)
        {
            bool result = false;
            try
            {
                string query = "INSERT INTO dokumentasi (id_penjadwalan, uploaded_by, file_name, file_url, file_path, caption_dokumentasi, upload_date) " +
                    "VALUES (@id_penjadwalan, @uploaded_by, @file_name, @file_url, @file_path, @caption_dokumentasi, @upload_date);";
                DBHelper db = new DBHelper(this.__constr);
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_penjadwalan", dokumentasi.Id_Penjadwalan);
                cmd.Parameters.AddWithValue("@uploaded_by", dokumentasi.Uploaded_By);
                cmd.Parameters.AddWithValue("@file_name", dokumentasi.File_Name);
                cmd.Parameters.AddWithValue("@file_url", dokumentasi.File_Url);
                cmd.Parameters.AddWithValue("@file_path", dokumentasi.File_Path);
                cmd.Parameters.AddWithValue("@caption_dokumentasi", dokumentasi.Caption_Dokumentasi);
                cmd.Parameters.AddWithValue("@upload_date", dokumentasi.Upload_Date);

                int rowsAffected = cmd.ExecuteNonQuery();
                result = rowsAffected > 0;

                cmd.Dispose();
                db.CloseConnection();
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine("Postgres error: " + ex.Message);
                Console.WriteLine("SQL State: " + ex.SqlState);
                Console.WriteLine("Detail: " + ex.Detail);
                Console.WriteLine("Where: " + ex.Where);
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Terjadi kesalahan saat menambah data :" + __errorMsg);
            }
            return result;
        }

        public bool EditDokumentasi(Dokumentasi dok)
        {
            string query = @"UPDATE dokumentasi SET 
                        id_penjadwalan = @id_penjadwalan,
                        uploaded_by = @uploaded_by,
                        file_name = @file_name,
                        file_url = @file_url,
                        file_path = @file_path,
                        caption_dokumentasi = @caption_dokumentasi,
                        upload_date = @upload_date
                    WHERE id_dokumentasi = @id_dokumentasi";

            DBHelper db = new DBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.Parameters.AddWithValue("@id_dokumentasi", dok.Id_Dokumentasi);
                cmd.Parameters.AddWithValue("@id_penjadwalan", dok.Id_Penjadwalan);
                cmd.Parameters.AddWithValue("@uploaded_by", dok.Uploaded_By);
                cmd.Parameters.AddWithValue("@file_name", dok.File_Name);
                cmd.Parameters.AddWithValue("@file_url", dok.File_Url);
                cmd.Parameters.AddWithValue("@file_path", dok.File_Path);
                cmd.Parameters.AddWithValue("@caption_dokumentasi", dok.Caption_Dokumentasi);
                cmd.Parameters.AddWithValue("@upload_date", dok.Upload_Date);

                int affected = cmd.ExecuteNonQuery();
                cmd.Dispose();
                db.CloseConnection();
                return affected > 0;
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal edit dokumentasi: " + __errorMsg);
                return false;
            }
        }

        public bool DeleteDokumentasi(int id)
        {
            string filePath = string.Empty;
            string fileName = string.Empty;

            try
            {
                DBHelper dbGet = new DBHelper(this.__constr);
                string getQuery = "SELECT file_path, file_name FROM dokumentasi WHERE id_dokumentasi = @id";
                NpgsqlCommand getCmd = dbGet.GetNpgsqlCommand(getQuery);
                getCmd.Parameters.AddWithValue("@id", id);
                using (var reader = getCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        filePath = reader["file_path"].ToString();
                        fileName = reader["file_name"].ToString();
                    }
                }
                getCmd.Dispose();
                dbGet.CloseConnection();

                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(fileName))
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'), fileName);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                DBHelper dbDelete = new DBHelper(this.__constr);
                string deleteQuery = "DELETE FROM dokumentasi WHERE id_dokumentasi = @id";
                NpgsqlCommand deleteCmd = dbDelete.GetNpgsqlCommand(deleteQuery);
                deleteCmd.Parameters.AddWithValue("@id", id);
                int affectedRows = deleteCmd.ExecuteNonQuery();
                deleteCmd.Dispose();
                dbDelete.CloseConnection();

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                __errorMsg = ex.Message;
                Console.WriteLine("Gagal menghapus dokumentasi: " + __errorMsg);
                return false;
            }
        }
    }
}
