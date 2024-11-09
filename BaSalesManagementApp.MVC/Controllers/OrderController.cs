using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly IAdminService _adminService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        /// <summary>
        /// OrderController kurucusu, IOrderService bağımlılığını alır.
        /// </summary>
        /// <param name="orderService">Sipariş hizmeti</param>
        public OrderController(IOrderService orderService, IProductService productService, ICompanyService companyService, IAdminService adminService, IStringLocalizer<Resource> stringLocalizer)
        {
            _orderService = orderService;
            _productService = productService;
            _companyService = companyService;
            _adminService = adminService;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Tüm siparişleri listeleyen ana sayfa görünümünü döndürür.
        /// </summary>
        /// <returns>Sipariş listesini gösteren ana sayfa görünümü</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "date", int pageSize = 10)
        {
            try
            {
                int pageNumber = page ?? 1;

                ViewBag.CurrentSort = sortOrder;
                ViewBag.CurrentPageSize = pageSize;

                var result = await _orderService.GetAllAsync(sortOrder);
                if (!result.IsSuccess)
                {

                    NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
                    // NotifyError(result.Message);
                    return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                }

                var orderListVM = result.Data.Select(order => order.Adapt<OrderListVM>()).ToList();

                var paginatedList = orderListVM.ToPagedList(pageNumber, pageSize);
                NotifySuccess(_stringLocalizer["ORDER_LISTED_SUCCESS"]);
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifySuccess(_stringLocalizer["ORDER_LISTED_SUCCESS"]);
                // NotifyError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Yeni bir sipariş oluşturma sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir sipariş oluşturma sayfası</returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                var productsResult = await _productService.GetAllAsync();
                var orderCreateVM = new OrderCreateVM
                {
                    Products = productsResult.Data
                };
                return View(orderCreateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Siparişi oluşturan adminin bilgileriyle birlikte bir siparişi oluşturur ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderCreateVM">Oluşturulacak siparişin verileri</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateVM orderCreateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"]);
                    orderCreateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderCreateVM);
                }

                // Doğru claim türünü kullanarak IdentityId'yi alın
                var identityIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                var identityId = identityIdClaim?.Value;

                if (string.IsNullOrEmpty(identityId))
                {
                    throw new Exception(_stringLocalizer[Messages.IDENTITY_ID_NOT_DETERMINED]);
                }

                var orderCreateDTO = orderCreateVM.Adapt<OrderCreateDTO>(); // Mapster ile dönüştürülme
                orderCreateDTO.Id = Guid.NewGuid(); // Yeni bir orderId oluşturma


             

                var result = await _orderService.AddAsync(orderCreateDTO, identityId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"]);
                    //NotifyError(result.Message);
                    orderCreateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderCreateVM);
                }

                NotifySuccess(_stringLocalizer["ORDER_CREATED_SUCCESS"]);
                //NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"] + ": " + ex.Message);
                //NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Ürün silinmiş olsa bile belirtilen siparişin detaylarını gösterir. 
        /// </summary>
        /// <param name="orderId">Gösterilecek siparişin ID'si</param>
        /// <returns>Sipariş detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetOrderWithDetailsByIdAsync(orderId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }



                var adminResult = await _adminService.GetByIdAsync(result.Data.AdminId);
                var adminDTO = adminResult?.Data ?? new AdminDTO { FirstName = "Bilinmeyen", LastName = "Admin" };

                // OrderDetailsVM'i oluştur ve değerleri ata
                var orderDetailsVM = result.Data.Adapt<OrderDetailsVM>();
                orderDetailsVM.Admin = adminDTO;

                // Deleted olmayan order detail'leri filtrele
                orderDetailsVM.OrderDetails = orderDetailsVM.OrderDetails
                    .Where(od => od.Status != Status.Deleted).ToList();



                NotifySuccess(_stringLocalizer["ORDER_FOUND_SUCCESS"]);
                // NotifySuccess(result.Message);
                return View(orderDetailsVM);
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Belirtilen siparişin güncelleme sayfasını gösterir.
        /// </summary>
        /// <param name="orderId">Güncellenecek siparişin ID'si</param>
        /// <returns>Sipariş güncelleme sayfası</returns>
        public async Task<IActionResult> Update(Guid orderId)
        {
            var result = await _orderService.GetByIdAsync(orderId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var orderUpdateVM = result.Data.Adapt<OrderUpdateVM>();

            // Deleted olmayan order detail'leri filtrele
            orderUpdateVM.OrderDetails = orderUpdateVM.OrderDetails
                .Where(od => od.Status != Core.Enums.Status.Deleted).ToList();

            foreach (var orderDetail in orderUpdateVM.OrderDetails)
            {
                var product = (await _productService.GetByIdAsync(orderDetail.ProductId)).Data;
                orderDetail.ProductName = product.Name;
            }
            orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;

            return View(orderUpdateVM);
        }

        /// <summary>
        ///  Sipariş bilgilerini günceller.
        /// </summary>
        /// <param name="orderUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OrderUpdateVM orderUpdateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                    orderUpdateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderUpdateVM);
                }

                var orderUpdateDTO = orderUpdateVM.Adapt<OrderUpdateDTO>(); // Mapster ile dönüştürülme

                var result = await _orderService.UpdateAsync(orderUpdateDTO);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                    //NotifyError(result.Message);
                    orderUpdateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderUpdateVM);
                }

                NotifySuccess(_stringLocalizer["ORDER_UPDATE_SUCCESS"]);
                //NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"] + ": " + ex.Message);
                //NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Belirtilen ID'li siparişi siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        public async Task<IActionResult> Delete(Guid orderId)
        {
            try
            {
                var result = await _orderService.DeleteAsync(orderId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"]);
                    //NotifyError(result.Message);
                }
                else
                {
                    NotifySuccess(_stringLocalizer["ORDER_DELETED_SUCCESS"]);
                    //NotifySuccess(result.Message);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }
    }
}
