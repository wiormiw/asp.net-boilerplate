using Application.Common.Interfaces.Users;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviour;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger logger, IUser user,
        IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName =  typeof(TRequest).Name;
        var userId =  _user.Id;
        var userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
            userName = await _identityService.GetUserNameAsync(userId) ?? "N/A";
        
        _logger.LogInformation("Handling Request: {RequestName} (User: {UserId} / {UserName}) {@RequestPayload}",
            requestName,
            string.IsNullOrEmpty(userId) ? "Anonymous" : userId,
            string.IsNullOrEmpty(userName) ? "Anonymous" : userName,
            request);
    }
}