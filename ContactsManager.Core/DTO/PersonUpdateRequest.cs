using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Person name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email name can't be blank")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceivedNewsLetter { get; set; }

        public Person ConvertToPerson() => new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            BirthDate = BirthDate,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceivedNewsLetter = ReceivedNewsLetter,
        };
    }
}
