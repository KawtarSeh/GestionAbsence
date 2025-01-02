using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2
{
    public class Constants
    {
        // Nom du fichier de la base de données
        public const string DatabaseFilename = "GestionAbsence3.db3";

        // Chemin de la base de données
        public static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        // Options pour l'ouverture de la base de données
        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.Create |    // Crée la base si elle n'existe pas
            SQLiteOpenFlags.ReadWrite | // Lecture/écriture
            SQLiteOpenFlags.SharedCache; // Partage du cache pour optimiser les accès
    }

}
