using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IWeatherViewModel
    {
        Task LoadAsync(int id);
    }
}