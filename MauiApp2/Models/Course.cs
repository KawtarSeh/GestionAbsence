using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }

        [NotNull]
        public string CourseName { get; set; }

        [NotNull]
        public int IdProf { get; set; }
    }
}
