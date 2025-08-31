using FluentAssertions;
using WellingtonWeatherRecommendationApi.Services;
using Xunit;

namespace WellingtonWeatherApi.Tests.Services
{
    public class RecommendationServiceTests
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationServiceTests()
        {
            _recommendationService = new RecommendationService();
        }

        [Theory]
        [InlineData(26.0, "Sunny", "It’s a great day for a swim")]
        [InlineData(20.0, "Sunny", "Don't forget to bring a hat")]
        [InlineData(10.0, "Rainy", "Don't forget to bring a coat")]
        [InlineData(20.0, "Rainy", "Don’t forget the umbrella")]
        [InlineData(10.0, "Snowing", "Don't forget to bring a coat")]
        [InlineData(20.0, "Snowing", "Dress warmly for the snow")]
        [InlineData(15.0, "Windy", "Wear a windbreaker")]
        [InlineData(15.0, "Unknown", "Dress appropriately for the weather")]
        public void GetRecommendation_ReturnsCorrectRecommendation(double temperature, string condition, string expectedRecommendation)
        {
            // Act
            var result = _recommendationService.GetRecommendation(temperature, condition);

            // Assert
            result.Should().Be(expectedRecommendation);
        }
    }
}