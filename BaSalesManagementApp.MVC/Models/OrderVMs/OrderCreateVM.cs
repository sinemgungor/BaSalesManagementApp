using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderCreateVM
    {
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }


        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }
        public Guid AdminId { get; set; }
        public List<OrderDetailCreateDTO> OrderDetails { get; set; }/* = new List<OrderDetailCreateDTO>();*/
        public List<ProductListDTO> Products { get; set; } = new List<ProductListDTO>();
    }
}