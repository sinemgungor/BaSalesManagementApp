
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
	public class OrderRepository : EFBaseRepository<Order>, IOrderRepository
	{
		private readonly BaSalesManagementAppDbContext _context;
		public OrderRepository(BaSalesManagementAppDbContext context) : base(context)
		{
			_context = context;
		}
		/// <summary>
		/// Orders ile Admins, Products ve Companies tablolarını birleştirerek siparişleri listeler. Bu listede Admin bilgisinin ve aktif firmalarınn olduğu siparişlerin de gösterilmesini sağlar.
		/// </summary>
		/// <returns></returns>
		public async Task<List<Order>> GetAllWithAdminAsync()
		{
			var orders = await _context.Orders
				.Include(o => o.Admin)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
					.ThenInclude(p => p.Company)  // Company Name & Status Durumu İçin
				.Where(x => x.Status != Status.Deleted)
				.ToListAsync();

			return orders;
		}

		/// <summary>
		/// Orders ile Admins, Products ve Companies tablolarını birleştirerek aranan siparişe dahil eder.
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public async Task<Order> GetOrderWithAdminAsync(Guid orderId)
		{
			var order = await _context.Orders
				.Include(o => o.Admin)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
					.ThenInclude(p => p.Company)
				.Where(x => x.Status != Status.Deleted && x.Id == orderId)
				.FirstOrDefaultAsync();

			return order;
		}

	}
}
