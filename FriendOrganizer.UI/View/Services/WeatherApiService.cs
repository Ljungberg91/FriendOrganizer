using FriendOrganizer.UI.Api;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.View.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        public string ErrorMessage { get; set; }


        public HttpClient httpClient;

        public WeatherApiService()
        {
            httpClient = new HttpClient();
        }

        public async Task<Weather> LoadWeatherAsync()
        {
            httpClient.BaseAddress = new Uri("https://www.metaweather.com/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Weather weather = await GetWeatherAsync();

            return weather;
        }


        public async Task<Weather> GetWeatherAsync()
        {
            Weather weather = null;
            HttpResponseMessage response = await httpClient.GetAsync("location/44418");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    weather = await response.Content.ReadAsAsync<Weather>();
                }
                catch
                {
                    ErrorMessage = "This Api can't access that location, try something else";
                }
            }

            return weather;
        }

        public async Task<Weather> UpdateWeatherAsync()
        {
            var newWeather = await GetWeatherAsync();
            return newWeather;
        }

    }
}
