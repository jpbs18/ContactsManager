using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Diagnostics.Metrics;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries() => await _db.Countries.ToListAsync();  

        public async Task<Country?> GetCountryById(Guid id) => await _db.Countries.FirstOrDefaultAsync(country => country.Id == id);

        public async Task<Country?> GetCountryByName(string name) => await _db.Countries.FirstOrDefaultAsync(country => country.Name == name); 
    }
}