using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class Enrollment
    {
        [PrimaryKey, AutoIncrement]
        public int EnrollmentId { get; set; }

        [NotNull]
        public int FiliereId { get; set; }

        [NotNull]
        public int CourseId { get; set; }
    }

}