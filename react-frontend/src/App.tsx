import React, { useState, useEffect } from 'react';
import WeatherDisplay from './components/WeatherDisplay';
import { WeatherService } from './services/WeatherService';
import { WeatherResponse } from './models/WeatherResponse';

const WELLINGTON_LATITUDE = -41.2865;
const WELLINGTON_LONGITUDE = 174.7762;

const App: React.FC = () => {
  const [weather, setWeather] = useState<WeatherResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWeather = async () => {
      try {
        const data = await WeatherService.getWeather(WELLINGTON_LATITUDE, WELLINGTON_LONGITUDE);
        setWeather(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unexpected error occurred');
      }
    };

    fetchWeather();
  }, []);

  return (
    <div className="container mt-5">
      <h1>Wellington Weather Recommendation</h1>
      <WeatherDisplay weather={weather} error={error} />
      {/* Enhancements:
          - Add a form to allow users to input custom latitude and longitude.
          - Implement a loading spinner while fetching data.
          - Add a refresh button to fetch updated weather data.
      */}
    </div>
  );
};

export default App;