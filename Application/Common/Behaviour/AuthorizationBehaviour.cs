using Application.Common.Interfaces.Users;
using Application.Common.Security;

namespace Application.Common.Behaviour;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    
    public AuthorizationBehaviour(IUser user, IIdentityService identityService) 
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = Attribute.GetCustomAttributes(
                request.GetType(), typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .ToArray();
        
        if (!authorizeAttributes.Any())
            return await next(cancellationToken);
        
        if (_user.Id is null)
            throw new UnauthorizedAccessException();
        
        // RBAC
        var rolesAttributes = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToArray();

        if (rolesAttributes.Any())
        {
            var requiredRoles = rolesAttributes
                .SelectMany(a => 
                    a.Roles.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct();

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