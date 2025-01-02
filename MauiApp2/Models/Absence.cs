using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class Absence
    {
        [PrimaryKey, AutoIncrement]
        public int AbsenceId { get; set; }

        [NotNull] // Pas besoin de ForeignKey ici pour SQLite
        public int StudentId { get; set; }

        [NotNull] 
        public int CourseId { get; set; }

        [NotNull]
        public DateTime Date { get; set; }

        [NotNull]
        public string Status { get; set; }
    }
}
