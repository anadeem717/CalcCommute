using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Newtonsoft.Json.Linq;

namespace CommuteTests {

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

    public class LocationTests
    {
        [Fact]
        public async Task FindDistance_ReturnsCorrectValues()
        {
        
            var mockLocationFetcher = new Mock<IFetchGmapLocation>();
            string addr1 = "123 Address1 Rd";
            string addr2 = "456 Address2 Rd";
            double expectedDuration = 60; // Expected duration in minutes
            double expectedDistance = 100; // Expected distance in miles

            // mock JSON object
            JObject mockResponse = new JObject(
                new JProperty("routes", new JArray(
                    new JObject(
                        new JProperty("legs", new JArray(
                            new JObject(
                                new JProperty("duration", new JObject(new JProperty("value", 3600))), // Duration in seconds
                                new JProperty("distance", new JObject(new JProperty("value", 160934))) // Distance in meters
                            )
                        ))
                    )
                ))
            );

            mockLocationFetcher.Setup(f => f.GetLocationAsync(addr1, addr2)).ReturnsAsync(mockResponse);

            var distFinder = new DistanceFinder(mockLocationFetcher.Object);

            var (duration, distance) = await distFinder.FindDistanceAsync(addr1, addr2);

            Assert.Equal(expectedDuration, duration);
            Assert.Equal(expectedDistance, distance);
        }
    }
}


