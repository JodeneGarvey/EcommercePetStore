using System.ComponentModel.DataAnnotations;

namespace EcommercePetStore.ViewModels
{
    public class EditUserViewModel
    {
        public string id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public IList<string> Roles { get; set; }
    }
}
