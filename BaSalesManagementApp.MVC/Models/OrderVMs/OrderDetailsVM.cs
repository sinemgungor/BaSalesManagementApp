using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderDetailsVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }

        public AdminDTO Admin { get; set; }
        public List<OrderDetailListDTO> OrderDetails { get; set; }
	}
}