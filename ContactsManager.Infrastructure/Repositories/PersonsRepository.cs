using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }


        public async Task<Person> AddPerson(Person person)
        {
            _logger.LogInformation("AddPerson of PersonsRepository");

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePerson(Guid id)
        {
            _logger.LogInformation("DeletePerson of PersonsRepository");

            var personsToDelete = _db.Persons.Where(person => person.Id == id);
            _db.Persons.RemoveRange(personsToDelete);
            int rowsDeleted = await _db.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsRepository");

            return await _db.Persons.Include(person => person.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _db.Persons.Include(person => person.Country).Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid id) => await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.Id == id);
        
        public async Task<Person> UpdatePerson(Person person)
        {
            _logger.LogInformation("UpdatePerson of PersonsRepository");

            var matchingPerson = await _db.Persons.FirstOrDefaultAsync(person => person.Id == person.Id);
            if (matchingPerson is null) return person;

            var properties = typeof(Person).GetProperties().Skip(1);

            foreach(var property in properties)
            {
                var updatedValue = property.GetValue(person);
                property.SetValue(matchingPerson, updatedValue);
            }

            await _db.SaveChangesAsync();
            return matchingPerson;
        }
    }
}
