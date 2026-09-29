namespace GreenGrowIoTMonitor;

public class SensorReading
{
    public string SensorName { get; set; } = "";
    public double Value { get; set; }
    public string Unit { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return $"{Value:0.0} {Unit}  ({Timestamp:HH:mm:ss})";
    }
}