using Microsoft.AspNetCore.Mvc.Filters;

namespace Plumsail.Interview.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.ValueProviderFactories.Clear();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}