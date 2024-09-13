using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Models.Dto
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public ProductDto? ProductDto { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
