using System;
using System.Threading.Tasks;
using FriendOrganizer.UI.View.Services;
using Prism.Events;
using FriendOrganizer.UI.Api;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using System.Windows.Input;
using System.Linq;

namespace FriendOrganizer.UI.ViewModel
{
    public class WeatherViewModel : DetailViewModelBase, IWeatherViewModel
    {
        private IWeatherApiService _weatherApiService;
        private Weather _Weather;
        private ConsolidatedWeather _ConsolidatedWeather;
        public WeatherViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IWeatherApiService weatherApiService) : base(eventAggregator, messageDialogService)
        {
            _weatherApiService = weatherApiService;
            OnUpdateWeatherCommand = new DelegateCommand(OnUpdateExecute);
        }

        public async void OnUpdateExecute()
        {
            try
            {
            var weather = await _weatherApiService.UpdateWeatherAsync();

            Weather = weather;
            ConsolidatedWeather = Weather.consolidated_weather.First();
            }
            catch
            {
                await MessageDialogService.ShowInfoDialogAsync("Could not update the weather, check your internet connection");
            }
        }

        public ICommand OnUpdateWeatherCommand { get; }

        public Weather Weather
        {
            get { return _Weather; }
            private set
            {
                _Weather = value;
                OnPropertyChanged();
            }
            
        }

        public ConsolidatedWeather ConsolidatedWeather
        {
            get { return _ConsolidatedWeather; }
            private set
            {
                _ConsolidatedWeather = value;
                OnPropertyChanged();
            }
        }

        public async override Task LoadAsync(int id)
        {
            try
            {
            var weather = await _weatherApiService.LoadWeatherAsync();
            for (int i = 0; i < weather.consolidated_weather.Count; i++)
            {
                ConsolidatedWeather = weather.consolidated_weather[0];
            }
            Weather = weather;
            }
            catch
            {
                await MessageDialogService.ShowInfoDialogAsync("Could not load the weather, check your internet connection");
            }
            

        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
