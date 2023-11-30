using System.ComponentModel.DataAnnotations;

namespace AgileProject.Models
{
    public class comments
    
    {
        [Key]
        public Guid comment_id { get; set; }
        public Guid ticket_id { get; set; }
        public string data { get; set; }
        public string user_id { get; set; }
    }
}
