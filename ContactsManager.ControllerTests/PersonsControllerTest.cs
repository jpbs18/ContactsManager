using Moq;
using ServiceContracts;
using FluentAssertions;
using CrudExample.Controllers;
using ServiceContracts.DTO;
using AutoFixture;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CrudTests
{
    public class PersonsControllerTest
    { 
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<IPersonService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;


        public PersonsControllerTest() 
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonService>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _loggerMock = new Mock<ILogger<PersonsController>>();

            _countriesService = _countriesServiceMock.Object;
            _personService = _personsServiceMock.Object;
            _logger = _loggerMock.Object;
        }


        #region Index

        [Fact]
        public async Task Index_ShouldReturnViewWithPersonsList()
        {
            // Arrange
            var list = _fixture.Create<List<PersonResponse>>();
            var personsController = new PersonsController(_personService, _countriesService, _logger);

            // Mocking
            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(list);
            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(list);

            // Action
            IActionResult actionResult = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
            viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(list);    
        }
        #endregion

        #region Create
        [Fact]
        public async Task Create_Errors_GoToCreateView()
        {
            // Arrange
            var personAddRequest = _fixture.Create<PersonAddRequest>();
            var personResponse = _fixture.Create<PersonResponse>();
            var countries = _fixture.Create<List<CountryResponse>>();
            var personsController = new PersonsController(_personService, _countriesService, _logger);

            // Mocking
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);

            // Action
            personsController.ModelState.AddModelError("Name: ", "Name can't be blank");
            IActionResult actionResult = await personsController.Create(personAddRequest);

            // Assert
            if(actionResult is ViewResult)
            {
                ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
                viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
                viewResult.ViewData.Model.Should().Be(personAddRequest);
            }

            if (actionResult is RedirectToActionResult)
            {
                RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);
                redirectResult.ActionName.Should().Be("Index");
            }
        }

        [Fact]
        public async Task Create_NoErrors_RedirectToIndexView()
        {
            // Arrange
            var personAddRequest = _fixture.Create<PersonAddRequest>();
            var personResponse = _fixture.Create<PersonResponse>();
            var countries = _fixture.Create<List<CountryResponse>>();
            var personsController = new PersonsController(_personService, _countriesService, _logger);

            // Mocking
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);

            // Action
            IActionResult actionResult = await personsController.Create(personAddRequest);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);
            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
