using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace AgileProject.Models
{
    public class Tickets
    {
        [Key]
        public Guid id { get; set; } = new Guid();
        [Required]
        public string Taskname { get; set; }
        public string Taskstatus { get; set; } = "todo";
        public DateTime Created { get; } = DateTime.Now;
        [Required]
        [DataType(DataType.Date)]
        public DateTime due { get; set; }
        [Required]
        public string user_id { get; set; }
    }
}
