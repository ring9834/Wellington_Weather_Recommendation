import React from 'react';
import { WeatherResponse } from '../models/WeatherResponse';

interface WeatherDisplayProps {
  weather: WeatherResponse | null;
  error: string | null;
}

const WeatherDisplay: React.FC<WeatherDisplayProps> = ({ weather, error }) => {
  if (error) {
    return (
      <div className="alert alert-danger" role="alert">
        {error}
      </div>
    );
  }

  if (!weather) {
    return null;
  }

  // Uses toFixed(1) to format numbers to one decimal place for readability.
  return (
    <div className="card">
      <div className="card-body">
        <h5 className="card-title">Wellington Weather</h5>
        <p className="card-text">
          <strong>Temperature:</strong> {weather.temperatureCelsius.toFixed(1)}°C
        </p>
        <p className="card-text">
          <strong>Wind Speed:</strong> {weather.windSpeedKph.toFixed(1)} km/h
        </p>
        <p className="card-text">
          <strong>Condition:</strong> {weather.condition}
        </p>
        <p className="card-text">
          <strong>Recommendation:</strong> {weather.recommendation}
        </p>
      </div>
    </div>
  );
};

export default WeatherDisplay;