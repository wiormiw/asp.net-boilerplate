using System.Collections.Concurrent;
using Application.Common.Interfaces.Users;
using Application.Common.Security;

namespace Application.Common.Behaviour;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private static readonly ConcurrentDictionary<Type, AuthorizeAttribute[]> _attributeCache = new();
    
    public AuthorizationBehaviour(IUser user, IIdentityService identityService) 
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request.GetType().IsDefined(typeof(AllowAnonymousAttribute), true))
            return await next(cancellationToken);
        
        var authorizeAttributes = _attributeCache.GetOrAdd(
            request.GetType(),
            type => type.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .OfType<AuthorizeAttribute>()
                .ToArray()
        );
        
        if (authorizeAttributes is [])
            return await next(cancellationToken);
        
        if (string.IsNullOrEmpty(_user.Id))
            throw new UnauthorizedAccessException();
        
        // RBAC
        var rolesAttributes = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToArray();

        if (rolesAttributes.Length > 0)
        {
            var requiredRoles = rolesAttributes
                .SelectMany(a => a.Roles.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .ToHashSet();

            var isAuthorized = false;
            foreach (var role in requiredRoles)
            {
                if (await _identityService.IsInRoleAsync(_user.Id, role))
                {
                    isAuthorized = true;
                    break;
                }
            }

            if (!isAuthorized)
                throw new UnauthorizedAccessException();
        }
        
        // Authenticated or authorization not needed
        return await next(cancellationToken);
    }
}