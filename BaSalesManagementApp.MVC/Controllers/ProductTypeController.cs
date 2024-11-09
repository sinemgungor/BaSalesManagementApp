using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.ProductTypeVMs;
using BaSalesManagementApp.MVC.Models.ProductVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class ProductTypeController : BaseController
    {
        private readonly IStockTypeService productTypeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        private readonly ICategoryService categoryService;

        public ProductTypeController(IStockTypeService productTypeService, IStringLocalizer<Resource> stringLocalizer, ICategoryService categoryService)
        {
            this.productTypeService = productTypeService;
            _stringLocalizer = stringLocalizer;
            this.categoryService = categoryService;
        }

        #region Geçmiş index ?
        //public async Task<IActionResult> Index(int? page)
        //{
        //    int pageNumber = page ?? 1;
        //    int pageSize = 5;

        //    var result = await productTypeService.GetAllAsync();
        //    if (!result.IsSuccess)
        //    {
        //        NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_UNSUCCESS]);
        //        //NotifyError(result.Message);
        //        return View(Enumerable.Empty<ProductTypeListVM>().ToPagedList(pageNumber, pageSize));
        //    }


        //    NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_SUCCESS]);
        //    //NotifySuccess(result.Message);
        //    var paginatedList = result.Data.Adapt<List<ProductTypeListVM>>().ToPagedList(pageNumber, pageSize);
        //    return View(paginatedList);
        //} 
        #endregion


        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", int pageSize = 10)
        {
            // Sayfa numarası, parametre boşsa 1 olarak ayarlanır
            int pageNumber = page ?? 1;

            // Ürün tiplerini sırala ve al
            var result = await productTypeService.GetAllAsync(sortOrder);


            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_UNSUCCESS]);
                return View(Enumerable.Empty<ProductTypeListVM>().ToPagedList(pageNumber, pageSize));
            }

            // İşlem başarılıysa başarı bildirimi göster
            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_SUCCESS]);

            var productTypeListVM = result.Data.Adapt<ProductTypeListVM>();

            var paginatedList = result.Data.Adapt<List<ProductTypeListVM>>().ToPagedList(pageNumber, pageSize);

            // Görünüm için gerekli verileri ViewData'ya ekle
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["CurrentPageSize"] = pageSize;  // Seçili sayfa boyutunu sakla

            return View(paginatedList);
        }




        [HttpGet("ProductType/Detail/{productTypeId}")]
        public async Task<IActionResult> Detail(Guid productTypeId)
        {
            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var productTypeDetailVM = result.Data.Adapt<ProductTypeDetailVM>();
            return View(productTypeDetailVM);
        }

        public async Task<IActionResult> Add()
        {
            var categoryResult = await categoryService.GetAllAsync();

            var categories = categoryResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var productTypeCreateVM = new ProductTypeAddVM
            {
                Categories = categories
            };

            return View(productTypeCreateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductTypeAddVM productTypeAddVM)
        {
            var categoryResult = await categoryService.GetAllAsync();

            productTypeAddVM.Categories = categoryResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            if (!ModelState.IsValid) return View(productTypeAddVM);

            productTypeAddVM.Name = StringUtilities.CapitalizeEachWord(productTypeAddVM.Name);

            var result = await productTypeService.AddAsync(productTypeAddVM.Adapt<StockTypeAddDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_ADD_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeAddVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Delete/{productTypeId}")]
        public async Task<IActionResult> Delete(Guid productTypeId)
        {
            var result = await productTypeService.DeleteAsync(productTypeId);

            if (result.IsSuccess)
            {
                NotifySuccess(_stringLocalizer[result.Message]);

            }
            else
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_DELETE_UNSUCCESS]);
            }
                

            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Update/{productTypeId}")]
        public async Task<IActionResult> Update(Guid productTypeId)
        {
            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var categoriesResult = await categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var productTypeEditVM = result.Data?.Adapt<ProductTypeUpdateVM>();

            productTypeEditVM.Categories = categories;

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            return View(productTypeEditVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductTypeUpdateVM productTypeUpdateVM)
        {
            var categoriesResult = await categoryService.GetAllAsync();

            productTypeUpdateVM.Categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            //if (!ModelState.IsValid) 
            //{

            //    return View(productTypeUpdateVM);

            //}

            productTypeUpdateVM.Name = StringUtilities.CapitalizeEachWord(productTypeUpdateVM.Name);

            var result = await productTypeService.UpdateAsync(productTypeUpdateVM.Adapt<StockTypeUpdateDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }
    }
}