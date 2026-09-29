# GreenGrow IoT Monitor

A C# WPF desktop app that monitors greenhouse conditions for GreenGrow Farms.

## Features
- Retrieves live weather data for Cape Town from the Open-Meteo API
- Displays temperature, humidity, wind speed and soil moisture
- Stores temperature readings in a Stack (LIFO)
- Push (retrieve), Peek (view latest) and Pop (remove latest)
- Handles an empty Stack without crashing

## Structure
- `SensorReading.cs`: represents a sensor reading
- `WeatherService.cs`: calls the API and parses the JSON
- `ReadingHistory.cs`: wraps the Stack
- `MainWindow.xaml` / `.xaml.cs`: the user interface

## Run
Open the `.sln` in Visual Studio and press F5. Internet access is required.
