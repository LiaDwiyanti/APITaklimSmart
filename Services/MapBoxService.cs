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

        public (double lat, double lon) GetKordinatLokasi(string alamat)
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
                        double lon = coords[0].GetDouble();
                        double lat = coords[1].GetDouble();

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
    }
}
