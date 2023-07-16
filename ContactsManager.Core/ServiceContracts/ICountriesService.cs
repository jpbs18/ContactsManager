using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Country entity manipulation
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>The country object after adding it with new Id</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all the countries
        /// </summary>
        /// <returns>List of all countries as List<CountryResponse></returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a specific country by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Matching country for a specific id</returns>
        Task<CountryResponse?> GetCountryById(Guid? id);

        /// <summary>
        /// Uploads countries form excel file into database
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns>The number of countries added to database</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}