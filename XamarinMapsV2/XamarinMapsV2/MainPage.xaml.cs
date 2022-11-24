using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace XamarinMapsV2
{
    public partial class MainPage : ContentPage
    {
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

                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);

                    if (results.ContainsKey(Permission.LocationWhenInUse))
                    {
                        status = results[Permission.LocationWhenInUse];
                    }
                }

                if (status == PermissionStatus.Granted)
                    localMap.IsShowingUser = true;
                else
                    await DisplayAlert("Местоположение отказано", "Невозможно показать ваше местоположение на карте",
                        "Ок");
            }
            catch (Exception ex) { }
            
        }
    }
}
