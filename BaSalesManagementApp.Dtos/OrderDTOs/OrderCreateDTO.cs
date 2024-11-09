using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderCreateDTO
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
        public Guid AdminId { get; set; }
        public List<OrderDetailCreateDTO> OrderDetails { get; set; }
    }
}
