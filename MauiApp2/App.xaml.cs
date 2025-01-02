using MauiApp2.Data;

namespace MauiApp2
{
    public partial class App : Application
    {
          public App(Database database)
        {
    
            InitializeComponent();

            MainPage = new NavigationPage(new LoadingPage(database));
        }

        
    }
}