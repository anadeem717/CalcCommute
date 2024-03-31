using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

// Defines a contract for fetching location data using Google Maps API.
public interface IFetchGmapLocation
{
    Task<JObject> GetLocationAsync(string origin, string destination);
}

// Implements IFetchGmapLocation to fetch data from Google Maps API.
public class GmapFetcher : IFetchGmapLocation
{
    private string ApiKey;
    private HttpClient Client;

    public GmapFetcher(string apiKey, HttpClient client)
    {
        ApiKey = apiKey;
        Client = client;
    }

    // Fetches directions between two locations using Google Maps API.
    public async Task<JObject> GetLocationAsync(string origin, string destination)
    {
        string apiUrl = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={ApiKey}";
        HttpResponseMessage response = await Client.GetAsync(apiUrl);
        string responseBody = await response.Content.ReadAsStringAsync();
        return JObject.Parse(responseBody);
    }
}

// Defines a contract for finding the distance and duration between two addresses.
public interface IClosestDistanceFinder
{
    Task<(double duration, double distance)> FindDistanceAsync(string addr1, string addr2);
}

// Implements IClosestDistanceFinder to calculate distance and duration based on location data.
public class DistanceFinder : IClosestDistanceFinder
{
    private IFetchGmapLocation Fetcher;

    public DistanceFinder(IFetchGmapLocation fetcher)
    {
        Fetcher = fetcher;
    }

    // Calculates the distance and duration between two addresses using fetched location data.
    public async Task<(double duration, double distance)> FindDistanceAsync(string addr1, string addr2)
    {
        var locationData = await Fetcher.GetLocationAsync(addr1, addr2);
        double totalDuration = double.Parse(locationData["routes"][0]["legs"][0]["duration"]["value"].ToString()) / 60.0;
        double totalDistanceMeters = double.Parse(locationData["routes"][0]["legs"][0]["distance"]["value"].ToString());
        double totalDistanceMiles = totalDistanceMeters / 1609.34; // Convert meters to miles

        return (totalDuration, totalDistanceMiles);
    }
}

// Defines a contract for reading an API key from a file.
public interface IApiKeyReader
{
    Task<string> ReadApiKeyFromFile(string filePath);
}

// Implements IApiKeyReader to read an API key from a specified file.
public class ApiKeyReader : IApiKeyReader
{
    public async Task<string?> ReadApiKeyFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string apiKey = await File.ReadAllTextAsync(filePath);
                return apiKey.Trim();
            }
            else
            {
                Console.WriteLine($"File '{filePath}' not found.");
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading API key from file: {e.Message}");
            return null;
        }
    }
}

// Main Program
class Program
{
    static async Task Main(string[] args)
    {
        string apiKeyFile = "Maps-API.txt";
        var apiKeyReader = new ApiKeyReader();
        string? apiKey = await apiKeyReader.ReadApiKeyFromFile(apiKeyFile);

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found");
            return;
        }

        HttpClient client = new HttpClient();
        var gmapFetcher = new GmapFetcher(apiKey, client);
        var distFinder = new DistanceFinder(gmapFetcher);

        // Prompt user for origin and destination
        Console.Write("Enter origin as {address} or {city, state}: ");
        var origin = Console.ReadLine();
        Console.Write("Enter destination address as {address} or {city, state}: ");
        var destination = Console.ReadLine();

        // Calculate and display the distance and duration if valid input provided
        if (!string.IsNullOrWhiteSpace(origin) && !string.IsNullOrWhiteSpace(destination))
        {
            var (duration, distance) = await distFinder.FindDistanceAsync(origin, destination);
            Console.WriteLine($"Total Duration: {duration:F2} min / {duration / 60:F2} hrs");
            Console.WriteLine($"Total Distance: {distance:F2} miles");
        }
        else
        {
            Console.WriteLine("Origin or destination is not provided.");
        }
    }
}