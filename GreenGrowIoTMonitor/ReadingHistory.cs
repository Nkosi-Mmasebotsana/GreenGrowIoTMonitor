namespace GreenGrowIoTMonitor;

public class ReadingHistory
{
    private readonly Stack<SensorReading> _readings = new();

    public int Count => _readings.Count;
    public bool IsEmpty => _readings.Count == 0;

    // Push: adds to the top of the stack
    public void AddReading(SensorReading reading)
    {
        _readings.Push(reading);
    }

    // Peek: looks at the top WITHOUT removing it
    public SensorReading? GetLatest()
    {
        if (IsEmpty) return null;
        return _readings.Peek();
    }

    // Pop: removes and returns the top
    public SensorReading? RemoveLatest()
    {
        if (IsEmpty) return null;
        return _readings.Pop();
    }

    // Enumerating a Stack goes top -> bottom (newest first)
    public IEnumerable<SensorReading> GetAll()
    {
        return _readings;
    }
}