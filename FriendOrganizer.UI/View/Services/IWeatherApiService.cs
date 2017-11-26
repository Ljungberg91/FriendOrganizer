using System.Threading.Tasks;
using FriendOrganizer.UI.Api;

namespace FriendOrganizer.UI.View.Services
{
    public interface IWeatherApiService
    {
        Task<Weather> LoadWeatherAsync();
        Task<Weather> UpdateWeatherAsync();
    }
}