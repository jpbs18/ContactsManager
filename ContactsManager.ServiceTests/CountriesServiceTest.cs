using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Moq;
using FluentAssertions;
using RepositoryContracts;

namespace CrudTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;

        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            CountryAddRequest? request = null;

            Func<Task> action = async () => await _countriesService.AddCountry(request);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task AddedCountry_NullCountryName_ToBeArgumentException()
        { 
            CountryAddRequest? request = new CountryAddRequest() { Name = null };

            Func<Task> action = async () => await _countriesService.AddCountry(request);

            await action.Should().ThrowAsync<ArgumentException>();
        }


        [Fact]
        public async Task AddedCountry_DuplicatedCountryName_ToBeArgumentException()
        {
            CountryAddRequest countryRequest = new CountryAddRequest() { Name = "Portugal" };
            Country country = countryRequest.ConvertToCountry();           

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(countryRequest);
                await _countriesService.AddCountry(countryRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }


        [Fact]
        public async Task AddedCountry_ProperCountryDetails_ToBeSuccsessful()
        {
            CountryAddRequest? request = new CountryAddRequest() { Name = "Portugal" };
            Country country = request.ConvertToCountry();
            CountryResponse expected = country.ConvertToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(null as Country);
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            CountryResponse result = await _countriesService.AddCountry(request);

            result.Should().Be(expected);
            result.Id.Should().NotBe(Guid.Empty);
        }

        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmpty()
        {
            var expected = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(expected);

            List<CountryResponse> result = await _countriesService.GetAllCountries();

            expected.Should().BeEquivalentTo(result);
        }


        [Fact]
        public async Task GetAllCountries_AddCountries_ToBeSuccessful()
        {
            var countries = new List<Country>()
            {
                new Country(){ Name = "Portugal" },
                new Country(){ Name = "Spain" }
            };

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> expected = countries.Select(country => country.ConvertToCountryResponse()).ToList();
            List<CountryResponse> result = await _countriesService.GetAllCountries();

            result.Should().NotBeEmpty();
            expected.Should().BeEquivalentTo(result); 
        }
        #endregion

        #region GetCountryById
        [Fact]
        public async Task GetCountryById_ValidCountryId_ToBeSuccessfull()
        {
            var country = new Country()
            {
                Id = Guid.NewGuid(),
                Name = "Brasil"
            };

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);

            CountryResponse? expected = country.ConvertToCountryResponse();
            CountryResponse? result = await _countriesService.GetCountryById(country.Id);

            result.Should().Be(expected);
        }


        [Fact]
        public async Task GetCountryById_NotExistingCountryId_ToBeNull()
        {
            Guid? invalidId = Guid.NewGuid();

            CountryResponse? result = await _countriesService.GetCountryById(invalidId);

            result.Should().BeNull();
        }


        [Fact]
        public async Task GetCountryById_NullCountryId_ToBeNull()
        {
            Guid? invalidId = null;

            CountryResponse? result = await _countriesService.GetCountryById(invalidId);

            result.Should().BeNull();
        }
        #endregion
    }
}
