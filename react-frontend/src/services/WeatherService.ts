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
        if (error instanceof AxiosError) {
            if (error.response?.status === 429) {
                const retryAfter = error.response.headers['retry-after'] || 'a few minutes';
                throw new Error(`Too many requests. Please try again after ${retryAfter} seconds.`);
            }
            throw new Error(`Failed to fetch weather data: ${error.message}`);
        }
        throw new Error('An unexpected error occurred');
    }
  }
}