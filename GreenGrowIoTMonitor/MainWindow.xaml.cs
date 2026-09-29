using System.Windows;
using System.Windows.Media;

namespace GreenGrowIoTMonitor;

public partial class MainWindow : Window
{
    private readonly WeatherService _weatherService = new();
    private readonly ReadingHistory _history = new();

    public MainWindow()
    {
        InitializeComponent();
        txtLocation.Text = "Location: " + WeatherService.LocationName;
        RefreshHistoryDisplay();
    }

    private async void btnRetrieve_Click(object sender, RoutedEventArgs e)
    {
        btnRetrieve.IsEnabled = false;
        try
        {
            WeatherSnapshot data = await _weatherService.GetCurrentWeatherAsync();

            DisplayCurrentData(data);
            _history.AddReading(data.Temperature);   // Push
            SetStatus("ONLINE", Brushes.SeaGreen);
            RefreshHistoryDisplay();
        }
        catch (Exception ex)
        {
            SetStatus("OFFLINE", Brushes.Red);
            MessageBox.Show("Could not retrieve data: " + ex.Message,
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            btnRetrieve.IsEnabled = true;
        }
    }

    private void btnPeek_Click(object sender, RoutedEventArgs e)
    {
        SensorReading? latest = _history.GetLatest();   // Peek (does not remove)
        if (latest == null)
        {
            MessageBox.Show("No readings stored.", "Peek");
            return;
        }

        MessageBox.Show($"Latest reading: {latest}\nStored readings: {_history.Count}", "Peek");
        RefreshHistoryDisplay();
    }

    private void btnPop_Click(object sender, RoutedEventArgs e)
    {
        SensorReading? removed = _history.RemoveLatest();   // Pop (removes)
        if (removed == null)
        {
            MessageBox.Show("There are no readings to remove.", "Pop");
            return;
        }

        MessageBox.Show($"Removed: {removed}\nStored readings: {_history.Count}", "Pop");
        RefreshHistoryDisplay();
    }

    private void DisplayCurrentData(WeatherSnapshot d)
    {
        txtTemperature.Text = $"Temperature: {d.Temperature.Value:0.0} {d.Temperature.Unit}";
        txtHumidity.Text = $"Humidity: {d.Humidity.Value:0} {d.Humidity.Unit}";
        txtWind.Text = $"Wind Speed: {d.WindSpeed.Value:0.0} {d.WindSpeed.Unit}";
        txtSoil.Text = d.SoilMoisture != null
            ? $"Soil Moisture: {d.SoilMoisture.Value:0.00} {d.SoilMoisture.Unit}"
            : "Soil Moisture: not available";
    }

    private void SetStatus(string text, Brush colour)
    {
        txtStatus.Text = "System Status: " + text;
        txtStatus.Foreground = colour;
    }

    private void RefreshHistoryDisplay()
    {
        SensorReading? latest = _history.GetLatest();
        txtLatest.Text = latest == null
            ? "Latest Reading: No readings stored"
            : "Latest Reading: " + latest;
        txtCount.Text = "Stored Readings: " + _history.Count;

        lstHistory.Items.Clear();
        foreach (SensorReading r in _history.GetAll())
            lstHistory.Items.Add(r.ToString());
    }
}