# Enhancements

## Enhancements for API
 - Add caching using Redis for OpenWeatherMap API responses to reduce API calls and improve performance.
 - Implement error handling for invalid latitude/longitude inputs.
 - Add support for historical weather data using OpenWeatherMap's One Call API.

## Enhancements for Testing
 - Add integration tests to verify the full API pipeline with a real or mocked OpenWeatherMap response.
 - Test edge cases for latitude/longitude. For example, values outside valid ranges: [-90, 90] for latitude, [-180, 180] for longitude.
 - Add performance tests to ensure the API responds within acceptable time limits.
 - Mock the HttpClientFactory for more realistic HTTP client testing in WeatherRepositoryTests.
