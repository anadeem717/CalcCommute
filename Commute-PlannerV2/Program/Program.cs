using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKeyFile = "Maps-API.txt";

        string apiKey = await ReadApiKeyFromFile(apiKeyFile);
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found");
            return;
        }

        string origin;
        string destination;

        HttpClient client = new HttpClient();
        
        Console.Write("Enter origin as {address} or {city, state}: ");
        origin = Console.ReadLine();

        Console.Write("Enter destination address as {address} or {city, state}: ");
        destination = Console.ReadLine();

        string apiUrl = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={apiKey}";

        Console.WriteLine("\nCalculating distance...");
        HttpResponseMessage response = await client.GetAsync(apiUrl);
        
        Console.WriteLine("\nSuccessfully retrieved data from Maps.");
        string responseBody = await response.Content.ReadAsStringAsync();

        // Parse JSON response
        Console.WriteLine("\nParsing Data....\n");
        JObject json = JObject.Parse(responseBody);

        // Check if the status is OK
        if (json["status"].ToString() != "OK")
        {
            Console.WriteLine("Error: Directions not found.");
            return;
        }

        // Get total duration and total distance
        double totalDuration = double.Parse(json["routes"][0]["legs"][0]["duration"]["value"].ToString()) / 60.0;
        double totalDistanceMeters = double.Parse(json["routes"][0]["legs"][0]["distance"]["value"].ToString());
        double totalDistanceMiles = totalDistanceMeters / 1609.34; // Convert meters to miles

        Console.WriteLine($"Total Duration: {totalDuration:F2} min / {totalDuration / 60:F2} hrs");
        Console.WriteLine($"Total Distance: {totalDistanceMiles:F2} miles");
}

    static async Task<string> ReadApiKeyFromFile(string filePath)
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
