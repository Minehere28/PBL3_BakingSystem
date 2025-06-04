using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class UserViewModel
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public double Balance { get; set; }

    }
}
