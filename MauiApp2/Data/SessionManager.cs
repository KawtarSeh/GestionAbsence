using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Data
{
    public static class SessionManager
    {
        public static int? UserId { get; set; }
        public static string Username { get; set; }
        public static bool IsLoggedIn => UserId.HasValue;

        public static void ClearSession()
        {
            UserId = null;
            Username = null;
        }
    }
}
