using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CrudExample.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var exceptionHttpContext = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if(exceptionHttpContext is not null && exceptionHttpContext.Error is not null)
            {
                ViewBag.ErrorMessage = exceptionHttpContext.Error.Message;
            }

            return View();
        }
    }
}
