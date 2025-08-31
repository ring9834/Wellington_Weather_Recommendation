namespace WellingtonWeatherRecommendationApi.Services
{
    public class SunnyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature > 25 ? "It’s a great day for a swim" : "Don't forget to bring a hat";
        }
    }

    public class RainyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature < 15 ? "Don't forget to bring a coat" : "Don’t forget the umbrella";
        }
    }

    public class SnowingRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return temperature < 15 ? "Don't forget to bring a coat" : "Dress warmly for the snow";
        }
    }

    public class WindyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            return "Wear a windbreaker";
        }
    }
}
