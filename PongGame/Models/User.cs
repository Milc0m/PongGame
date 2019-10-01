using System;
using Microsoft.AspNetCore.Identity;

namespace PongGame.Models
{
    public class User : IdentityUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime LastVisit { get; set; }
        public DateTime RegistrationDate { get; set; }

        
    }
}
