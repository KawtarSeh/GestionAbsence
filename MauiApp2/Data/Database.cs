using SQLite;
using MauiApp2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Data
{
    public  class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<Student>().Wait();
            _database.CreateTableAsync<Course>().Wait();
            _database.CreateTableAsync<Filiere>().Wait();
            _database.CreateTableAsync<Enrollment>().Wait();
            _database.CreateTableAsync<Absence>().Wait();
        }

        // Méthodes pour User
        public Task<int> SaveUserAsync(User user) => _database.InsertAsync(user);
        public Task<List<User>> GetUsersAsync() => _database.Table<User>().ToListAsync();

        // Méthodes pour Student
        public Task<int> SaveStudentAsync(Student student) => _database.InsertAsync(student);
        public Task<List<Student>> GetStudentsAsync() => _database.Table<Student>().ToListAsync();
        //Méthode pour vérifier l'existence d'un étudiant par CIN
        public async Task<string> AddStudentAsync(Student student)
        {
            if (await StudentExistsAsync(student.Cin))
            {
                return "Un étudiant avec ce CIN existe déjà.";
            }

            await SaveStudentAsync(student);
            return "Étudiant ajouté avec succès.";
        }


        // Vérifier l'existence d'un étudiant par CIN
        public async Task<bool> StudentExistsAsync(string cin)
        {
            var student = await _database.Table<Student>().Where(s => s.Cin == cin).FirstOrDefaultAsync();
            return student != null;
        }



        // Méthodes pour Course
        public Task<int> SaveCourseAsync(Course course) => _database.InsertAsync(course);
        public Task<List<Course>> GetCoursesAsync() => _database.Table<Course>().ToListAsync();
        //Méthode pour vérifier l'existence d'un cours par nom
        public async Task<bool> CourseExistsAsync(string courseName)
        {
            var course = await _database.Table<Course>().Where(c => c.CourseName == courseName).FirstOrDefaultAsync();
            return course != null;
        }
        //Une méthode pour récupérer les cours par professeur
        public Task<List<Course>> GetCoursesByProfessorAsync(int professorId)
        {
            return _database.Table<Course>().Where(c => c.IdProf == professorId).ToListAsync();
        }


        // Méthodes pour Filiere
        public Task<int> SaveFiliereAsync(Filiere filiere) => _database.InsertAsync(filiere);
        public Task<List<Filiere>> GetFilieresAsync() => _database.Table<Filiere>().ToListAsync();

        // Méthodes pour Enrollment
        public Task<int> SaveEnrollmentAsync(Enrollment enrollment) => _database.InsertAsync(enrollment);
        public Task<List<Enrollment>> GetEnrollmentsAsync() => _database.Table<Enrollment>().ToListAsync();
        // Ajouter un enregistrement dans la table Enrollment
        public async Task<string> AddEnrollmentAsync(Enrollment enrollment)
        {
            var existing = await _database.Table<Enrollment>()
                .Where(e => e.FiliereId == enrollment.FiliereId && e.CourseId == enrollment.CourseId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return "Cette association entre la filière et le cours existe déjà.";
            }

            await _database.InsertAsync(enrollment);
            return "Enregistrement ajouté avec succès.";
        }

        // Méthodes pour Absence
        public Task<int> SaveAbsenceAsync(Absence absence) => _database.InsertAsync(absence);
        public Task<List<Absence>> GetAbsencesAsync() => _database.Table<Absence>().ToListAsync();
        public async Task PrintAllStudentsAsync()
        {
            var students = await _database.Table<Student>().ToListAsync();
            System.Diagnostics.Debug.WriteLine("Les étudiants ==========================================================");
            foreach (var student in students)
            {
                System.Diagnostics.Debug.WriteLine($"Student: {student.FirstName} {student.LastName}, Filiere: {student.IdFiliere}");
            }
        }

        public async Task PrintAllAbsencesAsync()
        {
            var absences = await _database.Table<Absence>().ToListAsync();
            System.Diagnostics.Debug.WriteLine("Les absences ==========================================================");
            foreach (var ab in absences)
            {
                System.Diagnostics.Debug.WriteLine($"Student ID : {ab.StudentId}, Cours ID: {ab.CourseId}, Status: {ab.Status}");
            }
        }


        public async Task<int> UpdateAbsenceAsync(Absence absence)
        {
            try
            {
                // la table Absence existe
                await _database.CreateTableAsync<Absence>();

                // Mettez à jour l'absence dans la base de données
                return await _database.UpdateAsync(absence);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la mise à jour de l'absence : {ex.Message}");
                throw; // Re-throw the exception if needed
            }
        }


        // Méthodes pour insérer des exemples de données
        public async Task InsertSampleDataAsync()
        {
         
            // Vérifier si les filières existent déjà
            var existingFilieres = await _database.Table<Filiere>().ToListAsync();
            if (!existingFilieres.Any())
            {
                var filieres = new List<Filiere>
                {
                    new Filiere { FiliereName = "Informatique" },
                    new Filiere { FiliereName = "Mathématiques" },
                    new Filiere { FiliereName = "Physique" }
                };
                await _database.InsertAllAsync(filieres);
            }

            
            
        }
        //nohaila
        public async Task<List<Student>> GetStudentsByFiliereAsync(string filiereName)
        {
            try
            {
                // Création des tables si elles n'existent pas déjà
                await _database.CreateTableAsync<Student>();
                await _database.CreateTableAsync<Filiere>();

                // Requête pour récupérer les étudiants en fonction de la filière
                var query = @"
                    SELECT s.*
                    FROM Student s
                    INNER JOIN Filiere f ON s.IdFiliere = f.FiliereId
                    WHERE f.FiliereName = ?";

                // Exécution de la requête
                var students = await _database.QueryAsync<Student>(query, filiereName);
                return students;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des étudiants par filière : {ex.Message}");
                throw;
            }
        }


        public async Task<int> GetCourseIdByNameAsync(string courseName)
        {
            var course = await _database.Table<Course>()
                                        .FirstOrDefaultAsync(c => c.CourseName == courseName);
            return course?.CourseId ?? 0;
        }

        public async Task<int> SaveAllAbsencesAsync(List<Absence> absences)
        {
            try
            {
                await _database.CreateTableAsync<Absence>();

                return await _database.InsertAllAsync(absences);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'enregistrement des absences : {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetFilieresByCourseIdAsync(int courseId)
        {
            try
            {
                await _database.CreateTableAsync<Filiere>();
                await _database.CreateTableAsync<Enrollment>();

                // Requête pour récupérer les noms des filières basées sur le CourseId
                var query = @"
                    SELECT DISTINCT f.FiliereName
                    FROM Filiere f
                    INNER JOIN Enrollment e ON f.FiliereId = e.FiliereId
                    WHERE e.CourseId = ?";

                var filiereNames = await _database.QueryAsync<Filiere>(query, courseId);

                // Retourner uniquement les noms des filières
                return filiereNames.Select(f => f.FiliereName).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération des filières : {ex.Message}");
                throw;
            }
        }


        //sabrine
        public async Task<string> RegisterProfessorAsync(string username, string password)
        {
            try
            {
                // Vérifiez si un utilisateur avec ce nom d'utilisateur existe déjà
                var existingUser = await _database.Table<User>()
                                                  .Where(u => u.Username == username)
                                                  .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return "Un utilisateur avec ce nom existe déjà.";
                }

                // Créez et enregistrez le nouvel utilisateur
                var professor = new User
                {
                    Username = username,
                    Password = password
                };

                await SaveUserAsync(professor);

                return "Professeur enregistré avec succès.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'enregistrement du professeur : {ex.Message}");
                return "Une erreur est survenue lors de l'enregistrement.";
            }
        }

        public async Task<User> GetProfessorAsync(string username, string password)
        {
            try
            {
                // Recherche de l'utilisateur avec le nom d'utilisateur et le mot de passe
                var professor = await _database.Table<User>()
                                                .Where(p => p.Username == username && p.Password == password)
                                                .FirstOrDefaultAsync();

                return professor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération du professeur : {ex.Message}");
                throw; // Relance l'exception si nécessaire
            }
        }









    }
}


    
