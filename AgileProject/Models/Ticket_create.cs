using System.ComponentModel.DataAnnotations;

namespace AgileProject.Models
{
    public class Ticket_create
    {
        [Required]
        public string Taskname { get; set; }
        [Required]
        public string user_id { get; set; }
    }
}
