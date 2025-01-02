using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using MauiApp2.Data;

namespace MauiApp2
{
    public partial class LoadingPage : ContentPage
    {
        private readonly Database _database;
        public LoadingPage(Database database)
        {
            InitializeComponent();
            _database = database;
            LoadLoginPageAsync();
        }

        private async void LoadLoginPageAsync()
        {

            await Task.Delay(10000);
            await Navigation.PushAsync(new LoginPage(_database));
        }
    }
}