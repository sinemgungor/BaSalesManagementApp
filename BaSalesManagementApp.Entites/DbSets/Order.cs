
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Order : AuditableEntity
    {
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }

        public Guid AdminId { get; set; }
        public virtual Admin Admin { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
