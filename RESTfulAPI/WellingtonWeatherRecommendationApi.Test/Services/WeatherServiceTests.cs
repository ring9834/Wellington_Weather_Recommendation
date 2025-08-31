using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using WellingtonWeatherRecommendationApi.Models;
using WellingtonWeatherRecommendationApi.Repositories;
using WellingtonWeatherRecommendationApi.Services;
using Xunit;

namespace WellingtonWeatherApi.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
        private readonly Mock<IRecommendationService> _recommendationServiceMock;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _weatherRepositoryMock = new Mock<IWeatherRepository>();
            _recommendationServiceMock = new Mock<IRecommendationService>();
            _weatherService = new WeatherService(_weatherRepositoryMock.Object, _recommendationServiceMock.Object);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_NullWeatherData_ThrowsException()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            _weatherRepositoryMock
                .Setup(r => r.GetWeatherAsync(latitude, longitude))
                .ReturnsAsync((WeatherApiResponse)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _weatherService.GetWeatherForecastAsync(latitude, longitude));
            exception.Message.Should().Be("Invalid weather data received from API");
        }

        [Fact]
        public async Task GetWeatherForecastAsync_NullWeatherArray_ThrowsException()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var weatherApiResponse = new WeatherApiResponse
            {
                Main = new Main { Temp = 12.5 },
                Wind = new Wind { Speed = 7.0 },
                Weather = null
            };
            _weatherRepositoryMock
                .Setup(r => r.GetWeatherAsync(latitude, longitude))
                .ReturnsAsync(weatherApiResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _weatherService.GetWeatherForecastAsync(latitude, longitude));
            exception.Message.Should().Be("Invalid weather data received from API");
        }

        [Fact]
        public async Task GetWeatherForecastAsync_EmptyWeatherArray_ThrowsException()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var weatherApiResponse = new WeatherApiResponse
            {
                Main = new Main { Temp = 12.5 },
                Wind = new Wind { Speed = 7.0 },
                Weather = new Weather[0]
            };
            _weatherRepositoryMock
                .Setup(r => r.GetWeatherAsync(latitude, longitude))
                .ReturnsAsync(weatherApiResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _weatherService.GetWeatherForecastAsync(latitude, longitude));
            exception.Message.Should().Be("Invalid weather data received from API");
        }

        [Fact]
        public async Task GetWeatherForecastAsync_ValidData_ReturnsWeatherResponse()
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var weatherApiResponse = new WeatherApiResponse
            {
                Main = new Main { Temp = 12.5 },
                Wind = new Wind { Speed = 7.0 },
                Weather = new[] { new Weather { Main = "Rain" } }
            };
            _weatherRepositoryMock
                .Setup(r => r.GetWeatherAsync(latitude, longitude))
                .ReturnsAsync(weatherApiResponse);
            _recommendationServiceMock
                .Setup(r => r.GetRecommendation(12.5, "Rainy"))
                .Returns("Don't forget to bring a coat");

            // Act
            var result = await _weatherService.GetWeatherForecastAsync(latitude, longitude);

            // Assert
            result.Should().NotBeNull();
            result.TemperatureCelsius.Should().Be(12.5);
            result.WindSpeedKph.Should().BeApproximately(7.0 * 3.6, 0.01);
            result.Condition.Should().Be("Rainy");
            result.Recommendation.Should().Be("Don't forget to bring a coat");
        }

        [Theory]
        [InlineData("Clear", "Sunny")]
        [InlineData("Rain", "Rainy")]
        [InlineData("Snow", "Snowing")]
        [InlineData("Clouds", "Windy")]
        public async Task GetWeatherForecastAsync_WeatherCondition_MapsCorrectly(string apiCondition, string expectedCondition)
        {
            // Arrange
            var latitude = -41.2865;
            var longitude = 174.7762;
            var weatherApiResponse = new WeatherApiResponse
            {
                Main = new Main { Temp = 12.5 },
                Wind = new Wind { Speed = 7.0 },
                Weather = new[] { new Weather { Main = apiCondition } }
            };
            _weatherRepositoryMock
                .Setup(r => r.GetWeatherAsync(latitude, longitude))
                .ReturnsAsync(weatherApiResponse);
            _recommendationServiceMock
                .Setup(r => r.GetRecommendation(It.IsAny<double>(), It.IsAny<string>()))
                .Returns("Test recommendation");

            // Act
            var result = await _weatherService.GetWeatherForecastAsync(latitude, longitude);

            // Assert
            result.Condition.Should().Be(expectedCondition);
        }
    }
}