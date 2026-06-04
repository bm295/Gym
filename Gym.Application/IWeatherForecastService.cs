using System;
using System.Threading.Tasks;
using Gym.Domain;

namespace Gym.Application
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}
