using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WellingtonWeatherRecommendationApi.Models;
using WellingtonWeatherRecommendationApi.Repositories;

using Xunit;

namespace WellingtonWeatherApi.Tests.Repositories
{
    public class WeatherRepositoryTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly WeatherRepository _weatherRepository;

        public WeatherRepositoryTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("test-api-key");
            _configurationMock.Setup(c => c["OpenWeatherMap:BaseUrl"]).Returns("https://api.openweathermap.org/data/2.5/weather");
            _weatherRepository = new WeatherRepository(_httpClient, _configurationMock.Object);
        }

        [Fact]
        public async Task GetWeatherAsync_ValidCoordinates_ReturnsWeatherApiResponse()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var weatherApiResponse = new WeatherApiResponse
            {
                Main = new Main { Temp = 12.5, Humidity = 80 },
                Wind = new Wind { Speed = 7.0 },
                Weather = new[] { new Weather { Main = "Rain" } }
            };
            var jsonResponse = JsonConvert.SerializeObject(weatherApiResponse);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _weatherRepository.GetWeatherAsync(latitude, longitude);

            // Assert
            result.Should().NotBeNull();
            result.Main.Temp.Should().Be(12.5);
            result.Wind.Speed.Should().Be(7.0);
            result.Weather[0].Main.Should().Be("Rain");
        }

        [Fact]
        public async Task GetWeatherAsync_ApiReturnsError_ThrowsHttpRequestException()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _weatherRepository.GetWeatherAsync(latitude, longitude));
        }

        [Fact]
        public async Task GetWeatherAsync_NullDeserialization_ThrowsException()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("", Encoding.UTF8, "application/json") // Empty or invalid JSON
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _weatherRepository.GetWeatherAsync(latitude, longitude));
            exception.Message.Should().Be("Failed to deserialize weather API response");
        }
    }
}