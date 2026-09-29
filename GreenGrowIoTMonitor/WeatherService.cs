using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace GreenGrowIoTMonitor;

public class WeatherSnapshot
{
    public SensorReading Temperature { get; set; } = new();
    public SensorReading Humidity { get; set; } = new();
    public SensorReading WindSpeed { get; set; } = new();
    public SensorReading? SoilMoisture { get; set; }   // may be null
}

public class WeatherService
{
    public const string LocationName = "Cape Town, South Africa";

    private const double Latitude = -33.9249;
    private const double Longitude = 18.4241;

    private static readonly HttpClient _client = new();

    public async Task<WeatherSnapshot> GetCurrentWeatherAsync()
    {
        string lat = Latitude.ToString(CultureInfo.InvariantCulture);
        string lon = Longitude.ToString(CultureInfo.InvariantCulture);

        string url = "https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={lat}&longitude={lon}" +
                     "&current=temperature_2m,relative_humidity_2m,wind_speed_10m" +
                     "&hourly=soil_moisture_0_to_1cm" +
                     "&timezone=auto";

        string json = await _client.GetStringAsync(url);

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        JsonElement current = root.GetProperty("current");
        DateTime now = DateTime.Now;

        var snapshot = new WeatherSnapshot
        {
            Temperature = new SensorReading
            {
                SensorName = "Temperature",
                Value = current.GetProperty("temperature_2m").GetDouble(),
                Unit = "°C",
                Status = "ONLINE",
                Timestamp = now
            },
            Humidity = new SensorReading
            {
                SensorName = "Humidity",
                Value = current.GetProperty("relative_humidity_2m").GetDouble(),
                Unit = "%",
                Status = "ONLINE",
                Timestamp = now
            },
            WindSpeed = new SensorReading
            {
                SensorName = "Wind Speed",
                Value = current.GetProperty("wind_speed_10m").GetDouble(),
                Unit = "km/h",
                Status = "ONLINE",
                Timestamp = now
            }
        };

        // Soil moisture is hourly only, so find the entry for the current hour
        string currentTime = current.GetProperty("time").GetString() ?? "";
        if (currentTime.Length >= 13)
        {
            string currentHour = currentTime.Substring(0, 13) + ":00";

            JsonElement hourly = root.GetProperty("hourly");
            JsonElement times = hourly.GetProperty("time");
            JsonElement soil = hourly.GetProperty("soil_moisture_0_to_1cm");

            for (int i = 0; i < times.GetArrayLength(); i++)
            {
                if (times[i].GetString() == currentHour &&
                    soil[i].ValueKind == JsonValueKind.Number)
                {
                    snapshot.SoilMoisture = new SensorReading
                    {
                        SensorName = "Soil Moisture",
                        Value = soil[i].GetDouble(),
                        Unit = "m³/m³",
                        Status = "ONLINE",
                        Timestamp = now
                    };
                    break;
                }
            }
        }

        return snapshot;
    }
}