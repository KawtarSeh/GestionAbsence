using MauiApp2.Models;
using MauiApp2.Data;
using SQLite;

namespace MauiApp2;
public partial class SignUpPage : ContentPage
{
    private readonly Database _database;

    public SignUpPage(Database database)
    {
        InitializeComponent();
        _database = database;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Échec de l'inscription", "Veuillez entrer un nom d'utilisateur et un mot de passe.", "OK");
            return;
        }

        try
        {
            // Appel asynchrone pour enregistrer le professeur
            var result = await _database.RegisterProfessorAsync(username, password);

            if (result == "Un utilisateur avec ce nom existe déjà.")
            {
                await DisplayAlert("Échec de l'inscription", result, "OK");
            }
            else
            {
                await DisplayAlert("Inscription réussie", "Votre compte a été créé.", "OK");
                await Navigation.PushAsync(new LoginPage(_database));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors de l'inscription : {ex.Message}");
            await DisplayAlert("Erreur", "Une erreur est survenue lors de l'inscription. Veuillez réessayer.", "OK");
        }
    }
}
