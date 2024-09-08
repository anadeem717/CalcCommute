using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Newtonsoft.Json.Linq;

namespace CommuteTests {

    public class ApiKeyReaderTests
    {
        [Theory]
        [InlineData("test_api_key.txt", "test_api_key", true)] // Valid file path and API key
        [InlineData("non-existent-file.txt", null, false)] // Invalid file path
        public async Task ReadApiKeyFromFile(string apiKeyFile, string expectedApiKey, bool fileExists)
        {
            if (fileExists)
            {
                await File.WriteAllTextAsync(apiKeyFile, expectedApiKey ?? string.Empty);
            }
            
            var apiKeyReader = new ApiKeyReader();

            string result = await apiKeyReader.ReadApiKeyFromFile(apiKeyFile);

            Assert.Equal(expectedApiKey, result);
        }
    }

    public class LocationTests
    {
        [Theory]
        [InlineData("123 Address1 Rd", "456 Address2 Rd", 60, 100)] 
        public async Task FindDistance_ReturnsCorrectValues(string addr1, string addr2, double expectedDuration, double expectedDistance)
        {
            var mockLocationFetcher = new Mock<IFetchGmapLocation>();

            // mock JSON object
            JObject mockResponse = new JObject(
                new JProperty("routes", new JArray(
                    new JObject(
                        new JProperty("legs", new JArray(
                            new JObject(
                                new JProperty("duration", new JObject(new JProperty("value", expectedDuration * 60))), // Convert minutes to seconds
                                new JProperty("distance", new JObject(new JProperty("value", expectedDistance * 1609.34))) // Convert miles to meters
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
