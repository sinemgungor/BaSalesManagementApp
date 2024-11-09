using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.Dtos.StockTypeSizeDTOs;
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class StockTypeSizeController : BaseController
    {

        private readonly IStockTypeSizeService _stockTypeSizeService;
        private readonly IStockTypeService _productTypeService;

        private readonly IStringLocalizer<Resource> _stringLocalizer;

        public StockTypeSizeController(IStockTypeSizeService stockTypeSizeService, IStockTypeService productTypeService, IStringLocalizer<Resource> stringLocalizer)
        {
            _stockTypeSizeService = stockTypeSizeService;
            _stringLocalizer = stringLocalizer;
            _productTypeService = productTypeService;

        }

        /// <summary>
        /// Tüm Stok Tipi Boyutlarını listeleyen ana sayfa.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? page, string sortOrder, int pageSize = 10)
        {
            int pageNumber = page ?? 1;
            ViewBag.CurrentSort = sortOrder;
            ViewData["CurrentPageSize"] = pageSize; // Pass the page size to the view for the dropdown

            // Fetch the list from the service
            var result = await _stockTypeSizeService.GetAllAsync();
            if (!result.IsSuccess)
            {
                // Notify error if the data fetch fails
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_LIST_FAILED]);
                return View(Enumerable.Empty<StockTypeSizeListVM>().ToPagedList(pageNumber, pageSize));
            }

            // Adapt result to ViewModel
            var stockTypeSizeVM = result.Data?.Adapt<List<StockTypeSizeListVM>>() ?? new List<StockTypeSizeListVM>();

            // Sorting logic
            stockTypeSizeVM = sortOrder switch
            {
                "sizeName_asc" => stockTypeSizeVM.OrderBy(x => x.Size).ToList(),
                "sizeName_desc" => stockTypeSizeVM.OrderByDescending(x => x.Size).ToList(),
                "date_asc" => stockTypeSizeVM.OrderBy(x => x.CreatedDate).ToList(),
                "date_desc" => stockTypeSizeVM.OrderByDescending(x => x.CreatedDate).ToList(),
                _ => stockTypeSizeVM.OrderBy(x => x.Size).ToList() // Default sorting by Size
            };

            // Notify success if data is retrieved successfully
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_LISTED_SUCCESS]);

            // Paginate the list
            var paginatedList = stockTypeSizeVM.ToPagedList(pageNumber, pageSize);
            return View(paginatedList);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var productTypeResult = await _productTypeService.GetAllAsync();
            var producTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();

            var stockTypeSizeCreateVM = new StockTypeSizeCreateVM()
            {
                StockTypes = producTypes,
            };
            return View(stockTypeSizeCreateVM);
        }

        /// <summary>
        /// Yeni bir Stok Tipi Boyutu oluşturur ve ana sayfaya yönlendirir.      
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(StockTypeSizeCreateVM stockTypeSizeCreateVM)
        {
            var stockTypes = await _productTypeService.GetAllAsync();
            stockTypeSizeCreateVM.StockTypes = stockTypes.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();
            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeCreateVM);
            }

            
            stockTypeSizeCreateVM.Size=StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Size);
            stockTypeSizeCreateVM.Description=StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Description);

            var result = await _stockTypeSizeService.AddAsync(stockTypeSizeCreateVM.Adapt<StockTypeSizeCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATE_FAILED]);
                //NotifyError(result.Message);
                return View(stockTypeSizeCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li Stok Tipi Boyutunu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeId">Silinecek Stok Tipi Boyutunun ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.DeleteAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETE_FAILED]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETED_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutu bilgilerini alıp güncelleme sayfası oluşturur
        /// </summary>
        /// <param name="stockTypeSizeId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Update(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {

                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_NOT_FOUND]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            var productTypeResult = await _productTypeService.GetAllAsync();
            var productTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();


            var stockTypeSizeEditVM = result.Data.Adapt<StockTypeSizeUpdateVM>();
            stockTypeSizeEditVM.StockTypes = productTypes;

            return View(stockTypeSizeEditVM);
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutunun bilgilerini günceller ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(StockTypeSizeUpdateVM stockTypeSizeUpdateVM)
        {
            var productTypeResult = await _productTypeService.GetAllAsync();
            stockTypeSizeUpdateVM.StockTypes = productTypeResult.Data?.Adapt<List<StockTypeDto>>() ?? new List<StockTypeDto>();

            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeUpdateVM);
            }

            stockTypeSizeUpdateVM.Size=StringUtilities.CapitalizeEachWord(stockTypeSizeUpdateVM.Size);
            stockTypeSizeUpdateVM.Description=StringUtilities.CapitalizeFirstLetter(stockTypeSizeUpdateVM.Description);

            var result = await _stockTypeSizeService.UpdateAsync(stockTypeSizeUpdateVM.Adapt<StockTypeSizeUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_FAILED]);
                //NotifyError(result.Message);
                return View(stockTypeSizeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Girilen Stok Tipi Boyutu detaylarını gösterir.
        /// </summary>
        /// <param name="stockTypeSizeId">Girilen Stok Tipi Boyutu ID'si</param>
        /// <returns>Stok Tipi Boyutu detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var stockTypeSizeDetailVM = result.Data.Adapt<StockTypeSizeDetailVM>();
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_FOUND_SUCCESS]);
            return View(stockTypeSizeDetailVM);
        }
    }
}

