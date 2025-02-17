using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BibliotecaApp.Models
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; }

        //[Required(ErrorMessage = "RoleName")]
        [Display(Name = "RoleName", ResourceType = typeof(resources.Resource))]
        public string RoleName { get; set; }

        // Lista de utilizatori care au acest rol
        public List<UserModel> Users { get; set; }
    }
}
