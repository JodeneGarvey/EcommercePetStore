using Microsoft.AspNetCore.Identity;

namespace EcommercePetStore.Models
{
    public class Users: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
