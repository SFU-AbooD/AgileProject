using System.ComponentModel.DataAnnotations;

namespace AgileProject.Models
{
    public class User_
    {
        [Key]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
