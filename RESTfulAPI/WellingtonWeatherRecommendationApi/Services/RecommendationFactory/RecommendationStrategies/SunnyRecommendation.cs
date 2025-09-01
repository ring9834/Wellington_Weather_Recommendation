namespace WellingtonWeatherRecommendationApi.Services
{
    public class Sunny : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            string message = "Don't forget to bring a hat. ";
            if (temperature > 25)
                message += "It’s a great day for a swim. ";
            return message;
        }
    }
}
