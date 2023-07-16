using CrudExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CrudExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    List<SelectListItem> countryItems = countries.Select(country => new SelectListItem()
                    {
                        Text = country.Name,
                        Value = country.Id.ToString()
                    }).ToList();

                    personsController.ViewBag.CountryItems = countryItems;
                    personsController.ViewBag.Errors = personsController.ModelState.Values
                        .SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage)
                        .ToList();

                    var person = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(person); // skips the action method
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }

        }
    }
}
