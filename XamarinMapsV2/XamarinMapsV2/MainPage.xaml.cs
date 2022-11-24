using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Essentials;
using Plugin.Permissions;

namespace XamarinMapsV2
{
    public partial class MainPage : ContentPage
    {
        private bool hasLocationPermission = false;
        public MainPage()
        {
            InitializeComponent();
            GetPermission();
        }

        private async void GetPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);

                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationWhenInUse))
                {
                    await DisplayAlert("Разрешить местположение", "Вы должны разрешить показывать местопожоление", "Ок");
                }

                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);

                    if (results.ContainsKey(Permission.LocationWhenInUse))
                    {
                        status = results[Permission.LocationWhenInUse];
                    }
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    hasLocationPermission = true;
                    localMap.IsShowingUser = true;
                    GetLocation();
                }
                else
                    await DisplayAlert("Местоположение отказано", "Невозможно показать ваше местоположение на карте",
                        "Ок");
            }
            catch (Exception ex) { }
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (hasLocationPermission)
            {
                var locator = CrossGeolocator.Current;
                locator.PositionChanged += Locator_PositionChanged;

                await locator.StartListeningAsync(TimeSpan.Zero, 100);
            }
            

            GetLocation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossGeolocator.Current.StopListeningAsync();
            CrossGeolocator.Current.PositionChanged -= Locator_PositionChanged;
        }

        private async void GetLocation()
        {
            if (hasLocationPermission)
            {
                var locator = CrossGeolocator.Current;
                var position = await locator.GetPositionAsync();

                MoveMap(position);
            }
        }

        private void MoveMap(Position position)
        {
            var centerMap = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            localMap.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(centerMap, 2, 2));
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            MoveMap(e.Position);
        }
    }
}
