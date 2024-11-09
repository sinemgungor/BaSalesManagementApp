using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderListDTO
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
		public Guid AdminId { get; set; }
		public string? AdminName { get; set; }
        public List<OrderDetailListDTO> OrderDetails { get; set; } = new List<OrderDetailListDTO>();
    }
}