using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeader? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDto? ProductDto { get; set; }
        public int Count { get; set; }
    }
}
