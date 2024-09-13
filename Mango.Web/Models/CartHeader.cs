using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Web.Models
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CuponCode { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
