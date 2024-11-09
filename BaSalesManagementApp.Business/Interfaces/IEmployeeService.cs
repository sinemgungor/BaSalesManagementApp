using BaSalesManagementApp.Dtos.EmployeeDTOs;

namespace BaSalesManagementApp.Business.Interfaces
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Yeni bir çalışan ekler.
        /// </summary>
        /// <param name="employeeCreateDTO">Eklenen çalışanın bilgileri</param>
        /// <returns>Eklenen çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> AddAsync(EmployeeCreateDTO employeeCreateDTO);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı getirir.
        /// </summary>
        /// <param name="employeeId">Getirilecek çalışanın ID'si</param>
        /// <returns>Belirtilen ID'ye sahip çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> GetByIdAsync(Guid employeeId);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı siler.
        /// </summary>
        /// <param name="employeeId">Silinecek çalışanın ID'si</param>
        /// <returns>çalışanı silme işleminin sonuç durumunu döndürür</returns>
        Task<IResult> DeleteAsync(Guid employeeId);

        /// <summary>
        /// Tüm çalışanları getirir.
        /// </summary>
        /// <returns>Tüm çalışanların listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync();
		Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortEmployee, string searchQuery);

        /// <summary>
        /// Tüm çalışanları getirir ve sıralama seçeneğine göre sıralar.
        /// </summary>
        /// <param name="sortEmployee">Sıralama düzeni (örn. "name", "date")</param>
        /// <returns>Tüm çalışanların listesini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<List<EmployeeListDTO>>> GetAllAsync(string sortEmployee);

        /// <summary>
        /// Belirtilen ID'ye sahip çalışanı verilen bilgilerle günceller.
        /// </summary>
        /// <param name="employeeUpdateDTO">Güncellenecek çalışanın bilgileri</param>
        /// <returns>Güncellenen çalışanın verilerini ve sonuç durumunu döndürür</returns>
        Task<IDataResult<EmployeeDTO>> UpdateAsync(EmployeeUpdateDTO employeeUpdateDTO);
    }
}
