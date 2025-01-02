using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int StudentId { get; set; }

        [Unique, NotNull]
        public string Cin { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [Unique]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [NotNull]
        public int IdFiliere { get; set; }
    }
}
