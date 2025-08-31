import axios, { AxiosError } from 'axios';
import { WeatherResponse } from '../models/WeatherResponse';

export class WeatherService {
  private static readonly API_URL = 'https://localhost:5001/weather';

  static async getWeather(latitude: number, longitude: number): Promise<WeatherResponse> {
    try {
      const response = await axios.get<WeatherResponse>(this.API_URL, {
        params: { latitude, longitude },
      });
      console.log('Weather data fetched:', response.data);
      return response.data;
    } catch (error) {
      throw new Error(
        error instanceof AxiosError
          ? `Failed to fetch weather data: ${error.message}`
          : 'An unexpected error occurred'
      );
    }
  }
}