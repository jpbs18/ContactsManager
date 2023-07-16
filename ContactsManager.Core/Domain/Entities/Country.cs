using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(30)]
        public string? Name { get; set; }

        public virtual ICollection<Person>? Persons { get; set; }
    }
}