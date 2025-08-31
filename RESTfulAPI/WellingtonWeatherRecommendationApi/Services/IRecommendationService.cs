namespace WellingtonWeatherRecommendationApi.Services
{
    public interface IRecommendationService
    {
        string GetRecommendation(double temperature, string condition);
    }
}
