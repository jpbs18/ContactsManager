using Entities;

namespace RepositoryContracts
{
    public interface ICountriesRepository
    {
       Task<Country> AddCountry(Country country);
       Task<List<Country>> GetAllCountries();
       Task<Country?> GetCountryById(Guid id);
       Task<Country?> GetCountryByName(string name);
    }
}