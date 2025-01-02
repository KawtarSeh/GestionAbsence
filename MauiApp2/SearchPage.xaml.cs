using MauiApp2.Data;
using MauiApp2.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class SearchPage : ContentPage
    {
        private readonly Database database;
        

        public SearchPage(Database _database)
        {
            InitializeComponent();
            database = _database;

            // Vérification que l'utilisateur est connecté
            if (!SessionManager.IsLoggedIn)
            {
                RedirectToLoginPage();
            }
            else
            {
                LoadPageData();
            }

        }

        private async void LoadPageData()
        {
            InsertAndPrintSampleData();
        }

        private async void InsertAndPrintSampleData()
        {
            //await database.InsertSampleDataAsync();
            await database.PrintAllStudentsAsync();
            await database.PrintAllAbsencesAsync();
            await LoadCoursesPickerData();  // Charger les données des cours dans le Picker
        }

        private async Task LoadCoursesPickerData()
        {
            try
            {
                var courses = await database.GetCoursesAsync();
                var filteredCourses = courses
                    .Where(c => c.IdProf == SessionManager.UserId)
                    .Select(c => c.CourseName)
                    .ToList();

                coursesPicker.ItemsSource = filteredCourses;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors du chargement des cours : {ex.Message}", "OK");
            }
        }

        private async void OnCourseSelected(object sender, EventArgs e)
        {
            await LoadFilieresPickerData();
            await LoadStudentsPicker();
        }

        private async void OnFiliereSelected(object sender, EventArgs e)
        {
            await LoadStudentsPicker();
        }

        private async Task LoadFilieresPickerData()
        {
            string selectedCourse = coursesPicker.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var courses = await database.GetCoursesAsync();
                var enrollments = await database.GetEnrollmentsAsync();
                var students = await database.GetStudentsAsync();
                var selectedCourseId = courses.FirstOrDefault(c => c.CourseName == selectedCourse)?.CourseId;

                if (selectedCourseId.HasValue)
                {
                    var filiereIds = enrollments.Where(e => e.CourseId == selectedCourseId.Value)
                                                .Select(e => students.FirstOrDefault(s => s.StudentId == e.FiliereId)?.IdFiliere)
                                                .Distinct()
                                                .ToList();

                    var filieres = await database.GetFilieresAsync();
                    var filteredFilieres = filieres.Where(f => filiereIds.Contains(f.FiliereId))
                                                   .Select(f => f.FiliereName)
                                                   .ToList();

                    filieresPicker.ItemsSource = filteredFilieres;
                }
            }
        }

        private async Task LoadStudentsPicker()
        {
            string selectedCourse = coursesPicker.SelectedItem as string;
            string selectedFiliere = filieresPicker.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedCourse) && !string.IsNullOrEmpty(selectedFiliere))
            {
                var filiere = (await database.GetFilieresAsync()).FirstOrDefault(f => f.FiliereName == selectedFiliere);
                if (filiere != null)
                {
                    var students = await database.GetStudentsAsync();
                    var courses = await database.GetCoursesAsync();
                    var course = courses.FirstOrDefault(c => c.CourseName == selectedCourse);

                    if (course != null)
                    {
                        var enrollments = await database.GetEnrollmentsAsync();

                        var filteredStudents = students
                            .Where(s => s.IdFiliere == filiere.FiliereId &&
                                        enrollments.Any(e => e.FiliereId == s.StudentId && e.CourseId == course.CourseId))
                            .Select(s => $"{s.FirstName} {s.LastName}")
                            .ToList();

                        studentsPicker.ItemsSource = filteredStudents;
                    }
                }
            }
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string fullName = studentsPicker.SelectedItem as string;
            string selectedCourse = coursesPicker.SelectedItem as string;
            string selectedFiliere = filieresPicker.SelectedItem as string;

            if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(selectedCourse) && !string.IsNullOrEmpty(selectedFiliere))
            {
                var filiere = (await database.GetFilieresAsync()).FirstOrDefault(f => f.FiliereName == selectedFiliere);
                if (filiere != null)
                {
                    var results = await SearchStudent(fullName, filiere.FiliereId, selectedCourse);
                    studentInfoLabel.Text = results.Item1;
                    DisplayDetails(results.Item2);

                    // Update the counts for present and absent
                    presentCountLabel.Text = results.Item2.Count(a => a.Status == "Present").ToString();
                    absentCountLabel.Text = results.Item2.Count(a => a.Status == "Absent").ToString();

                    // Show the details section
                    detailsSection.IsVisible = true;
                }
                else
                {
                    resultLabel.Text = "Filière non trouvée.";
                    detailsLayout.Children.Clear();
                }
            }
            else
            {
                resultLabel.Text = "Veuillez remplir tous les champs.";
                detailsLayout.Children.Clear();
            }
        }

        private async Task<(string, List<Absence>)> SearchStudent(string fullName, int filiereId, string courseName)
        {
            var nameParts = fullName.Split(' ');
            if (nameParts.Length < 2)
                return ("Nom complet invalide.", new List<Absence>());

            string firstName = nameParts[0];
            string lastName = nameParts[1];

            var students = await database.GetStudentsAsync();
            var courses = await database.GetCoursesAsync();
            var absences = await database.GetAbsencesAsync();

            var student = students.FirstOrDefault(s => s.FirstName == firstName && s.LastName == lastName && s.IdFiliere == filiereId);

            if (student == null)
                return ("Étudiant non trouvé.", new List<Absence>());

            var course = courses.FirstOrDefault(c => c.CourseName == courseName);
            if (course == null)
                return ("Cours non trouvé.", new List<Absence>());

            var studentAbsences = absences.Where(a => a.StudentId == student.StudentId && a.CourseId == course.CourseId).ToList();
            int presentCount = studentAbsences.Count(a => a.Status == "Present");
            int absentCount = studentAbsences.Count(a => a.Status == "Absent");


            string result = $"Informations de l'étudiant\n";
            result += $"Nom : {student.FirstName} {student.LastName}\n";
            result += $"Email : {student.Email}\n";
            result += $"Téléphone : {student.PhoneNumber}\n";

            return (result, studentAbsences);
        }

        private void DisplayDetails(List<Absence> absences)
        {
            detailsLayout.Children.Clear();

            foreach (var absence in absences)
            {
                // Créer un label pour afficher la date d'absence
                detailsLayout.Children.Add(new Label
                {
                    Text = $"Date : {absence.Date.ToShortDateString()}",
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(9)
                });

                // Créer un Picker pour le statut d'absence
                var statusPicker = new Picker
                {
                    Title = "Sélectionner le statut",
                    ItemsSource = new List<string> { "Présent", "Absent" },
                    SelectedItem = absence.Status == "Present" ? "Présent" : "Absent"
                };
                detailsLayout.Children.Add(statusPicker);

                // Créer un bouton pour modifier le statut
                var modifyButton = new Button
                {
                    Text = "Modifier le statut"
                };
                modifyButton.Clicked += async (sender, e) =>
                {
                    var selectedStatus = statusPicker.SelectedItem as string;

                    if (selectedStatus != null)
                    {
                        // Récupérer le statut actuel
                        var currentStatus = absence.Status == "Present" ? "Présent" : "Absent";

                        // Construire le message de confirmation
                        var message = $"Êtes-vous sûr de vouloir modifier le statut d'absence de {currentStatus} à {selectedStatus}?";

                        // Confirmer la modification
                        bool confirm = await DisplayAlert("Confirmation", message, "Oui", "Non");

                        if (confirm)
                        {
                            // Mettre à jour le statut
                            absence.Status = selectedStatus == "Présent" ? "Present" : "Absent";
                            await database.UpdateAbsenceAsync(absence); // Mettre à jour la base de données

                            await DisplayAlert("Succès", "Le statut a été modifié.", "OK");

                            // Récupérer l'étudiant et le cours
                            var student = (await database.GetStudentsAsync()).FirstOrDefault(s => s.StudentId == absence.StudentId);
                            var course = (await database.GetCoursesAsync()).FirstOrDefault(c => c.CourseId == absence.CourseId);

                            if (student != null && course != null)
                            {
                                var courseName = course.CourseName;
                                var filiereId = student.IdFiliere; // Utiliser l'ID de la filière de l'étudiant

                                // Recharger les absences après modification
                                var updatedAbsences = await database.GetAbsencesAsync(); // Récupérer toutes les absences
                                var studentAbsences = updatedAbsences.Where(a => a.StudentId == absence.StudentId && a.CourseId == absence.CourseId).ToList();

                                // Mettre à jour l'affichage avec les absences mises à jour

                                OnSearchClicked(sender, e);
                            }
                        }
                    }
                };

                detailsLayout.Children.Add(modifyButton);
            }
        }

        private void OnStatusSelected(object sender, EventArgs e)
        {
            // Logique à ajouter si nécessaire
        }

        private async void RedirectToLoginPage()
        {
            await DisplayAlert("Session expirée", "Veuillez vous reconnecter.", "OK");
            Application.Current.MainPage = new NavigationPage(new LoginPage(database));
        }
    }
}