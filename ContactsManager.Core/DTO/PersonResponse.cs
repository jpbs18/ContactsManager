using System;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceivedNewsLetter { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is null || obj.GetType() != typeof(PersonResponse)) return false;
            
            PersonResponse? objToCompare = obj as PersonResponse;
            return objToCompare!.Id == this.Id;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}, Email: {Email}, Date of Birth: {BirthDate?.ToString("dd-MM-yyyy")}";
        }

        public PersonUpdateRequest ConvertToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                Id = Id,
                Name = Name,
                Email = Email,
                BirthDate = BirthDate,
                Gender = (GenderOptions) Enum.Parse(typeof(GenderOptions), Gender!, true),
                Address = Address,
                CountryId = CountryId,
                ReceivedNewsLetter = ReceivedNewsLetter,
            };
        }
    }

    public static class PersonExtensions
    {
        public static PersonResponse ConvertToPersonResponse(this Person? person) => new PersonResponse() 
        { 
            Id =  person!.Id, 
            Name = person.Name, 
            Email = person.Email,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            CountryId = person.CountryId,
            Country = person.Country?.Name,
            Address = person.Address,
            ReceivedNewsLetter = person.ReceivedNewsLetter,
            Age = person.BirthDate != null ? Math.Floor((DateTime.Now - person.BirthDate.Value).TotalDays / 365) : null
        };
    }
}
