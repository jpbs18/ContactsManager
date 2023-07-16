using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Please select a gender")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceivedNewsLetter { get; set; }

        public Person ConvertToPerson() => new() { 
            Id = Guid.NewGuid(), 
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
