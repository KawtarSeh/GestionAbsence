using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class Filiere
    {
        [PrimaryKey, AutoIncrement]
        public int FiliereId { get; set; }

        [NotNull]
        public string FiliereName { get; set; }
    }
}
