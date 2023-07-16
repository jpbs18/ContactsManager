using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ResultFilters
{
    public class TokenResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
           
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("Authorization-Key", "A100");
        }
    }
}
