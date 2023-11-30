using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace AgileProject.Models
{
    public class files
    {
        [Key]
        public Guid file_id { get; set; }
        public Guid ticket_id{ get; set; }
        public byte[] file { get; set; }
        public string file_name { get; set; }
        public string file_extention { get; set; }
    }
}
