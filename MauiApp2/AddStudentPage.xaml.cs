using MauiApp2.Data;
using MauiApp2.Models;
using MauiApp2;

namespace MauiApp2;

public partial class AddStudentPage : ContentPage
{
    private Database _database;

    public AddStudentPage(Database database)
    {
        InitializeComponent();
        // Utiliser le chemin et les options définis dans Constants
        _database = database;
        InsertFiliere();
        
    }

    private async void InsertFiliere()
    {
        await _database.InsertSampleDataAsync();
        LoadFilieres();
    }
    private async void LoadFilieres()
    {
        try
        {
            var filieres = await _database.GetFilieresAsync();
            if (filieres != null && filieres.Count > 0)
            {
                filieresPicker.ItemsSource = filieres;
                filieresPicker.ItemDisplayBinding = new Binding("FiliereName");
            }
            else
            {
                await DisplayAlert("Info", "Aucune filière disponible.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec du chargement des filières : {ex.Message}", "OK");
        }
    }

    

    private async void OnAddStudentClicked(object sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        try
        {
            var selectedFiliere = (Filiere)filieresPicker.SelectedItem;
            

            var student = new Student
            {
                Cin = CinEntry.Text,
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text,
                Email = EmailEntry.Text,
                PhoneNumber = PhoneNumberEntry.Text,
                IdFiliere = selectedFiliere.FiliereId
            };

            var result = await _database.AddStudentAsync(student);
            await DisplayAlert("Résultat", result, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(CinEntry.Text) ||
            string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PhoneNumberEntry.Text) ||
            filieresPicker.SelectedItem == null)
        {
            DisplayAlert("Erreur", "Veuillez remplir tous les champs et effectuer une sélection.", "OK");
            return false;
        }
        return true;
    }

    private void ClearFields()
    {
        CinEntry.Text = string.Empty;
        FirstNameEntry.Text = string.Empty;
        LastNameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PhoneNumberEntry.Text = string.Empty;
        filieresPicker.SelectedIndex = -1;
    }


}
