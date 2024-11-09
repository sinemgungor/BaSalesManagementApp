

using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public Guid? CityId { get; set; } = null;

        public Guid? CountryId { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public Status Status { get; set; }
        public List<CityDTO> Cities { get; set; } = new List<CityDTO>();
        public List<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
        public string? CountryCode { get; set; }

    }
}
