using MauiApp2.Data;

namespace MauiApp2;

public partial class HomePage : ContentPage
{
    private readonly Database _database;

    public HomePage(Database database)
    {
        InitializeComponent();
        _database = database;

        // Vérifiez si l'utilisateur est connecté
        if (!SessionManager.IsLoggedIn)
        {
            RedirectToLoginPage();
        }
        else
        {
            SetWelcomeMessage(SessionManager.Username);
        }
    }

    private void SetWelcomeMessage(string userName)
    {
        var currentHour = DateTime.Now.Hour;
        string greeting;

        if (currentHour < 12)
            greeting = "Bonjour";
        else if (currentHour < 18)
            greeting = "Bon après-midi";
        else
            greeting = "Bonsoir";

        WelcomeLabel.Text = $"{greeting}, {userName}!";
    }

    private async void RedirectToLoginPage()
    {
        await DisplayAlert("Session expirée", "Veuillez vous connecter pour continuer.", "OK");
        Application.Current.MainPage = new NavigationPage(new LoginPage(_database));
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame)
        {
            await AnimateFrame(frame);
        }
    }

    private async Task AnimateFrame(Frame frame)
    {
        var originalColor = frame.BackgroundColor;
        frame.BackgroundColor = Color.FromArgb("#E0E0E0"); // Gris clair temporaire

        await frame.ScaleTo(0.9, 100, Easing.CubicIn); // Réduction
        await frame.ScaleTo(1, 100, Easing.CubicOut);  // Retour à la taille normale

        frame.BackgroundColor = originalColor; // Restaurer la couleur
    }

    // Méthode pour rediriger vers la page "Add Student"
    private async void NavigateToAddStudentPage(object sender, EventArgs e)
    {
        if (SessionManager.IsLoggedIn)
        {
            await Navigation.PushAsync(new AddStudentPage(_database));
        }
        else
        {
            RedirectToLoginPage();
        }
    }

    // Méthode pour rediriger vers la page "Add Course"
    private async void NavigateToAddCoursePage(object sender, EventArgs e)
    {
        if (SessionManager.IsLoggedIn)
        {
            await Navigation.PushAsync(new AddCoursePage(_database));
        }
        else
        {
            RedirectToLoginPage();
        }
    }

    // Méthode pour rediriger vers la page "Absence"
    private async void NavigateToAbsencePage(object sender, EventArgs e)
    {
        if (SessionManager.IsLoggedIn)
        {
            await Navigation.PushAsync(new AbsencePage(_database));
        }
        else
        {
            RedirectToLoginPage();
        }
    }

    // Méthode pour rediriger vers la page "Search"
    private async void NavigateToSearchPage(object sender, EventArgs e)
    {
        if (SessionManager.IsLoggedIn)
        {
            await Navigation.PushAsync(new SearchPage(_database));
        }
        else
        {
            RedirectToLoginPage();
        }
    }

    // Méthode pour déconnexion
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
        if (confirm)
        {
            SessionManager.ClearSession();
            Application.Current.MainPage = new NavigationPage(new LoginPage(_database));
        }
    }
}
