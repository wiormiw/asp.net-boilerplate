using System.Reflection;

namespace Api.Infrastructure;

public static class MethodInfoExtensions
{
    public static bool IsAnonymous(this MethodInfo method)
    {
        var invalidChars = new[] {'<', '>'};
        return method.Name.IndexOfAny(invalidChars) >= 0;
    }

    public static void AnonymousMethod(this IGuardClause guardClause, Delegate input)
    {
        if (input.Method.IsAnonymous()) 
            throw new ArgumentException("Please provide endpoint name when using anonymous handlers.");
    }
}