using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of the CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }

            CountryResponse? countryToCompare = obj as CountryResponse;

            return this.Id == countryToCompare!.Id && this.Name == countryToCompare.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ConvertToCountryResponse(this Country country)
        {
            return new CountryResponse() { Id = country.Id, Name = country.Name };
        }
    }
}
