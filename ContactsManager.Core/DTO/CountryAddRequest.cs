using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for adding a new country
    /// </summary>
    public class CountryAddRequest
    {
        public string? Name { get; set; }

        public Country ConvertToCountry() => new Country() { Id = Guid.NewGuid(), Name = Name };
        
    }
}
