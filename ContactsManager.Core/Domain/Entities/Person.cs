using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(40)]
        public string? Name { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }

        [StringLength(120)]
        public string? Address { get; set; }
        public bool ReceivedNewsLetter { get; set; }

        [ForeignKey("CountryId")]
        public Country? Country { get; set; }
    }
}
