using MauiApp2.Data;
using MauiApp2.Models;
using SQLite;

namespace MauiApp2;

public partial class LoginPage : ContentPage
{
    private readonly Database _database;

    public LoginPage(Database database)
    {
        InitializeComponent();
        _database = database;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Échec de connexion", "Veuillez entrer un nom d'utilisateur et un mot de passe.", "OK");
            return;
        }

        try
        {
            // Appel asynchrone pour récupérer le professeur
            var professor = await _database.GetProfessorAsync(username, password);

            if (professor != null)
            {
                // Stockage des informations utilisateur dans la session
                SessionManager.UserId = professor.UserId;
                SessionManager.Username = professor.Username;

                await DisplayAlert("Connexion réussie", $"Bienvenue {professor.Username}!", "OK");
                await Navigation.PushAsync(new HomePage(_database));
            }
            else
            {
                await DisplayAlert("Échec de connexion", "Nom d'utilisateur ou mot de passe incorrect.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de la tentative de connexion : {ex.Message}");
            await DisplayAlert("Erreur", "Une erreur s'est produite lors de la connexion. Veuillez réessayer.", "OK");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage(_database));
    }
}
