using CrudExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CrudExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }   

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController controller = (PersonsController) context.Controller;

            controller.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name), "Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.BirthDate), "Date of Birth" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
            };

            controller.ViewBag.TableHeadings = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name), "Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.BirthDate), "Date of Birth" },
                { nameof(PersonResponse.Age), "Age" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
                { nameof(PersonResponse.ReceivedNewsLetter), "Received News Letter" },
            };

            Dictionary<string, object?>? parameters = (Dictionary<string, object?>?) context.HttpContext.Items["arguments"];

            if(parameters is not null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    controller.ViewBag.SearchBy = parameters["searchBy"];
                }

                if (parameters.ContainsKey("searchString"))
                {
                    controller.ViewBag.SearchString = parameters["searchString"];
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    controller.ViewBag.SortBy = parameters["sortBy"];
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    controller.ViewBag.SortOrder = Convert.ToString(parameters["sortOrder"]);
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.Name),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.BirthDate),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryId),
                        nameof(PersonResponse.Address),
                    };

                    if (!searchByOptions.Any(option => option == searchBy))
                    {
                        _logger.LogInformation("SearchBy received value is {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);
                    }
                }
            }
        }
    }
}
