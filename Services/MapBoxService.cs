using System.Globalization;
using System.Net;
using System.Text.Json;

namespace APITaklimSmart.Services
{
    public class MapBoxService
    {
        private readonly string _accessToken;

        public MapBoxService(IConfiguration configuration)
        {
            _accessToken = configuration["MapBoxAccessToken"];
        }

        public (decimal lat, decimal lon) GetKordinatLokasi(string alamat)
        {
            string url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{Uri.EscapeDataString(alamat)}.json?access_token={_accessToken}&limit=1";

            using (var client = new WebClient())
            {
                try
                {
                    var response = client.DownloadString(url);
                    var geoData = JsonDocument.Parse(response);

                    var features = geoData.RootElement.GetProperty("features");
                    if (features.GetArrayLength() > 0)
                    {
                        var coords = features[0].GetProperty("geometry").GetProperty("coordinates");
                        decimal lon = (decimal)coords[0].GetDouble();
                        decimal lat = (decimal)coords[1].GetDouble();

                        return (lat, lon);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Gagal mengambil koordinat: " + ex.Message);
                }
                return (0, 0);
            }
        }
        public string GetAlamatDariKoordinat(decimal lat, decimal lon)
        {
            string url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{lon.ToString(CultureInfo.InvariantCulture)}{lat.ToString(CultureInfo.InvariantCulture)}.json?access_token={_accessToken}&limit=1";;

            using (var client = new WebClient())
            {
                try
                {
                    var response = client.DownloadString(url);
                    var geoData = JsonDocument.Parse(response);

                    var features = geoData.RootElement.GetProperty("features");
                    if (features.GetArrayLength() > 0)
                    {
                        string alamat = features[0].GetProperty("place_name").GetString();
                        return alamat;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Gagal reverse geocoding: " + ex.Message);
                }

                return "Alamat tidak ditemukan";
            }
        }
    }
}
