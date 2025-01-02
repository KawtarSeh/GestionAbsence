using MauiApp2.Data;
using MauiApp2.Models;
using System.Linq;

namespace MauiApp2;

public partial class AddCoursePage : ContentPage
{
    private readonly Database _database;
    private readonly int _professorId; // ID du professeur connecté

    public AddCoursePage(Database database)
    {
        InitializeComponent();
        _database = database;

        // Utiliser l'ID du professeur connecté depuis la session
        _professorId = SessionManager.UserId ?? 0;

        if (_professorId == 0)
        {
            DisplayAlert("Erreur", "Professeur non connecté.", "OK");
            RedirectToLoginPage();
        }
        else
        {
            InitializePage();
        }
    }

    private async void InitializePage()
    {
        // Charger les filières
        await InsertSampleDataIfNeeded();
        await LoadFilieres();
    }

    private async Task InsertSampleDataIfNeeded()
    {
        try
        {
            await _database.InsertSampleDataAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de l'insertion des données initiales : {ex.Message}", "OK");
        }
    }

    private async Task LoadFilieres()
    {
        try
        {
            var filieres = await _database.GetFilieresAsync();
            FilieresCollectionView.ItemsSource = filieres;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors du chargement des filières : {ex.Message}", "OK");
        }
    }

    private async void OnAddCourseClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await AnimateButton(button);
        }

        if (string.IsNullOrWhiteSpace(CourseNameEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez saisir un nom pour le cours.", "OK");
            return;
        }

        var selectedFilieres = FilieresCollectionView.SelectedItems.Cast<Filiere>().ToList();

        if (!selectedFilieres.Any())
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner au moins une filière.", "OK");
            return;
        }

        try
        {
            // Créer un nouveau cours
            var course = new Course
            {
                CourseName = CourseNameEntry.Text,
                IdProf = _professorId
            };

            // Ajouter le cours à la base de données
            await _database.SaveCourseAsync(course);

            // Associer les filières sélectionnées au cours
            foreach (var filiere in selectedFilieres)
            {
                var enrollment = new Enrollment
                {
                    FiliereId = filiere.FiliereId,
                    CourseId = course.CourseId
                };

                await _database.AddEnrollmentAsync(enrollment);
            }

            await DisplayAlert("Succès", "Cours et associations ajoutés avec succès.", "OK");

            // Réinitialiser les champs
            ResetForm();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
    }

    private async Task AnimateButton(Button button)
    {
        await button.ScaleTo(0.9, 100, Easing.CubicIn);
        await button.ScaleTo(1, 100, Easing.CubicOut);
    }

    private void ResetForm()
    {
        CourseNameEntry.Text = string.Empty;
        FilieresCollectionView.SelectedItems.Clear();
    }

    private async void RedirectToLoginPage()
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage(_database));
        await DisplayAlert("Session expirée", "Veuillez vous reconnecter.", "OK");
    }
}
