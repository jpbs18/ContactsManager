using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _repository;

        public CountriesService(ICountriesRepository repository) 
        {
            _repository = repository;
        }


        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest is null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (string.IsNullOrEmpty(countryAddRequest.Name))
            {
                throw new ArgumentException(nameof(countryAddRequest.Name));
            }

            Country country = countryAddRequest.ConvertToCountry();

            if (await _repository.GetCountryByName(country.Name!) is not null)
            {
                throw new ArgumentException($"{nameof(country)}: {country}");
            }

            var result = await _repository.AddCountry(country);
            return result.ConvertToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries() =>
            (await _repository.GetAllCountries()).Select(country => country.ConvertToCountryResponse()).ToList();
        

        public async Task<CountryResponse?> GetCountryById(Guid? id) => id is null ? null 
           : (await _repository.GetCountryById(id.Value))?.ConvertToCountryResponse();

        public async Task<CountryResponse?> GetCountryByName(string name) => string.IsNullOrEmpty(name) ? null
            : (await _repository.GetCountryByName(name))?.ConvertToCountryResponse();

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for(int row = 2;  row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if(await _repository.GetCountryByName(countryName) is null)
                        {
                            Country countryToAdd = new Country(){ Name = countryName };
                            await _repository.AddCountry(countryToAdd);
                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }
    }
}