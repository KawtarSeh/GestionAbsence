using MauiApp2.Models;
using System.Collections.ObjectModel;
using MauiApp2.Data;


namespace MauiApp2;

    public partial class AbsencePage : ContentPage
    {
        
        private readonly Database database;
        private string selectedCourse;
        private string selectedFiliere;
        private ObservableCollection<StudentViewModel> studentsCollection;


        public AbsencePage(Database _database)
        {
            InitializeComponent();
            //database = new Database(Constants.DatabasePath);
            database = _database;

            // V�rification que l'utilisateur est connect�
            if (!SessionManager.IsLoggedIn)
            {
                RedirectToLoginPage();
            }
            else
            {
                InitializePage();
            }
    }

        private void InitializePage()
        {
        studentsCollection = new ObservableCollection<StudentViewModel>();
        studentsCollectionView.ItemsSource = studentsCollection;

        // Charger les donn�es pour les Pickers
        InsertAndPrintSampleData();
        }


    private async void InsertAndPrintSampleData()
        {
            //await database.InsertSampleDataAsync();
            await database.PrintAllStudentsAsync();
            await database.PrintAllAbsencesAsync();
            LoadCoursesPickerData();

        }

        private async void LoadCoursesPickerData()
        {
            try
        {
            var courses = await database.GetCoursesAsync();
            var filteredCourses = courses
                .Where(c => c.IdProf == SessionManager.UserId)
                .Select(c => c.CourseName)
                .ToList();

            coursepicker.ItemsSource = filteredCourses;
        }
            catch (Exception ex)
            {
            await DisplayAlert("Erreur", $"Erreur lors du chargement des cours : {ex.Message}", "OK");
            }
        }

    private void OnCourseSelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null || picker.SelectedIndex == -1)
                return;

            selectedCourse = picker.Items[picker.SelectedIndex];

            // R�initialiser la s�lection de la fili�re
            filierepicker.SelectedIndex = -1;
            selectedFiliere = null;

            LoadFilieresPickerData();

            studentsCollection.Clear();
            InfoLabel.Text = "Veuillez s�lectionner une fili�re.";
            InfoLabel.IsVisible = true;
        }

        private async Task LoadFilieresPickerData()
        {
            int courseId = await database.GetCourseIdByNameAsync(selectedCourse);


            var filiereNames = await database.GetFilieresByCourseIdAsync(courseId);
            filierepicker.ItemsSource = filiereNames;
        }

        private void OnFiliereSelected(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null || picker.SelectedIndex == -1)
                return;

            selectedFiliere = picker.Items[picker.SelectedIndex];

            LoadStudentsByFiliere();
        }

        private async void LoadStudentsByFiliere()
        {
            try
            {
                var students = await database.GetStudentsByFiliereAsync(selectedFiliere);

                studentsCollection.Clear();

                if (students.Any())
                {
                    InfoLabel.IsVisible = false;
                    foreach (var student in students)
                    {
                        studentsCollection.Add(new StudentViewModel
                        {
                            StudentId = student.StudentId,
                            FullName = $"{student.FirstName} {student.LastName}",
                            IsChecked = false
                        });
                    }
                }
                else
                {
                    InfoLabel.Text = "Aucun �tudiant trouv� pour la fili�re s�lectionn�e.";
                    InfoLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des �tudiants : {ex.Message}");
                InfoLabel.Text = "Une erreur est survenue lors du chargement des �tudiants.";
                InfoLabel.IsVisible = true;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert("Confirmation", "�tes-vous s�r de vouloir enregistrer les absences ?", "Oui", "Non");

            if (isConfirmed)
            {
                try
                {
                    int courseId = await database.GetCourseIdByNameAsync(selectedCourse);

                    var absences = new List<Absence>();

                    foreach (var student in studentsCollection)
                    {
                        absences.Add(new Absence
                        {
                            StudentId = student.StudentId,
                            CourseId = courseId,
                            Date = DateTime.Now,
                            Status = student.IsChecked ? "Present" : "Absent"
                        });
                    }

                    await database.SaveAllAbsencesAsync(absences);

                    await database.PrintAllAbsencesAsync();

                    await DisplayAlert("Succ�s", "Les absences ont �t� enregistr�es avec succ�s.", "OK");

                    await Navigation.PushAsync(new HomePage(database));  //on va la remplacer par l'accueil

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement des absences : {ex.Message}");
                    await DisplayAlert("Erreur", "Une erreur s'est produite lors de l'enregistrement des absences.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Info", "L'enregistrement des absences a �t� annul�.", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert("Confirmation", "Are you sure you want to exit? All changes will be lost.", "Yes", "No");

            if (isConfirmed)
            {
                await Navigation.PushAsync(new HomePage(database));  //on va la remplacer par l'accueil
            }
        
        }

        private async void RedirectToLoginPage()
        {
            await DisplayAlert("Session expir�e", "Veuillez vous reconnecter.", "OK");
            Application.Current.MainPage = new NavigationPage(new LoginPage(database));
        }



}
