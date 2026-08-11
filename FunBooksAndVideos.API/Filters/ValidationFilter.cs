using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FunBooksAndVideos.API.Filters;

public class ValidationFilter<T> : IAsyncActionFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<T>()
            .FirstOrDefault();

        if (request is null)
        {
            await next();
            return;
        }

        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(new
            {
                errors
            });

            return;
        }

        await next();
    }
}