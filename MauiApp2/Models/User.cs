using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [Unique, NotNull]
        public string Username { get; set; }

        [NotNull]
        public string Password { get; set; }
    }
}
