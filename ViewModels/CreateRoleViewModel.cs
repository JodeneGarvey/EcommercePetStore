using System.ComponentModel.DataAnnotations;

namespace EcommercePetStore.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
