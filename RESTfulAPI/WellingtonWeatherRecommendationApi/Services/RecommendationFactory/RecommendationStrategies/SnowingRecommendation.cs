namespace WellingtonWeatherRecommendationApi.Services
{
    public class Snowing : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            string message = "Dress warmly for the snow. ";
            if (temperature < 15)
                message += "Don't forget to bring a coat. ";
            return message;
        }
    }
}
