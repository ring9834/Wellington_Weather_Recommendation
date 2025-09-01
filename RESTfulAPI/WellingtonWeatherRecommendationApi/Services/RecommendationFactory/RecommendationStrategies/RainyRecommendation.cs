namespace WellingtonWeatherRecommendationApi.Services
{
    public class RainyRecommendation : IRecommendationStrategy
    {
        public string GetRecommendation(double temperature)
        {
            string message = "Don’t forget the umbrella. ";
            if (temperature < 15) 
                message += "Don't forget to bring a coat. ";
            return message;
        }
    }
}
