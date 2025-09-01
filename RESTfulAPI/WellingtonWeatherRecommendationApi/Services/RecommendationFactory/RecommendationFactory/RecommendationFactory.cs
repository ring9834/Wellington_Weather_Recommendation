using System.Reflection;

namespace WellingtonWeatherRecommendationApi.Services
{
    public class RecommendationFactory: IRecommendationFactory
    {
        private readonly Dictionary<string, IRecommendationStrategy> _strategies = new();

        public RecommendationFactory()
        {
            //_strategies = new Dictionary<string, IRecommendationStrategy>
            //{
            //    { "Sunny", new SunnyRecommendation() },
            //    { "Rainy", new RainyRecommendation() },
            //    { "Snowing", new SnowingRecommendation() },
            //    { "Windy", new WindyRecommendation() }
            //};
            LoadAvailableRecommendationStrategyTypes();
        }

        public string GetRecommendation(double temperature, string condition)
        {
            if (_strategies.TryGetValue(condition, out var strategy))
            {
                return strategy.GetRecommendation(temperature);
            }
            return "Dress appropriately for the weather";
        }

        // Using reflection to load available recommendation strategies
        private void LoadAvailableRecommendationStrategyTypes()
        {
            Type[] assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in assemblyTypes)
            {
                if (type.GetInterface(typeof(IRecommendationStrategy).ToString()) != null)
                {
                    _strategies.Add(type.Name, (IRecommendationStrategy)type);
                }
            }
        }
    }
}
