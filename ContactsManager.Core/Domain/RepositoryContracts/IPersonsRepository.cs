using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    public interface IPersonsRepository
    {
        Task<Person> AddPerson(Person person);
        Task<List<Person>> GetAllPersons();
        Task<Person?> GetPersonById(Guid id);
        Task<bool> DeletePerson(Guid id);
        Task<Person> UpdatePerson(Person person);
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
    }
}
