using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CrudExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile is null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select a file";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Please select the correct file extension (.xlsx)";
                return View();
            }

            int countries = await _countriesService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{countries} Countries uploaded!";
            return View();
        }
    }
}
