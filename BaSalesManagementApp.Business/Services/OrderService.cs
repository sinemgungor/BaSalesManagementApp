using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;

namespace BaSalesManagementApp.Business.Services
{
    /// <summary>
    /// OrderService sınıfı, siparişlerle ilgili CRUD işlemlerini gerçekleştirir.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        /// <summary>
        /// OrderService kurucusu, IOrderRepository bağımlılığını alır.
        /// </summary>
        /// <param name="orderRepository">Sipariş deposu</param>
        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IAdminRepository adminRepository)
        {
            _orderRepository = orderRepository;
            _adminRepository = adminRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        /// <summary>
        /// Tüm siparişleri getirir.
        /// </summary>
        /// <returns>Tüm siparişlerin listesi</returns>
        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllWithAdminAsync();
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            OrderId = order.Id,
                            ProductId = od.Product.Id,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            TotalPrice = od.TotalPrice,
                            ProductName = od.Product.Name,
                            CompanyName = od.Product.Company.Name,
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }

        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder)
        {
            try
            {
                var orders = await _orderRepository.GetAllWithAdminAsync();
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            OrderId = order.Id,
                            ProductId = od.Product.Id,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            TotalPrice = od.TotalPrice,
                            ProductName = od.Product.Name,
                            CompanyName = od.Product.Company.Name,
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                switch (sortOrder.ToLower())
                {
                    case "date":
                        orderList = orderList.OrderByDescending(s => s.OrderDate).ToList();
                        break;
                    case "datedesc":
                        orderList = orderList.OrderBy(s => s.OrderDate).ToList();
                        break;
                    case "active":
                        orderList = orderList.Where(s => s.IsActive == true).ToList();
                        break;
                    case "inactive":
                        orderList = orderList.Where(s => s.IsActive == false).ToList();
                        break;
                        //case "alphabetical":
                        //    orderList = orderList.OrderBy(s => s.ProductName).ToList();
                        //    break;
                        //case "alphabeticaldesc":
                        //    orderList = orderList.OrderByDescending(s => s.ProductName).ToList();
                        //    break;
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Belirtilen ID'li siparişi getirir.
        /// </summary>
        /// <param name="orderId">Getirilecek siparişin ID'si</param>
        /// <returns>Belirtilen ID'li siparişin verileri</returns>
        public async Task<IDataResult<OrderDTO>> GetByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ErrorDataResult<OrderDTO>(Messages.ORDER_NOT_FOUND);
                }

                return new SuccessDataResult<OrderDTO>(order.Adapt<OrderDTO>(), Messages.ORDER_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderDTO>(Messages.ORDER_GET_FAILED + ex.Message);
            }
        }

		/// <summary>
		/// Aranan siparişi admin, product ve company bilgileri ile birlikte getirir.
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public async Task<IDataResult<OrderListDTO>> GetOrderWithDetailsByIdAsync(Guid orderId)
		{
			try
			{
				var order = await _orderRepository.GetOrderWithAdminAsync(orderId);
				if (order == null)
				{
					return new ErrorDataResult<OrderListDTO>(Messages.ORDER_NOT_FOUND);
				}

                var orderListDTO = order.Adapt<OrderListDTO>();

                var orderDetails = order.OrderDetails.ToList();
				for (int i = 0; i < orderDetails.Count; i++)
                {
                    orderListDTO.OrderDetails[i].CompanyDTO.Name = orderDetails[i].Product.Company.Name;
                    orderListDTO.OrderDetails[i].CompanyDTO.Address = orderDetails[i].Product.Company.Address;
                    orderListDTO.OrderDetails[i].CompanyDTO.Phone = orderDetails[i].Product.Company.Phone;
                    orderListDTO.OrderDetails[i].CompanyDTO.Status = orderDetails[i].Product.Company.Status;

                }

				return new SuccessDataResult<OrderListDTO>(orderListDTO, Messages.ORDER_FOUND_SUCCESS);
			}
			catch (Exception ex)
			{
				return new ErrorDataResult<OrderListDTO>(Messages.ORDER_GET_FAILED + ex.Message);
			}
		}

		/// <summary>
		/// Yeni bir sipariş oluşturur.
		/// </summary>
		/// <param name="orderCreateDTO">Oluşturulacak sipariş bilgileri</param>
		/// <returns>Sipariş oluşturma işleminin sonucu</returns>
		public async Task<IDataResult<OrderDTO>> AddAsync(OrderCreateDTO orderCreateDTO, string identityId)
        {
            try
            {
                //Identity Id yi kullan
                var admin = await _adminRepository.GetByIdentityId(identityId);
                if (admin == null)
                {
                    throw new Exception("Admin could not be determined from the IdentityId.");
                }

                var order = orderCreateDTO.Adapt<Order>();
                order.AdminId = admin.Id;

                // order değişkeni içerisindeki OrderDetails'a orderId burada eklendi
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangeAsync();

                // EF Relationship Tracking ve relationship fix-up sayesinde buraya gerek kalmadı
                //if (orderCreateDTO.OrderDetails != null && orderCreateDTO.OrderDetails.Any())
                //{
                //    foreach (var OrderDetailCreateDTO in orderCreateDTO.OrderDetails)
                //    {
                //        var orderDetail = OrderDetailCreateDTO.Adapt<OrderDetail>();
                //        orderDetail.OrderId = order.Id;
                //        await _orderDetailRepository.AddAsync(orderDetail);
                //    }
                //    await _orderDetailRepository.SaveChangeAsync();
                //}

                return new SuccessDataResult<OrderDTO>(order.Adapt<OrderDTO>(), Messages.ORDER_CREATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderDTO>(orderCreateDTO.Adapt<OrderDTO>(), Messages.ORDER_CREATE_FAILED + ex.Message);
            }
        }


       public async Task<IDataResult<OrderDTO>> UpdateAsync(OrderUpdateDTO orderUpdateDTO)
{
    try
    {
        // Mevcut siparişi getir
        var existingOrder = await _orderRepository.GetByIdAsync(orderUpdateDTO.Id);
        if (existingOrder == null)
        {
            return new ErrorDataResult<OrderDTO>(Messages.ORDER_NOT_FOUND);
        }
 
        // Sipariş verilerini güncelle
        existingOrder.TotalPrice = orderUpdateDTO.TotalPrice;
 
        // Track IDs of updated order details
        var updatedOrderDetailIds = new HashSet<Guid>();
 
        foreach (var detail in orderUpdateDTO.OrderDetails)
        {
            var existingDetail = existingOrder.OrderDetails.FirstOrDefault(d => d.Id == detail.Id);
 
            if (existingDetail != null)
            {
                existingDetail.Discount = detail.Discount;
                existingDetail.Quantity = detail.Quantity;
                existingDetail.ProductId = detail.ProductId;
                existingDetail.TotalPrice = detail.TotalPrice;
 
                // Track the updated detail ID
                updatedOrderDetailIds.Add(existingDetail.Id);
            }
            else
            {
                var newDetail = new OrderDetail
                {
                    OrderId = existingOrder.Id,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    Discount = detail.Discount,
                    TotalPrice = detail.TotalPrice

                };



                        // Track the updated detail ID
                        updatedOrderDetailIds.Add(newDetail.Id);
                        existingOrder.OrderDetails.Add(newDetail);
                await _orderDetailRepository.AddAsync(newDetail);
                      
            }
                    

                }


                // Remove order details not present in the updated order details
                var detailsToRemove = existingOrder.OrderDetails
                    .Where(d => !updatedOrderDetailIds.Contains(d.Id))
                    .ToList();



                //// Remove order details not present in the updated order details and marked as Deleted
                //var detailsToRemove = existingOrder.OrderDetails
                //    .Where(d => !updatedOrderDetailIds.Contains(d.Id) && d.Status == Status.Deleted) // Sadece status 'Deleted' olanları sil
                //    .ToList();


                foreach (var detail in detailsToRemove)
        {
            existingOrder.OrderDetails.Remove(detail);
            await _orderDetailRepository.DeleteAsync(detail); // Assuming there's a DeleteAsync method
        }
 
        await _orderRepository.UpdateAsync(existingOrder); // Update the order and its details
        await _orderRepository.SaveChangeAsync();
 
        return new SuccessDataResult<OrderDTO>(existingOrder.Adapt<OrderDTO>(), Messages.ORDER_UPDATE_SUCCES);
    }
    catch (Exception ex)
    {
        return new ErrorDataResult<OrderDTO>(Messages.ORDER_UPDATE_FAILED + ex.Message);
    }
}
        /// <summary>
        /// Belirtilen ID'li siparişi siler.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Sipariş silme işleminin sonucu</returns>
        public async Task<IResult> DeleteAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ErrorResult(Messages.ORDER_NOT_FOUND);
                }

                await _orderRepository.DeleteAsync(order);
                await _orderRepository.SaveChangeAsync();

                return new SuccessResult(Messages.ORDER_DELETED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.ORDER_DELETE_FAILED + ex.Message);
            }
        }

        public Task<IDataResult<OrderDTO>> AddAsync(OrderCreateDTO orderCreateDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<IDataResult<List<OrderListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                var orders = await _orderRepository.GetAllWithAdminAsync();

                // Siparişleri `ProductName`'e göre filtrele
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    orders = orders.Where(order => order.OrderDetails
                                        .Any(orderDetail => orderDetail.Product.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                                   .ToList();
                }

                // Siparişleri DTO'ya dönüştür
                var orderList = orders.Select(order => new OrderListDTO
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    IsActive = order.IsActive,
                    AdminName = order.Admin.FirstName + " " + order.Admin.LastName,
                    OrderDetails = order.OrderDetails
                        .Where(od => od.Product.Company.Status != Status.Deleted) // Silinmiş şirketlere ait order detail'lar gösterilmiyor
                        .Select(od => new OrderDetailListDTO
                        {
                            Id = od.Id,
                            ProductName = od.Product.Name,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice,
                            Discount = od.Discount,
                            CompanyName = od.Product.Company.Name,
                            IsCompanyActive = od.Product.Company.Status != Status.Passive
                        }).ToList()
                }).ToList();

                if (orderList == null || orderList.Count == 0)
                {
                    return new ErrorDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LIST_EMPTY);
                }

                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        orderList = orderList.OrderByDescending(s => s.OrderDate).ToList();
                        break;
                    case "datedesc":
                        orderList = orderList.OrderBy(s => s.OrderDate).ToList();
                        break;
                    case "active":
                        orderList = orderList.Where(s => s.IsActive == true).ToList();
                        break;
                    case "inactive":
                        orderList = orderList.Where(s => s.IsActive == false).ToList();
                        break;
                    //case "alphabetical":
                    //    orderList = orderList.OrderBy(s => s.ProductName).ToList();
                    //    break;
                    //case "alphabeticaldesc":
                    //    orderList = orderList.OrderByDescending(s => s.ProductName).ToList();
                    //    break;
                }

                return new SuccessDataResult<List<OrderListDTO>>(orderList, Messages.ORDER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OrderListDTO>>(new List<OrderListDTO>(), Messages.ORDER_LIST_FAILED + ex.Message);
            }
        }
    }
}
