using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Tüm ürünleri liste olarak döndürür
        /// </summary>
        /// <returns></returns>
        Task<IDataResult<List<ProductListDTO>>> GetAllAsync(string sortOrder ,string searchQuery);
		Task<IDataResult<List<ProductListDTO>>> GetAllAsync();
		/// <summary>
		/// Verilen Id değerine sahip ürünü döndürür.
		/// </summary>
		/// <param name="productId">İstenen ürünün Id değeri</param>
		/// <returns></returns>
		Task<IDataResult<ProductDTO>> GetByIdAsync(Guid productId);
        /// <summary>
        /// Yeni bir ürün oluşturur.
        /// </summary>
        /// <param name="productCreateDTO">Oluşturulacak ürünün özelliklerini içerir.</param>
        /// <returns></returns>
        Task<IDataResult<ProductDTO>> AddAsync(ProductCreateDTO productCreateDTO);
        /// <summary>
        /// ID'sine karşılık gelen ürünü verilen özellikler ile günceller
        /// </summary>
        /// <param name="productUpdateDTO"></param>
        /// <returns></returns>
        Task<IResult> UpdateAsync(ProductUpdateDTO productUpdateDTO);
        /// <summary>
        /// Id'si verilen ürünü siler
        /// </summary>
        /// <param name="productId">Silinecek olan ürünün Id değeri</param>
        /// <returns></returns>
        Task<IResult> DeleteAsync(Guid productId);
        Task<IResult> UpdateAllProductsAsync();

        Task<bool> IsProductInOrderAsync(Guid productId);
        /// <summary>
        /// Tüm Ürünleri getirir.
        /// /// <param name="sortOrder">Ürün listesinin sıralama düzenini belirten parametre.
        /// </summary>
        /// <returns>Tüm Ürün listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<ProductListDTO>>> GetAllAsyncProduct(string sortOrder);
    }
}
