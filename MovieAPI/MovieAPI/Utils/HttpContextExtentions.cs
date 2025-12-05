using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Utils;

public static class HttpContextExtentions
{
    public async static Task InsertParameterIntoHeader<T>(this HttpContext httpContext, IQueryable<T> query)
    {
        if(httpContext == null) throw new ArgumentNullException(nameof(httpContext));

        double quantity = await query.CountAsync();
        httpContext.Response.Headers.Add( "TotalRecord", quantity.ToString());
    }
}