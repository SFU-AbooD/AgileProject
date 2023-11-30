using System.ComponentModel.DataAnnotations;
namespace AgileProject.Models
{
    public class updates
    {
        [Key]
        public Guid update_id { get; set; } 
        public Guid ticket_id { get; set; } 
        public string message { get; set; }  
        public string user_id { get; set; }  
    }
}
