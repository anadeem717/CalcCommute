using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public interface IApiKeyReader
{
    Task<string> ReadApiKeyFromFile(string filePath);
}

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

public class ApiKeyReaderTests
{
    [Fact]
    public async Task ReadApiKeyFromFile_ValidFilePath()
    {
    
        // mock API key and file
        string apiKey = "test_api_key";
        string apiKeyFile = "test_api_key.txt";
        
        await File.WriteAllTextAsync(apiKeyFile, apiKey);
        var apiKeyReader = new ApiKeyReader();

        string result = await apiKeyReader.ReadApiKeyFromFile(apiKeyFile);

        Assert.Equal(apiKey, result);
    }

    [Fact]
    public async Task ReadApiKeyFromFile_InvalidFilePath()
    {
        string apiKeyFile = "non-existent-file.txt";
        var apiKeyReader = new ApiKeyReader();

        string result = await apiKeyReader.ReadApiKeyFromFile(apiKeyFile);

        // result should be null since the file does not exist
        Assert.Null(result);
    }

}
