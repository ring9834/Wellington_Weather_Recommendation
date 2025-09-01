namespace WellingtonWeatherRecommendationApi.Services
{
    public class Windy : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return "Wear a windbreaker";
        }
    }
}
