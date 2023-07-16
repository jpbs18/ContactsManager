using ServiceContracts;
using ServiceContracts.Enums;
using ServiceContracts.DTO;
using Services;
using Entities;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace CrudTests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonService> _logger;
        private readonly Mock<ILogger<PersonService>> _loggerMock;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;

        public PersonServiceTest()
        {
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            _loggerMock = new Mock<ILogger<PersonService>>();
            _logger = _loggerMock.Object;

            _personService = new PersonService(_personsRepository, _logger);
        }

        #region GetPersonById
        [Fact]
        public async Task GetPersonById_InvalidId_ToNullReferenceException()
        {
            Guid personId = Guid.NewGuid();

            Func<Task> action = async() => await _personService.GetPersonById(personId);

            await action.Should().ThrowAsync<NullReferenceException>(); 
        }

        [Fact]
        public async Task GetPersonById_NotExistentId_ToBeNull()
        {
            Guid? personId = null;

            PersonResponse? response = await _personService.GetPersonById(personId);

            response.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonById_ValidId_ToBeSuccessful()
        {
            Person person = new() 
            {
                Name = "João Santos",
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = Convert.ToString(GenderOptions.Male),
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse expected = person.ConvertToPersonResponse();
            PersonResponse? result = await _personService.GetPersonById(person.Id);

            expected.Should().Be(result);
        }
        #endregion

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson_ToArgumentNullException()
        {
            PersonAddRequest? request = null;

            Func<Task> action = async() => await _personService.AddPerson(request);
            
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_NoNamePerson_ToArgumentException()
        {
            PersonAddRequest? request = new PersonAddRequest() { Name = null, Email = "Joao_10@hotmail.com" };
            Person person = request.ConvertToPerson();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            Func<Task> action = async () => await _personService.AddPerson(request);
            
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_DuplicatedName_ToArgumentException()
        {
            PersonAddRequest? request = new PersonAddRequest() { Name = "João", Email = "john@glintt.com" };
            PersonAddRequest? request2 = new PersonAddRequest() { Name = "João", Email = "john@glintt.com" };

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(request);
                await _personService.AddPerson(request2);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ValidPerson_ToBeSuccessful()
        {
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "João Santos",
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = GenderOptions.Male,
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            Person person = personAddRequest.ConvertToPerson();
            PersonResponse personExpected = person.ConvertToPersonResponse();

            _personsRepositoryMock.Setup(method => method.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            
            PersonResponse response = await _personService.AddPerson(personAddRequest);
            
            response.Should().Be(personExpected);
            response.Id.Should().NotBe(Guid.Empty);
        }
        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmpty()
        {
            var persons = new List<Person>();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            List<PersonResponse> list = await _personService.GetAllPersons();

            list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_ValidList_ToBeSuccessful()
        {
            var persons = new List<Person>()
            {
               new Person()
               {
                    Name = "João Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               },
               new Person()
               {
                    Name = "Joaquim Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               }
            };

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> expected = persons.Select(person => person.ConvertToPersonResponse()).ToList();
            List<PersonResponse> result = await _personService.GetAllPersons();

            result.Should().NotBeEmpty();
            expected.Should().BeEquivalentTo(result);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            var persons = new List<Person>()
            {
               new Person()
               {
                    Name = "João Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               },
               new Person()
               {
                    Name = "Joaquim Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               }
            };

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            var expected = persons.Select(person => person.ConvertToPersonResponse()).ToList();
            var result = await _personService.GetFilteredPersons(nameof(Person.Name), "");

            expected.Should().BeEquivalentTo(result);
        }


        [Fact]
        public async Task GetFilteredPersons_ValidSearchText_ToBeSuccessful()
        {
            var persons = new List<Person>()
            {
               new Person()
               {
                    Name = "João Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               },
               new Person()
               {
                    Name = "Joaquim Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               }
            };

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            var expected = persons.Select(person => person.ConvertToPersonResponse()).ToList();
            var result = await _personService.GetFilteredPersons(nameof(Person.Name), "Jo");

            expected.Should().BeEquivalentTo(result);
        }
        #endregion

        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersons_SortByNameInDescendingOrder()
        {
            var persons = new List<Person>()
            {
               new Person()
               {
                    Name = "João Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               },
               new Person()
               {
                    Name = "Joaquim Santos",
                    Email = "joao_10@hotmail.com",
                    BirthDate = DateTime.Parse("1991-04-16"),
                    Gender = Convert.ToString(GenderOptions.Male),
                    CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                    Address = "Maia",
                    ReceivedNewsLetter = true
               }
            };

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            var list = await _personService.GetAllPersons();
            var result = await _personService.GetSortedPersons(list, nameof(Person.Name), SortOrderOptions.DESC);

            result.Should().BeInDescendingOrder(person => person.Name);
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? personUpdate = null;

            Func<Task> action = async() => await _personService.UpdatePerson(personUpdate);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
        {
            PersonUpdateRequest? personUpdate = new PersonUpdateRequest() 
            {
                Id = Guid.NewGuid()
            };

            Func<Task> action = async () => await _personService.UpdatePerson(personUpdate);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            var person = new Person()
            {
                Name = null,
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = Convert.ToString(GenderOptions.Male),
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            PersonResponse expected = person.ConvertToPersonResponse();
            PersonUpdateRequest request = expected.ConvertToPersonUpdateRequest();

            Func<Task> action = async () => await _personService.UpdatePerson(request);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_ValidPerson_ToBeSuccessful()
        {
            var person = new Person()
            {
                Name = "Joao",
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = Convert.ToString(GenderOptions.Male),
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            PersonResponse expected = person.ConvertToPersonResponse();
            PersonUpdateRequest request = expected.ConvertToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);

            PersonResponse result = await _personService.UpdatePerson(request);

            result.Should().Be(expected);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidId_ToBeSuccessful()
        {
            var person = new Person()
            {
                Name = "Joao",
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = Convert.ToString(GenderOptions.Male),
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            var response = person.ConvertToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);

            bool result = await _personService.DeletePerson(response.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePerson_InvalidId_ToBeFalse()
        {
            var person = new Person()
            {
                Name = "Joao",
                Email = "joao_10@hotmail.com",
                BirthDate = DateTime.Parse("1991-04-16"),
                Gender = Convert.ToString(GenderOptions.Male),
                CountryId = Guid.Parse("83eafbcf6b934537fb2d08db70374104"),
                Address = "Maia",
                ReceivedNewsLetter = true
            };

            var response = person.ConvertToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(false);

            bool result = await _personService.DeletePerson(new Guid("06A47193E37D45679BEB21575B18AF9A"));

            result.Should().BeFalse();
        }
        #endregion
    }
}
