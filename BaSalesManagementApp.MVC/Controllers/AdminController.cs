using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.Business.Utilities;
using X.PagedList;
using Microsoft.Extensions.Localization;


namespace BaSalesManagementApp.MVC.Controllers
{
	public class AdminController : BaseController
	{
		private readonly IAdminService _adminService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IStringLocalizer<Resource> _stringLocalizer;

		public AdminController(IAdminService adminService, IWebHostEnvironment webHostEnvironment, IStringLocalizer<Resource> stringLocalizer)
		{
			_adminService = adminService;
			_webHostEnvironment = webHostEnvironment;
			_stringLocalizer = stringLocalizer;
		}
        //deneme

        //public async Task<IActionResult> Index(int? page, string sortAdmin = "name")
        //{
        //	try
        //	{
        //		int pageNumber = page ?? 1;
        //		int pageSize = 5;

        //		var result = await _adminService.GetAllAsync(sortAdmin);
        //		if (!result.IsSuccess)
        //		{
        //			NotifyError(result.Message);
        //			return View(Enumerable.Empty<AdminListVM>().ToPagedList(pageNumber, pageSize));
        //		}

        //		var adminListVM = result.Data.Adapt<List<AdminListVM>>();

        //		var paginatedList = adminListVM.ToPagedList(pageNumber, pageSize);
        //		NotifySuccess(_stringLocalizer[Messages.ADMIN_LISTED_SUCCESS]);
        //              ViewData["CurrentSortAdmin"] = sortAdmin;
        //              ViewData["CurrentPage"] = pageNumber;
        //              return View(paginatedList);
        //	}
        //	catch (Exception ex)
        //	{
        //		NotifyError(_stringLocalizer[Messages.ADMIN_LISTED_ERROR] + ": " + ex.Message);
        //		return View("Error");
        //	}
        //}


        public async Task<IActionResult> Index(int? page, string sortAdmin = "name", int pageSize = 10)
        {
            try
            {
                int pageNumber = page ?? 1; // Sayfa numarası
                var result = await _adminService.GetAllAsync(sortAdmin);

                if (!result.IsSuccess)
                {
                    NotifyError(result.Message);
                    return View(Enumerable.Empty<AdminListVM>().ToPagedList(pageNumber, pageSize));
                }

                var adminListVM = result.Data.Adapt<List<AdminListVM>>();

                // PagedList ile sayfalama ve seçilen pageSize'a göre veri gösterimi
                var paginatedList = adminListVM.ToPagedList(pageNumber, pageSize);

                NotifySuccess(_stringLocalizer[Messages.ADMIN_LISTED_SUCCESS]);

                ViewData["CurrentSortAdmin"] = sortAdmin;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["CurrentPageSize"] = pageSize; // Seçilen pageSize'ı sakla

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_LISTED_ERROR] + ": " + ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR] + ": " + ex.Message);

				return View("Error");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AdminCreateVM adminCreateVM)
		{
			if (ModelState.IsValid)
			{

				adminCreateVM.FirstName = StringUtilities.CapitalizeEachWord(adminCreateVM.FirstName);
				adminCreateVM.LastName = StringUtilities.CapitalizeFirstLetter(adminCreateVM.LastName);

				var adminDto = adminCreateVM.Adapt<AdminCreateDTO>();
				byte[] photoBytes = null;
				if (adminCreateVM.Photo != null && adminCreateVM.Photo.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await adminCreateVM.Photo.CopyToAsync(memoryStream);
						photoBytes = memoryStream.ToArray();
					}
				}
				else
				{
					adminDto.PhotoData = null;
				}
                if (adminCreateVM.Photo != null)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(adminCreateVM.Photo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Photo", _stringLocalizer["Invalid file type. Please upload an image file."]);
                        
                        return View(adminCreateVM);
                    }
                }
                adminDto.PhotoData = photoBytes;
				var result = await _adminService.AddAsync(adminDto);
				if (!result.IsSuccess)
				{
					NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR]);
					//  NotifyError(result.Message);
					return View(adminCreateVM);
				}
				NotifySuccess(_stringLocalizer[Messages.ADMIN_CREATED_SUCCESS]);
				// NotifySuccess(result.Message);
				return RedirectToAction("Index");
			}
			return View(adminCreateVM);
		}

		public async Task<IActionResult> Delete(Guid adminId)
		{
			try
			{
				var result = await _adminService.DeleteAsync(adminId);
				if (!result.IsSuccess)
				{
					NotifySuccess(_stringLocalizer[Messages.ADMIN_DELETED_SUCCESS]);
					// NotifyError(result.Message);
				}

				NotifySuccess(result.Message);
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_DELETE_ERROR] + ": " + ex.Message);
				//NotifyError("Admin silinirken bir hata meydana geldi: " + ex.Message);
				return View("Index");
			}
		}
        public async Task<IActionResult> Update(Guid adminId)
        {
            var result = await _adminService.GetByIdAsync(adminId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            

            var adminUpdateVM = result.Data.Adapt<AdminUpdateVM>();

            if (adminUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(adminUpdateVM.PhotoData);
                adminUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }

            return View(adminUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AdminUpdateVM adminUpdateVM)
        {
            if (ModelState.IsValid)
            {
                var adminUpdateDto = adminUpdateVM.Adapt<AdminUpdateDTO>();

                // Yeni fotoğraf yükleme kontrolü
                if (adminUpdateVM.Photo != null && adminUpdateVM.Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await adminUpdateVM.Photo.CopyToAsync(memoryStream);
                        adminUpdateDto.PhotoData = memoryStream.ToArray();
                    }
                }

                // Admin güncelleme işlemi
                var result = await _adminService.UpdateAsync(adminUpdateDto);

                if (!result.IsSuccess)
                {
                    NotifyError(Messages.ADMIN_UPDATE_ERROR);
                    return View(adminUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.ADMIN_UPDATE_SUCCESS]);
                return RedirectToAction("Index");
            }

            return View(adminUpdateVM);
        }


        public async Task<IActionResult> Details(Guid adminId)
		{
			var result = await _adminService.GetByIdAsync(adminId);
			if (!result.IsSuccess)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
				// NotifyError(result.Message);
				return RedirectToAction("Index");
			}
			NotifySuccess(_stringLocalizer[Messages.ADMIN_GETBYID_SUCCESS]);
			// NotifySuccess(result.Message);
			return View(result.Data.Adapt<AdminDetailsVM>());
		}
	}
}
