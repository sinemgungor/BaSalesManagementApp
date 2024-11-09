using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IAdminService _adminService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        /// <summary>
        /// Müşteri kontrolcüsü
        /// </summary>
        /// <param name="customerService">Müşteri işlemleri servisi</param>
        /// <param name="companyService">Şirket işlemleri servisi</param>
        /// /// <param name="stringLocalizer">Çeviri hizmeti.</param>
        /// <param name="countryService">Ülke işlemleri servisi.</param>
        /// <param name="cityService">Şehir işlemleri servisi.</param>
        public CustomerController(ICustomerService customerService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, ICountryService countryService, ICityService cityService, IAdminService adminService)
        {
            _customerService = customerService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _countryService = countryService;
            _cityService = cityService;
            _adminService = adminService;
        }


        /// <summary>
        /// Müşterileri gösteren ana sayfa görünümü
        /// </summary>
        /// <returns>Tüm müşterileri listeleyen ana sayfa görünümünü döndürür.</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical", int pageSize = 10)
        {
            try
            {
                int pageNumber = page ?? 1;
                

                var result = await _customerService.GetAllAsync(sortOrder);
               var customerListVMs = result.Data.Adapt<List<CustomerListVM>>();

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CUSTOMER_LIST_FAILED]);
                    // NotifyError(result.Message);
                    return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                }

                NotifySuccess(_stringLocalizer[Messages.CUSTOMER_LISTED_SUCCESS]);
                //NotifySuccess(result.Message);
                var paginatedList = result.Data.Adapt<List<CustomerListVM>>().ToPagedList(pageNumber, pageSize);
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize; // Seçilen pageSize'ı sakla
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED] + ": " + ex.Message);
                return View("Error");
            }

        }
        /// <summary>
        /// Yeni bir müşteri oluşturma sayfası
        /// /// Şirket, şehir ve ülke bilgilerini doldurarak sayfa modelini hazırlar.
        /// </summary>
        /// <returns>Yeni bir müşteri oluşturma sayfasını döndürür.</returns>
        public async Task<IActionResult> Create()
        {
            var companiesRes = await _companyService.GetAllAsync();
            var citiesRes = await _cityService.GetAllAsync(); 
            var countriesRes = await _countryService.GetAllAsync();
            var model = new CustomerCreateVM
            {
                Companies = companiesRes.Data.Adapt<List<Company>>(),
                Cities = citiesRes.Data.Adapt<List<City>>(),
                Countries = countriesRes.Data.Adapt<List<Country>>()
            };
            return View(model);
        }

        /// <summary>
        /// Yeni bir müşteri oluşturma işlemi
        /// Form doğrulaması başarılı ise müşteri oluşturulur ve başarılı sonuç mesajıyla müşteri listesine yönlendirilir.
        /// </summary>
        /// <param name="model">Müşteri oluşturmak için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                // olası bir hata durumunda dropdown list'in tekrar doldurulması
                var companiesRes = await _companyService.GetAllAsync();
                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    model.Companies = companiesRes.Data.Adapt<List<Company>>();
                }
                var citiesRes = await _cityService.GetAllAsync();
                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    model.Cities = citiesRes.Data.Adapt<List<City>>();
                }
                var countriesRes = await _countryService.GetAllAsync();
                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    model.Countries = countriesRes.Data.Adapt<List<Country>>();
                }
                return View(model);
            }

            
            model.Name=StringUtilities.CapitalizeEachWord(model.Name);
            model.Address=StringUtilities.CapitalizeEachWord(model.Address);

            var result = await _customerService.AddAsync(model.Adapt<CustomerCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_ADD_ERROR]);
                //NotifyError(result.Message);
                return View(model);
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }




        /// <summary>
        /// Müşteri detaylarını gösteren sayfa
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri detaylarını gösteren sayfayı döndürür.</returns>
        public async Task<IActionResult> Details(Guid customerId)
        {
            try
            {

                var result = await _customerService.GetByIdAsync(customerId);
                
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CUSTOMER_NOT_FOUND]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }

                var adminResult = await _adminService.GetByIdentityIdAsync(result.Data.CreatedBy);
                
                var adminDetailsVM = adminResult?.Data.Adapt<AdminDetailsVM>() ?? new AdminDetailsVM { FirstName = "Bilinmeyen", LastName = "Bilinmeyen" , Email = "Bilinmeyen"};
                    
                var customerDetailsVM = result.Data.Adapt<CustomerDetailsVM>();
                customerDetailsVM.Admin = adminDetailsVM;

                NotifySuccess(_stringLocalizer["CUSTOMER_FOUND_SUCCESS"]);

                return View(customerDetailsVM);
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için sayfa
        /// Müşterinin şirket, şehir ve ülke bilgilerini de günceller.
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri bilgilerini güncellemek için sayfayı döndürür.</returns>
        public async Task<IActionResult> Update(Guid customerId)
        {
            var result = await _customerService.GetByIdAsync(customerId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var companiesResult = await _companyService.GetAllAsync();
            var citiesResult = await _cityService.GetAllAsync();
            var countriesResult = await _countryService.GetAllAsync();

            var customerUpdateVM = result.Data.Adapt<CustomerUpdateVM>();
            //CustomerUpdateVM customerUpdateVM = new CustomerUpdateVM();
            //customerUpdateVM.Phone = result.Data.Phone;
            //customerUpdateVM.CityId = result.Data.CityId;
            //customerUpdateVM.CompanyId = result.Data.CompanyId;
           

            customerUpdateVM.Companies = companiesResult.Data.Adapt<List<CompanyDTO>>();
            customerUpdateVM.Cities = citiesResult.Data.Adapt<List<CityDTO>>();
            customerUpdateVM.Countries = countriesResult.Data.Adapt<List<CountryDTO>>();

            return View(customerUpdateVM);
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için işlem
        /// </summary>
        /// <param name="customerUpdateVM">Müşteri bilgilerini güncellemek için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        public async Task<IActionResult> Update(CustomerUpdateVM customerUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                
                var companiesRes = await _companyService.GetAllAsync();
                var citiesRes = await _cityService.GetAllAsync();
                var countriesRes = await _countryService.GetAllAsync();

                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    customerUpdateVM.Companies = companiesRes.Data.Adapt<List<CompanyDTO>>();
                }
                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    customerUpdateVM.Cities = citiesRes.Data.Adapt<List<CityDTO>>();
                }

                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    customerUpdateVM.Countries = countriesRes.Data.Adapt<List<CountryDTO>>();
                }
                return View(customerUpdateVM);
            }

            
            customerUpdateVM.Name=StringUtilities.CapitalizeEachWord(customerUpdateVM.Name);
            customerUpdateVM.Address=StringUtilities.CapitalizeEachWord(customerUpdateVM.Address);

            var result = await _customerService.UpdateAsync(customerUpdateVM.Adapt<CustomerUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_UPDATED_FAILED]);
                //NotifyError(result.Message);
                return View(customerUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_UPDATED_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index"); ;
        }

        /// <summary>
        /// Müşteri silme işlemi
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        public async Task<IActionResult> Delete(Guid customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_DELETE_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_DELETE_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ülke ID'sine göre şehir listesini getirir.
        /// </summary>
        /// <param name="countryId">Şehirlerin getirileceği ülkenin benzersiz ID'si.</param>
        /// <returns>Belirtilen ülkeye ait şehirlerin listesini içeren bir JSON nesnesi.Eğer bir hata oluşursa, hata mesajı ile birlikte bad request döner.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCitiesByCountryId(Guid countryId)
        {
            try
            {
                var citiesResult = await _cityService.GetByCountryIdAsync(countryId);
                var cities = citiesResult.Data?.Adapt<List<CityDTO>>();
                return Json(cities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error fetching cities.");
            }
        }


     
    }
}
