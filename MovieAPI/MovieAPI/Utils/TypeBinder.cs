using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MovieAPI.Utils;

public class TypeBinder<T>: IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var propertyName = bindingContext.ModelName;
        var value = bindingContext.ValueProvider.GetValue(propertyName);

        if (value == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        try
        {
            var deserializedValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
            bindingContext.Result = ModelBindingResult.Success(deserializedValue);
        }
        catch
        {
            bindingContext.ModelState.TryAddModelError(propertyName, "Wrong data type");
        }
        return Task.CompletedTask;
    }
}