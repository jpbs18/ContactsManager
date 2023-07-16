using CrudExample.Filters.ActionFilters;
using CrudExample.Filters.AuthorizationFilters;
using CrudExample.Filters.ResourceFilters;
using CrudExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudExample.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Custom-Key", "Custom-Value-Controller", 3 }, Order = 3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _countriesService = countriesService;
            _logger = logger;
        }


        [HttpGet]
        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Custom-Key", "Custom-Value-Action", 1 }, Order = 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy},  searchString: {searchString}, sortBy: {sortBy}, sortOrderOptions: {sortOrder}");

            List<PersonResponse> personsList = await _personService.GetFilteredPersons(searchBy, searchString);       
            List<PersonResponse> personsOrdered = await _personService.GetSortedPersons(personsList, sortBy, sortOrder);
                                
            return View(personsOrdered);
        }


        [HttpGet]
        [Route("[action]")]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "my-key", "my-value", 4 })]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            List<SelectListItem> countryItems = countries.Select(country => new SelectListItem() 
            { 
                Text = country.Name,
                Value = country.Id.ToString()
            }).ToList();

            ViewBag.CountryItems = countryItems; 

            return View();
        }


        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _personService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }


        [HttpGet]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personToUpdate = await _personService.GetPersonById(personId);

            if(personToUpdate is null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonUpdateRequest updatedPerson = personToUpdate.ConvertToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            List<SelectListItem> countryItems = countries.Select(country => new SelectListItem()
            {
                Text = country.Name,
                Value = country.Id.ToString()
            }).ToList();

            ViewBag.CountryItems = countryItems;

            return View(updatedPerson);
        }


        [HttpPost]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task <IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? person = await _personService.GetPersonById(personRequest.Id);

            if(person is null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons"); 
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? person = await _personService.GetPersonById(personId);

            return person is null ? RedirectToAction("Index", "Persons") : View(person); 
        }


        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest person)
        {
            PersonResponse? personToDelete = await _personService.GetPersonById(person.Id);

            if(personToDelete is null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personService.DeletePerson(person.Id);

            return RedirectToAction("Index", "Persons");
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            var persons = await _personService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData) 
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() 
                { 
                    Top = 20, 
                    Right = 20, 
                    Bottom = 20, 
                    Left = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetPersonsCSV();

            return File(memoryStream, "application/octet-stream", "persons.csv");
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
        }
    }
}
